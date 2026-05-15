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
