using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.MSBuild;
using Tapper;
using UiRtc.TypeScriptGenerator.DataModels;
using UiRtc.TypeScriptGenerator.CustomExceptions;
using UiRtc.Typing.PublicInterface.Attributes;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.TypeScriptGenerator
{
    internal class DataCollectorService(ILogger<App> _logger)
    {
        private Compilation? _compilation;

        public async Task<IReadOnlyList<GeneratedSourceCode>> TsModelGenerator(string projectPath, CancellationToken cancelationToken)
        {
            _logger.Log(LogLevel.Information, "Transpiling projects models {path}...", Path.GetFullPath(projectPath));

            var compilation = await CreateCompilationAsync(projectPath, cancelationToken);

            var typeMapperProvider = new DefaultTypeMapperProvider(compilation, false);

            _logger.Log(LogLevel.Information, "Mappers added");

            //TODO: Most of options could be setup from command line
            var options = new TranspilationOptions(
                compilation,
                typeMapperProvider,
                SerializerOption.Json,
                NamingStyle.CamelCase,
                EnumStyle.Value,
                NewLineOption.Lf,
                4,
                false,
                true);

            var transpiler = new Transpiler(compilation, options, _logger);

            _logger.Log(LogLevel.Information, "Ready for Transpile");

            var transpile = transpiler.Transpile();
            _logger.Log(LogLevel.Information, "typeScript models has been generated: Count: {Count}", transpile.Count);

            return transpile;
        }

        public async Task<(IDictionary<string, IEnumerable<SenderDataRecord>> senders, IDictionary<string, IEnumerable<HandlerDataRecord>> handlers)> ExtractHubsContracts(
            string projectPath,
            CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, "Transpiling projects contracts {path}...", Path.GetFullPath(projectPath));

            var compilation = await CreateCompilationAsync(projectPath, cancellationToken);

            var senderSymbol = compilation.GetTypeByMetadataName($"{typeof(IUiRtcSenderContract<DummyHub>).GetGenericTypeDefinition().FullName}");

            var handlerSymbols = new[]
            {
                compilation.GetTypeByMetadataName(typeof(IUiRtcHandler<DummyHub>).GetGenericTypeDefinition().FullName!),
                compilation.GetTypeByMetadataName(typeof(IUiRtcHandler<DummyHub, object>).GetGenericTypeDefinition().FullName!),
                compilation.GetTypeByMetadataName(typeof(IUiRtcContextHandler<DummyHub>).GetGenericTypeDefinition().FullName!),
                compilation.GetTypeByMetadataName(typeof(IUiRtcContextHandler<DummyHub, object >).GetGenericTypeDefinition().FullName!)
            }.Where(symbol => symbol != null).ToArray(); // Filter out nulls

            _logger.Log(LogLevel.Information, "Finding handlers implementation");

            var handlerRecords = compilation.SyntaxTrees.SelectMany(tree =>
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                return handlerSymbols.SelectMany(symbol => GetHandlerData(tree, semanticModel, symbol));
            }); var handlerRecordsByHub = handlerRecords.GroupBy(g => g.hubName).ToDictionary(k => k.Key, g => g.AsEnumerable());

            _logger.Log(LogLevel.Information, "Finding senders contract");
            var senderRecords = compilation.SyntaxTrees.SelectMany(tree =>
            {
                return GetSenderData(tree, compilation.GetSemanticModel(tree), senderSymbol);
            });

            var senderRecordsByHub = senderRecords.GroupBy(g => g.hubName).ToDictionary(k => k.Key, g => g.AsEnumerable());

            _logger.Log(LogLevel.Information, "Found senders hubs: Count: {Count}", senderRecordsByHub.Count);
            _logger.Log(LogLevel.Information, "Found senders methods (Total): Count: {Count}", senderRecordsByHub.Sum(hub => hub.Value.Count()));

            _logger.Log(LogLevel.Information, "Found handlers hubs: Count: {Count}", handlerRecordsByHub.Count);
            _logger.Log(LogLevel.Information, "Found handlers methods (Total): Count: {Count}", handlerRecordsByHub.Sum(hub => hub.Value.Count()));


            return (senderRecordsByHub, handlerRecordsByHub);

        }

        private IEnumerable<HandlerDataRecord> GetHandlerData(SyntaxTree syntaxTree, SemanticModel semanticModel, INamedTypeSymbol? handlerSymbol)
        {
            var handlersCollection = new List<HandlerDataRecord>();

            if (handlerSymbol == null)
            {
                return handlersCollection;
            }

            var classDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classDeclarations)
            {
                var classNamedTypeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

                if (classNamedTypeSymbol != null &&
                    classNamedTypeSymbol.AllInterfaces.Select(i => i.ConstructedFrom).Contains(handlerSymbol, SymbolEqualityComparer.Default))
                {
                    //Here if class which determinate as handler implementation

                    //Concrete Interface which determinate that it's handler
                    var concreateInterface = classNamedTypeSymbol.AllInterfaces.FirstOrDefault(i => i.ConstructedFrom.Equals(handlerSymbol, SymbolEqualityComparer.Default));
                    if (concreateInterface == null) continue; // Skip if not found

                    var handlerMethodName = GetMethodName(classNamedTypeSymbol);
                    var hubName = GetHubNameFromAttributes(concreateInterface.TypeArguments.First());

                    //If this is handler with parameter
                    var modelType = concreateInterface.TypeArguments.Length > 1 ? concreateInterface.TypeArguments.Skip(1).First().Name : null;
                    var modelNamespace = concreateInterface.TypeArguments.Length > 1 ? concreateInterface.TypeArguments.Skip(1).First().ContainingNamespace?.ToString() : null;

                    var record = new HandlerDataRecord(hubName, handlerMethodName, modelType, modelNamespace);
                    _logger.Log(LogLevel.Information, $"For hub {hubName} has been found handler: {handlerMethodName}");

                    AddToHandlerDataRecords(handlersCollection, record);
                }
            }
            return handlersCollection;
        }

        private IEnumerable<SenderDataRecord> GetSenderData(SyntaxTree syntaxTree, SemanticModel semanticModel, INamedTypeSymbol? senderContractSymbol)
        {
            var senderDataRecords = new List<SenderDataRecord>();

            if (senderContractSymbol == null)
            {
                return senderDataRecords;
            }

            var interfaceDeclarations = syntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>();

            foreach (var interfaceDeclaration in interfaceDeclarations)
            {
                var interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration) as INamedTypeSymbol;

                if (interfaceSymbol != null && interfaceSymbol.AllInterfaces.Select(i => i.ConstructedFrom).Contains(senderContractSymbol, SymbolEqualityComparer.Default))
                {
                    //This interfaceSymbol is a contract for sender. Contains list of methods 

                    var hubName = GetHubNameFromAttributes(
                        interfaceSymbol
                            .AllInterfaces
                            .First(i => i.ConstructedFrom.Equals(senderContractSymbol, SymbolEqualityComparer.Default))
                            .TypeArguments[0]);

                    var sendorMethods = interfaceSymbol.GetMembers().OfType<IMethodSymbol>().ToList();
                    foreach (IMethodSymbol? sendorMethod in sendorMethods)
                    {
                        var modelType = sendorMethod.Parameters.FirstOrDefault()?.Type;
                        var modelTypeName = modelType?.Name;
                        var modelNamespace = modelType?.ContainingNamespace?.ToString();

                        var senderMethodName = GetMethodName(sendorMethod);

                        var record = new SenderDataRecord(hubName, senderMethodName, modelTypeName, modelNamespace);
                        _logger.Log(LogLevel.Information, $"For sender {hubName} has been found sendor methods: {senderMethodName}");

                        AddToSenderDataRecords(senderDataRecords, record);
                    }
                }
            }
            return senderDataRecords;
        }

        private void AddToHandlerDataRecords(List<HandlerDataRecord> records, HandlerDataRecord record)
        {
            if (records.Where(r => r.hubName == record.hubName
                && r.methodName == record.methodName
                && r.modelType == record.modelType).Any())
            {
                return;
            }

            if (records.Where(r => r.hubName == record.hubName
                && r.methodName == record.methodName).Any())
            {
                _logger.LogError("Hub {Hub} allowed to has the same sender {Handler} name only if this handlers has identical parameters.",
                                 record.hubName,
                                 record.methodName);
                throw new Exception("\"Hub allowed to has the same handler name only if this handlers has identical parameters.\"");
            }

            records.Add(record);
        }

        private void AddToSenderDataRecords(List<SenderDataRecord> records, SenderDataRecord record)
        {
            if (records.Where(r=> r.hubName == record.hubName
                && r.methodName == record.methodName
                && r.modelType == record.modelType).Any())
            {
                return;
            }

            if (records.Where(r => r.hubName == record.hubName
                && r.methodName == record.methodName).Any())
            {
                _logger.LogError("Hub {Hub} allowed to has the same sender {Handler} name only if this sender has identical parameters.",
                                 record.hubName,
                                 record.methodName);
                throw new Exception("\"Hub allowed to has the same sender name only if this sender has identical parameters.\"");
            }

            records.Add(record);
        }

        private async Task<Compilation> CreateCompilationAsync(string projectPath, CancellationToken cancelationToken, bool IsDiagnosticOn = false)
        {
            _logger.Log(LogLevel.Information, "Compile source project");

            if (_compilation != null)
            {
                _logger.Log(LogLevel.Information, "Project has been compiled... return cache.");
                return _compilation;
            }

            using var workspace = MSBuildWorkspace.Create();
            _logger.Log(LogLevel.Information, "BuildWorkspace created");

            workspace.LoadMetadataForReferencedProjects = true;
            if (IsDiagnosticOn)
            {
                _logger.Log(LogLevel.Information, "Detail Diagnostic is On");

                workspace.WorkspaceFailed += (sender, args) =>
                {
                    _logger.Log(LogLevel.Warning, $"Workspace failed: {args.Diagnostic.Message} ");
                };
            }

            var msBuildProject = await workspace.OpenProjectAsync(projectPath, cancellationToken: cancelationToken);
            _logger.Log(LogLevel.Information, "Project has been opened");

            var compilation = await msBuildProject.GetCompilationAsync(cancelationToken);
            if (compilation is null)
            {
                _logger.Log(LogLevel.Error, $"Failed to create compilation for {projectPath}");
                throw new InvalidOperationException($"Failed to create compilation for {projectPath}");
            }

            _logger.Log(LogLevel.Information, "compilation has been done");

            _compilation = compilation;
            return compilation;
        }

        private string GetMethodName(ISymbol symbol)
        {
            if (symbol == null) throw new ArgumentNullException(nameof(symbol));

            var attribute = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToString() == typeof(UiRtcMethodAttribute).FullName);

            var methodName = attribute?.ConstructorArguments.Any() == true ? attribute.ConstructorArguments[0].Value?.ToString() : null;

            return string.IsNullOrEmpty(methodName) ? symbol.Name : methodName;
        }

        private string GetHubNameFromAttributes(ITypeSymbol typedSymbol)
        {
            var attributes = typedSymbol.GetAttributes();
            var hubAttribute = attributes
                .FirstOrDefault(attr => attr.AttributeClass != null && attr.AttributeClass.Name == nameof(UiRtcHubAttribute));

            var hubName = hubAttribute?.ConstructorArguments.FirstOrDefault().Value?.ToString();
            if (string.IsNullOrEmpty(hubName))
            {
                return typedSymbol.Name;
            }

            return hubName;
        }

        private class DummyHub : IUiRtcHub
        {
        }
    }
}
