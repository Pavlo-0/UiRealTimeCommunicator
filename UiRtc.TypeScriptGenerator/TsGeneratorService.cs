using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UiRtc.TypeScriptGenerator.DataModels;
using Tapper;

namespace UiRtc.TypeScriptGenerator
{
    internal class TsGeneratorService
    {
        private readonly ILogger<App> _logger;

        public TsGeneratorService(ILogger<App> logger)
        {
            _logger = logger;
        }

        public string GenerateService(IDictionary<string, IEnumerable<SenderDataRecord>> senders, IDictionary<string, IEnumerable<HandlerDataRecord>> consumers, IEnumerable<GeneratedSourceCode> models, string outputDirectory)
        {
            // Save models as separate files in the provided output directory and get their base names
            var savedModelBaseNames = SaveModels(models, outputDirectory);

            // Generate import lines for saved models (not strictly injected unless template contains {{MODEL_IMPORTS}})
            var modelImportLines = GenerateModelImports(savedModelBaseNames);
            var modelImportsBlock = string.Join("\r\n", modelImportLines);

            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            string templateName = "Templates\\TsTemplate.v1.0.ts";
            string filePath = Path.Combine(assemblyPath, templateName);
            string template = File.ReadAllText(filePath);

            var hubNames = senders.Keys.Union(consumers.Keys).ToArray();
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";
            string version = GetVersionFromTemplate(templateName);

            string result = template
                .Replace("{{VERSION}}", version)
                .Replace("{{TIMESTAMP}}", timestamp)
                .Replace("{{MODEL_IMPORTS}}", modelImportsBlock)
                .Replace("{{HUBS}}", GenerateTypeUnion(hubNames))
                .Replace("{{ALL_HUBS}}", GenerateArrayItems(hubNames))
                .Replace("{{HUB_METHODS}}", GenerateTypeUnion(consumers.Values.SelectMany(methods => methods.Select(m => m.MethodName))))
                .Replace("{{HUB_METHOD_DEFINITIONS}}", string.Empty)
                .Replace("{{HUB_SUBSCRIPTIONS}}", GenerateTypeUnion(senders.Values.SelectMany(methods => methods.Select(m => m.MethodName))))
                .Replace("{{HUB_SUBSCRIPTION_DEFINITIONS}}", string.Empty)
                .Replace("{{CONNECTIONS}}", GenerateConnections(hubNames))
                .Replace("{{UI_RTC_SUBSCRIPTION}}", GenerateUiRtcSubscription(senders))
                .Replace("{{UI_RTC_COMMUNICATION}}", GenerateUiRtcCommunication(consumers));

            _logger.LogInformation("Generated TypeScript Code: {Length} bytes", result.Length);
            return result;
        }

        private List<string> SaveModels(IEnumerable<GeneratedSourceCode> models, string outputDirectory)
        {
            var savedBaseNames = new List<string>();

            if (models == null) return savedBaseNames;

            try
            {
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                foreach (var model in models)
                {
                    try
                    {
                        var safeFileName = model.SourceName;
                        var filePath = Path.Combine(outputDirectory, safeFileName);

                        // Delete existing file if any
                        if (File.Exists(filePath)) File.Delete(filePath);

                        File.WriteAllText(filePath, model.Content);
                        _logger.LogInformation("Saved model file {FilePath}", filePath);

                        // store base name without extension for imports
                        var baseName = Path.GetFileNameWithoutExtension(safeFileName);
                        if (!string.IsNullOrWhiteSpace(baseName))
                        {
                            savedBaseNames.Add(baseName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save model file {SourceName}", model.SourceName);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save models to directory {OutputDirectory}", outputDirectory);
                throw;
            }

            return savedBaseNames;
        }

        private IEnumerable<string> GenerateModelImports(IEnumerable<string> modelBaseNames)
        {
            if (modelBaseNames == null) yield break;

            foreach (var baseName in modelBaseNames)
            {
                if (string.IsNullOrWhiteSpace(baseName)) continue;

                var identifier = SanitizeIdentifier(baseName);

                yield return $"import * as {identifier} from \"./{baseName}\";";
            }
        }

        private static string SanitizeIdentifier(string input)
        {
            if (string.IsNullOrEmpty(input)) return "_";

            var identifier = Regex.Replace(input, "[^A-Za-z0-9_]", "_");
            if (string.IsNullOrEmpty(identifier)) return "_";
            if (char.IsDigit(identifier[0])) identifier = "_" + identifier;
            return identifier;
        }

        private string GetVersionFromTemplate(string fileName)
        {
            // Regular expression to match ".V" followed by a version number
            Match match = Regex.Match(fileName, @"\.v([\d\.]+)", RegexOptions.IgnoreCase);

            return match.Success ? match.Groups[1].Value : "undeterminate";
        }

        private static string GenerateTypeUnion(IEnumerable<string> values)
        {
            var literals = values.Select(TsStringLiteral).Distinct().ToArray();

            return literals.Length == 0
                ? "never"
                : string.Join("\r\n  | ", literals);
        }

        private static string GenerateArrayItems(IEnumerable<string> values) =>
            string.Join(",\r\n  ", values.Select(TsStringLiteral).Distinct());

        private static string GenerateConnections(string[] hubsName) =>
            string.Join("\r\n", hubsName.Select(h => $"  [{TsStringLiteral(h)}]: {{ }},"));

        private static string GenerateUiRtcSubscription(IDictionary<string, IEnumerable<SenderDataRecord>> senders)
        {
            var sb = new StringBuilder();
            foreach (var (hub, methods) in senders)
            {
                sb.AppendLine($"  [{TsStringLiteral(hub)}]: {{");
                foreach (var method in methods)
                {
                    var callBackParam = string.IsNullOrWhiteSpace(method.ModelType) || string.IsNullOrWhiteSpace(method.ModelNamespace) ? "" : $"data: {SanitizeIdentifier(method.ModelNamespace)}.{method.ModelType}";
                    sb.AppendLine($"    [{TsStringLiteral(method.MethodName)}]: (callBack: ({callBackParam}) => void) =>\r\n      subscribe({TsStringLiteral(method.HubName)}, {TsStringLiteral(method.MethodName)}, callBack),");
                }
                sb.AppendLine("  },");
            }
            return sb.ToString();
        }

        private static string GenerateUiRtcCommunication(IDictionary<string, IEnumerable<HandlerDataRecord>> consumers)
        {
            var sb = new StringBuilder();
            foreach (var (hub, methods) in consumers)
            {
                sb.AppendLine($"  [{TsStringLiteral(hub)}]: {{");
                foreach (var method in methods)
                {
                    sb.AppendLine(string.IsNullOrWhiteSpace(method.ModelType) || string.IsNullOrWhiteSpace(method.ModelNamespace)
                        ? $"    [{TsStringLiteral(method.MethodName)}]: () =>\r\n      send({TsStringLiteral(method.HubName)}, {TsStringLiteral(method.MethodName)}),"
                        : $"    [{TsStringLiteral(method.MethodName)}]: (request: {SanitizeIdentifier(method.ModelNamespace)}.{method.ModelType}) =>\r\n      send({TsStringLiteral(method.HubName)}, {TsStringLiteral(method.MethodName)}, request),"
                    );
                }
                sb.AppendLine("  },");
            }
            return sb.ToString();
        }

        private static string TsStringLiteral(string value) => JsonSerializer.Serialize(value);
    }
}
