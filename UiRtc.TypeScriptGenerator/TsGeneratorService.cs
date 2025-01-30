using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using UiRtc.TypeScriptGenerator.DataModels;

namespace UiRtc.TypeScriptGenerator
{
    internal class TsGeneratorService
    {
        private readonly ILogger<App> _logger;

        public TsGeneratorService(ILogger<App> logger)
        {
            _logger = logger;
        }

        public string GenerateService(IDictionary<string, IEnumerable<SenderDataRecord>> senders, IDictionary<string, IEnumerable<HandlerDataRecord>> consumers, string[] modelsContent)
        {
            string templateName = "TsTemplate.v1.0.ts";
            string template = File.ReadAllText(".\\Templates\\" + templateName);

            var hubNames = senders.Keys.Union(consumers.Keys).ToArray();
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " UTC";
            string version = GetVersionFromTemplate(templateName);

            string result = template
                .Replace("{{VERSION}}", version)
                .Replace("{{TIMESTAMP}}", timestamp)
                .Replace("{{HUBS}}", string.Join(" | ", hubNames.Select(h => $"'{h}'")))
                .Replace("{{ALL_HUBS}}", string.Join(", ", hubNames.Select(h => $"'{h}'")))
                .Replace("{{HUB_METHODS}}", string.Join(" | ", consumers.Keys.Select(h => $"{h}Method")))
                .Replace("{{HUB_METHOD_DEFINITIONS}}", GenerateHubMethodDefinitions(consumers))
                .Replace("{{HUB_SUBSCRIPTIONS}}", string.Join(" | ", senders.Keys.Select(h => $"{h}Subscription")))
                .Replace("{{HUB_SUBSCRIPTION_DEFINITIONS}}", GenerateHubSubscriptionDefinitions(senders))
                .Replace("{{CONNECTIONS}}", GenerateConnections(hubNames))
                .Replace("{{UI_RTC_SUBSCRIPTION}}", GenerateUiRtcSubscription(senders))
                .Replace("{{UI_RTC_COMMUNICATION}}", GenerateUiRtcCommunication(consumers))
                .Replace("{{MODELS}}", string.Join("\r\n\r\n", modelsContent));

            _logger.LogInformation("Generated TypeScript Code: {Length} bytes", result.Length);
            return result;
        }

        private string GetVersionFromTemplate(string fileName)
        {
            // Regular expression to match ".V" followed by a version number
            Match match = Regex.Match(fileName, @"\.v([\d\.]+)", RegexOptions.IgnoreCase);

            return match.Success ? match.Groups[1].Value : "undeterminate";
        }

        private static string GenerateHubMethodDefinitions(IDictionary<string, IEnumerable<HandlerDataRecord>> consumers) =>
            string.Join("\r\n", consumers.Select(c =>
                $"type {c.Key}Method = {string.Join(" | ", c.Value.Select(m => $"'{m.methodName}'"))};"));

        private static string GenerateHubSubscriptionDefinitions(IDictionary<string, IEnumerable<SenderDataRecord>> senders) =>
            string.Join("\r\n", senders.Select(s =>
                $"type {s.Key}Subscription = {string.Join(" | ", s.Value.Select(m => $"'{m.methodName}'"))};"));

        private static string GenerateConnections(string[] hubsName) =>
            string.Join("\r\n", hubsName.Select(h => $"  {h}: {{}},"));

        private static string GenerateUiRtcSubscription(IDictionary<string, IEnumerable<SenderDataRecord>> senders)
        {
            var sb = new StringBuilder();
            foreach (var (hub, methods) in senders)
            {
                sb.AppendLine($"  {hub}: {{");
                foreach (var method in methods)
                {
                    sb.AppendLine($"    {method.methodName}: (callBack: (data: {method.modelType}) => void) => subscribe(\"{method.hubName}\", \"{method.methodName}\", callBack),");
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
                sb.AppendLine($"  {hub}: {{");
                foreach (var method in methods)
                {
                    sb.AppendLine(method.modelType != null
                        ? $"    {method.methodName}: (request: {method.modelType}) => send(\"{method.hubName}\", \"{method.methodName}\", request),"
                        : $"    {method.methodName}: () => send(\"{method.hubName}\", \"{method.methodName}\"),");
                }
                sb.AppendLine("  },");
            }
            return sb.ToString();
        }
    }
}
