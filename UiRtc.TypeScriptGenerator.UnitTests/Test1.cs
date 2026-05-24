using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Tapper;
using UiRtc.TypeScriptGenerator;
using UiRtc.TypeScriptGenerator.DataModels;

namespace UiRtc.TypeScriptGenerator.UnitTests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void GeneratedContract_ImportsSignalRHttpConnectionOptions()
        {
            var contract = BuildGeneratedContract();

            StringAssert.Contains(contract, "IHttpConnectionOptions");
            StringAssert.Contains(contract, "from \"@microsoft/signalr\"");
        }

        [TestMethod]
        public void GeneratedContract_ExposesOptionalAuthenticationConfiguration()
        {
            var contract = BuildGeneratedContract();

            StringAssert.Contains(contract, "accessTokenFactory?: () => string | Promise<string>;");
            StringAssert.Contains(contract, "connectionOptions?: IHttpConnectionOptions;");
            StringAssert.Contains(contract, "hubConnectionOptions?: Partial<Record<uiRtcHubs, IHttpConnectionOptions>>;");
        }

        [TestMethod]
        public void GeneratedContract_BuildsHubConnectionFromConfigurationAndOptions()
        {
            var contract = BuildGeneratedContract();

            StringAssert.Contains(contract, "connections[hubName].connection = buildConnection(config, hubName);");
            StringAssert.Contains(contract, "const buildConnection = (\n  config: IUiRtcConfiguration,\n  hubName: uiRtcHubs\n): HubConnection =>");
            StringAssert.Contains(contract, "builder.withUrl(url, options);");
            StringAssert.Contains(contract, "const options = buildConnectionOptions(config, hubName);");
        }

        [TestMethod]
        public void GeneratedContract_KeepsLegacyConfigurationShapeValid()
        {
            var contract = BuildGeneratedContract();

            StringAssert.Contains(contract, "serverUrl: string;");
            StringAssert.Contains(contract, "activeHubs: uiRtcHubs[] | \"All\";");
            StringAssert.Contains(contract, "accessTokenFactory?:");
            StringAssert.Contains(contract, "await uiRtc.initAsync({ serverUrl: SERVER_URL, activeHubs: 'All' })");
        }

        [TestMethod]
        public async Task Generator_WithMissingProjectThrows()
        {
            var app = new App(NullLogger<App>.Instance);
            var missingProject = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.csproj");

            try
            {
                await app.Generator(missingProject, Path.GetTempPath());
                Assert.Fail("Expected missing project to throw.");
            }
            catch (FileNotFoundException)
            {
            }
        }

        [TestMethod]
        public void GenerateService_WhenModelOutputDirectoryCannotBeCreatedThrows()
        {
            var generator = new TsGeneratorService(NullLogger<App>.Instance);
            var outputFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.tmp");
            File.WriteAllText(outputFile, string.Empty);

            try
            {
                generator.GenerateService(
                    new Dictionary<string, IEnumerable<SenderDataRecord>>(),
                    new Dictionary<string, IEnumerable<HandlerDataRecord>>(),
                    Array.Empty<GeneratedSourceCode>(),
                    outputFile);

                Assert.Fail("Expected model output failure to throw.");
            }
            catch (IOException)
            {
            }
            finally
            {
                File.Delete(outputFile);
            }
        }

        [TestMethod]
        public void GenerateService_WithCustomNamesUsesEscapedStringLiteralKeys()
        {
            var contract = GenerateContract(
                new Dictionary<string, IEnumerable<SenderDataRecord>>
                {
                    ["chat-room"] = new[]
                    {
                        new SenderDataRecord("chat-room", "receive update\"\\", null, null)
                    }
                },
                new Dictionary<string, IEnumerable<HandlerDataRecord>>
                {
                    ["client hub"] = new[]
                    {
                        new HandlerDataRecord("client hub", "send-message", null, null)
                    }
                });

            StringAssert.Contains(contract, "type uiRtcHubs = \"chat-room\"\r\n  | \"client hub\";");
            StringAssert.Contains(contract, "type hubMethods = \"send-message\";");
            StringAssert.Contains(contract, "type hubSubscriptions = \"receive update\\u0022\\\\\";");
            StringAssert.Contains(contract, "[\"chat-room\"]: {");
            StringAssert.Contains(contract, "[\"receive update\\u0022\\\\\"]: (callBack:");
            StringAssert.Contains(contract, "subscribe(\"chat-room\", \"receive update\\u0022\\\\\", callBack)");
            StringAssert.Contains(contract, "[\"client hub\"]: {");
            StringAssert.Contains(contract, "[\"send-message\"]: () =>");
            Assert.IsFalse(contract.Contains("chat-roomSubscription"));
            Assert.IsFalse(contract.Contains("client hubMethod"));
        }

        [TestMethod]
        public void GenerateService_WithEmptyContractsUsesNeverUnions()
        {
            var contract = GenerateContract(
                new Dictionary<string, IEnumerable<SenderDataRecord>>(),
                new Dictionary<string, IEnumerable<HandlerDataRecord>>());

            StringAssert.Contains(contract, "type uiRtcHubs = never;");
            StringAssert.Contains(contract, "type hubMethods = never;");
            StringAssert.Contains(contract, "type hubSubscriptions = never;");
            Assert.IsFalse(contract.Contains("type hubMethods = \"undefined\";"));
        }

        [TestMethod]
        public void GenerateService_WithOneSidedContractsUsesNeverForMissingSide()
        {
            var senderOnlyContract = GenerateContract(
                new Dictionary<string, IEnumerable<SenderDataRecord>>
                {
                    ["notifications"] = new[] { new SenderDataRecord("notifications", "received", null, null) }
                },
                new Dictionary<string, IEnumerable<HandlerDataRecord>>());

            StringAssert.Contains(senderOnlyContract, "type uiRtcHubs = \"notifications\";");
            StringAssert.Contains(senderOnlyContract, "type hubMethods = never;");
            StringAssert.Contains(senderOnlyContract, "type hubSubscriptions = \"received\";");

            var handlerOnlyContract = GenerateContract(
                new Dictionary<string, IEnumerable<SenderDataRecord>>(),
                new Dictionary<string, IEnumerable<HandlerDataRecord>>
                {
                    ["commands"] = new[] { new HandlerDataRecord("commands", "send", null, null) }
                });

            StringAssert.Contains(handlerOnlyContract, "type uiRtcHubs = \"commands\";");
            StringAssert.Contains(handlerOnlyContract, "type hubMethods = \"send\";");
            StringAssert.Contains(handlerOnlyContract, "type hubSubscriptions = never;");
        }

        [TestMethod]
        public void GeneratedContracts_CompileWithTypeScriptWhenCompilerIsAvailable()
        {
            var tscPath = FindTypeScriptCompilerPath();
            if (tscPath is null)
            {
                return;
            }

            CompileGeneratedTypeScript(
                tscPath,
                GenerateContract(
                    new Dictionary<string, IEnumerable<SenderDataRecord>>
                    {
                        ["chat-room"] = new[]
                        {
                            new SenderDataRecord("chat-room", "receive update\"\\", null, null)
                        }
                    },
                    new Dictionary<string, IEnumerable<HandlerDataRecord>>
                    {
                        ["client hub"] = new[]
                        {
                            new HandlerDataRecord("client hub", "send-message", null, null)
                        }
                    }));

            CompileGeneratedTypeScript(
                tscPath,
                GenerateContract(
                    new Dictionary<string, IEnumerable<SenderDataRecord>>(),
                    new Dictionary<string, IEnumerable<HandlerDataRecord>>()));

            CompileGeneratedTypeScript(
                tscPath,
                GenerateContract(
                    new Dictionary<string, IEnumerable<SenderDataRecord>>
                    {
                        ["notifications"] = new[] { new SenderDataRecord("notifications", "received", null, null) }
                    },
                    new Dictionary<string, IEnumerable<HandlerDataRecord>>()));

            CompileGeneratedTypeScript(
                tscPath,
                GenerateContract(
                    new Dictionary<string, IEnumerable<SenderDataRecord>>(),
                    new Dictionary<string, IEnumerable<HandlerDataRecord>>
                    {
                        ["commands"] = new[] { new HandlerDataRecord("commands", "send", null, null) }
                    }));
        }

        private static string BuildGeneratedContract()
        {
            var template = File.ReadAllText(FindTemplatePath());

            var contract = template
                .Replace("{{VERSION}}", "1.0")
                .Replace("{{TIMESTAMP}}", "test")
                .Replace("{{MODEL_IMPORTS}}", string.Empty)
                .Replace("{{HUBS}}", "\"Chat\" | \"Weather\"")
                .Replace("{{ALL_HUBS}}", "\"Chat\",\n  \"Weather\"")
                .Replace("{{HUB_METHODS}}", "ChatMethod | WeatherMethod")
                .Replace("{{HUB_METHOD_DEFINITIONS}}", "type ChatMethod = \"SendMessage\";\ntype WeatherMethod = \"GetForecast\";")
                .Replace("{{HUB_SUBSCRIPTIONS}}", "ChatSubscription | WeatherSubscription")
                .Replace("{{HUB_SUBSCRIPTION_DEFINITIONS}}", "type ChatSubscription = \"ReceiveMessage\";\ntype WeatherSubscription = \"Forecast\";")
                .Replace("{{CONNECTIONS}}", "Chat: { },\n  Weather: { },")
                .Replace("{{UI_RTC_SUBSCRIPTION}}", "Chat: {\n    ReceiveMessage: (callBack: (data: any) => void) =>\n      subscribe(\"Chat\", \"ReceiveMessage\", callBack),\n  },")
                .Replace("{{UI_RTC_COMMUNICATION}}", "Chat: {\n    SendMessage: (request: any) =>\n      send(\"Chat\", \"SendMessage\", request),\n  },");

            return contract.Replace("\r\n", "\n");
        }

        private static string GenerateContract(
            IDictionary<string, IEnumerable<SenderDataRecord>> senders,
            IDictionary<string, IEnumerable<HandlerDataRecord>> consumers)
        {
            var directory = Directory.CreateTempSubdirectory("uirtc-ts-generator-");

            try
            {
                var generator = new TsGeneratorService(NullLogger<App>.Instance);
                return generator.GenerateService(senders, consumers, Array.Empty<GeneratedSourceCode>(), directory.FullName);
            }
            finally
            {
                directory.Delete(true);
            }
        }

        private static void CompileGeneratedTypeScript(string tscPath, string contract)
        {
            var directory = Directory.CreateTempSubdirectory("uirtc-ts-compile-");

            try
            {
                var signalRDirectory = Path.Combine(directory.FullName, "node_modules", "@microsoft", "signalr");
                Directory.CreateDirectory(signalRDirectory);
                File.WriteAllText(Path.Combine(directory.FullName, "contract.ts"), contract);
                File.WriteAllText(
                    Path.Combine(directory.FullName, "tsconfig.json"),
                    """
                    {
                      "compilerOptions": {
                        "target": "ES2020",
                        "lib": ["ES2020", "DOM", "DOM.Iterable"],
                        "module": "ESNext",
                        "moduleResolution": "node",
                        "strict": true,
                        "noEmit": true,
                        "skipLibCheck": true
                      },
                      "files": ["contract.ts"]
                    }
                    """);
                File.WriteAllText(
                    Path.Combine(signalRDirectory, "package.json"),
                    "{\"name\":\"@microsoft/signalr\",\"version\":\"0.0.0\",\"types\":\"index.d.ts\"}");
                File.WriteAllText(
                    Path.Combine(signalRDirectory, "index.d.ts"),
                    """
                    export interface IHttpConnectionOptions {
                      accessTokenFactory?: () => string | Promise<string>;
                    }

                    export enum HubConnectionState {
                      Connected = "Connected",
                      Connecting = "Connecting",
                      Reconnecting = "Reconnecting"
                    }

                    export class HubConnection {
                      state: HubConnectionState;
                      start(): Promise<void>;
                      stop(): Promise<void>;
                      on(methodName: string, newMethod: (...args: any[]) => void): void;
                      off(methodName: string, method?: (...args: any[]) => void): void;
                      send(methodName: string, ...args: any[]): Promise<void>;
                    }

                    export class HubConnectionBuilder {
                      withUrl(url: string, options?: IHttpConnectionOptions): this;
                      withAutomaticReconnect(): this;
                      build(): HubConnection;
                    }
                    """);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = $"\"{tscPath}\" --project \"{Path.Combine(directory.FullName, "tsconfig.json")}\" --pretty false",
                    WorkingDirectory = directory.FullName,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var process = Process.Start(startInfo);
                if (process is null)
                {
                    return;
                }

                var completed = process.WaitForExit(30000);
                if (!completed)
                {
                    process.Kill();
                    Assert.Fail("TypeScript compiler timed out.");
                }

                var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
                Assert.AreEqual(0, process.ExitCode, output);
            }
            catch (System.ComponentModel.Win32Exception)
            {
            }
            finally
            {
                directory.Delete(true);
            }
        }

        private static string? FindTypeScriptCompilerPath()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                var candidate = Path.Combine(
                    directory.FullName,
                    "IntegrationTest",
                    "FE01.IntegrationTest",
                    "node_modules",
                    "typescript",
                    "bin",
                    "tsc");

                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            return null;
        }

        private static string FindTemplatePath()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                var candidate = Path.Combine(
                    directory.FullName,
                    "UiRtc.TypeScriptGenerator",
                    "Templates",
                    "TsTemplate.v1.0.ts");

                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new FileNotFoundException("Could not locate TsTemplate.v1.0.ts from the test output directory.");
        }
    }
}
