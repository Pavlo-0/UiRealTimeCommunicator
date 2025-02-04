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
            var handler1Symbol = compilation.GetTypeByMetadataName($"{typeof(IUiRtcHandler<DummyHub>).GetGenericTypeDefinition().FullName}");
            var handler2Symbol = compilation.GetTypeByMetadataName($"{typeof(IUiRtcHandler<DummyHub, object>).GetGenericTypeDefinition().FullName}");

            _logger.Log(LogLevel.Information, "Finding handlers implementation");

            var handlerRecords = compilation.SyntaxTrees
                .SelectMany(tree =>
                {
                    var semanticModel = compilation.GetSemanticModel(tree);
                    return GetHandlerData(tree, semanticModel, handler1Symbol).Union(
                        GetHandlerData(tree, semanticModel, handler2Symbol));
                });
            var handlerRecordsByHub = handlerRecords.GroupBy(g => g.hubName).ToDictionary(k => k.Key, g => g.AsEnumerable());

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
                    var hubName = GetHubNameFromAttributes(concreateInterface.TypeArguments.First().GetAttributes(), handlerMethodName);

                    //If this is handler with parameter
                    var modelType = concreateInterface.TypeArguments.Length > 1 ? concreateInterface.TypeArguments.Skip(1).First().Name : null;

                    var record = new HandlerDataRecord(hubName, handlerMethodName, modelType);
                    _logger.Log(LogLevel.Information, $"For hub {hubName} has been found handler: {handlerMethodName}");
                    handlersCollection.Add(record);
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
                            .TypeArguments[0]
                            .GetAttributes(),
                        interfaceSymbol.Name);

                    var sendorMethods = interfaceSymbol.GetMembers().OfType<IMethodSymbol>().ToList();
                    foreach (IMethodSymbol? sendorMethod in sendorMethods)
                    {
                        var modelTypeName = sendorMethod.Parameters.FirstOrDefault()?.Type.Name;

                        var senderMethodName = GetMethodName(sendorMethod);

                        var record = new SenderDataRecord(hubName, senderMethodName, modelTypeName);
                        _logger.Log(LogLevel.Information, $"For sender {hubName} has been found sendor methods: {senderMethodName}");
                        senderDataRecords.Add(record);
                    }
                }
            }
            return senderDataRecords;
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

        private string GetHubNameFromAttributes(IEnumerable<AttributeData> attributes, string blameName)
        {
            var hubAttribute = attributes
                .FirstOrDefault(attr => attr.AttributeClass != null && attr.AttributeClass.Name == nameof(UiRtcHubAttribute));
            if (hubAttribute == null)
            {
                _logger.Log(LogLevel.Error, $"Attribute {nameof(UiRtcHubAttribute)} not found at {blameName}");
                throw new UiRtcHubAttributeNotFound($"Attribute {nameof(UiRtcHubAttribute)} not found at {blameName}");
            }

            var hubName = hubAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
            if (string.IsNullOrEmpty(hubName))
            {
                _logger.Log(LogLevel.Error, $"Name for {nameof(UiRtcHubAttribute)} should not be empty at {blameName}");
                throw new UiRtcHubAttributeNotFound($"Name for {nameof(UiRtcHubAttribute)} should not be empty at {blameName}");
            }

            return hubName;
        }

        private class DummyHub : IUiRtcHub
        {
        }
    }
}
