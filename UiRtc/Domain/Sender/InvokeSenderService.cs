﻿using Microsoft.AspNetCore.SignalR;
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
        private string? HubName { get; set; }
        private string[]? ConnectionIds { get; set; }

        public void ResolveHub(string hubName)
        {
            HubName = hubName;
        }

        public void ResolveConnectionId(string[] connectionIds)
        {
            ConnectionIds = connectionIds;
        }


        public async Task Invoke(string method, object? model)
        {
            if (string.IsNullOrWhiteSpace(HubName))
            {
                throw new Exception("SignalR Hub name has not been set up");
            }

            var hubType = hubRepository.GetSignalRHubType(HubName);

            // Get the generic IHubContext<> type for the given hubType
            Type hubContextType = typeof(IHubContext<>).MakeGenericType(hubType);

            // Resolve the service dynamically
            var context = services.GetRequiredService(hubContextType) as IHubContext;

            if (context == null)
            {
                throw new Exception("SignalR Hub Context can't be obtained");
            }

            var clients = (ConnectionIds is { Length: > 0 })
     ? context.Clients.Clients(ConnectionIds.ToList())
     : context.Clients.All;

            if (model is null)
            {
                await clients.SendAsync(method);
            }
            else
            {
                var jsonModel = GetJSONModel(model);
                await clients.SendAsync(method, model);
            }
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
