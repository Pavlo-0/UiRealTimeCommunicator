﻿using Microsoft.AspNetCore.SignalR;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Public;

namespace UiRtc.Domain.HubMap
{
    internal class ProxyHub : Hub
    {
        public string HubName { get; init; }

        private readonly IConnectionInvokeService connectionInvokeService;

        public ProxyHub(IConnectionInvokeService connectionInvokeService)
        {
            this.connectionInvokeService = connectionInvokeService;
            
        }

        public async override Task OnConnectedAsync()
        {
            await connectionInvokeService.OnConnectedAsync(HubName, new ProxyContext(Context));
            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            await connectionInvokeService.OnDisconnectedAsync(HubName, new ProxyContext(Context), exception);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
