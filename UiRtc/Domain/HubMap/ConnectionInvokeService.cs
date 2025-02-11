using Microsoft.AspNetCore.SignalR;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository.Interface;

namespace UiRtc.Domain.HubMap
{
    internal class ConnectionInvokeService(IConnectionRepository connectionRepository,
                                           IServiceProvider serviceProvider) : IConnectionInvokeService
    {
        public async Task OnConnectedAsync(string hubName, HubCallerContext context)
        {
            string connectionId = context.ConnectionId;
            var hubConnections = connectionRepository.Get(hubName);
            foreach (var hubConnection in hubConnections)
            {
                var serviceInstance = serviceProvider.GetService(hubConnection.ConInterfaceImplementation);
                if (serviceInstance != null)
                {
                    dynamic dynamicInstance = serviceInstance;
                    await dynamicInstance.OnConnectedAsync(connectionId, context);
                }

            }
        }

        public async Task OnDisconnectedAsync(string hubName, HubCallerContext context, Exception? exception)
        {
            string connectionId = context.ConnectionId;
            var hubConnections = connectionRepository.Get(hubName);
            foreach (var hubConnection in hubConnections)
            {
                var serviceInstance = serviceProvider.GetService(hubConnection.ConInterfaceImplementation);
                if (serviceInstance != null)
                {
                    dynamic dynamicInstance = serviceInstance;
                    await dynamicInstance.OnDisconnectedAsync(connectionId, context, exception);
                }
            }
        }
    }
}
