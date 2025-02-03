using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UiRtc.Domain.Repository.Interface;
using UiRtc.Domain.Sender.Interface;

namespace UiRtc.Domain.Sender
{
    internal class InvokeSenderService(IServiceProvider services, IHubRepository hubRepository) : IInvokeSenderService
    {

        private string HubName { get; set; }

        public void ResolveHub(string hubName)
        {
            HubName = hubName;
        }

        public async Task Invoke(string method, object model)
        {
            var hubType = hubRepository.GetSignalRHubType(HubName);

            // Get the generic IHubContext<> type for the given hubType
            Type hubContextType = typeof(IHubContext<>).MakeGenericType(hubType);

            // Resolve the service dynamically
            var context = services.GetRequiredService(hubContextType) as IHubContext;

            string jsonModel = GetJSONModel(model);
            await context.Clients.All.SendAsync(method, model);
        }

        private string GetJSONModel(object model)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            return JsonConvert.SerializeObject(model, settings);
        }
    }
}
