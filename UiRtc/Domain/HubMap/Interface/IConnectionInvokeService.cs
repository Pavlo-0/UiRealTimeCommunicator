using Microsoft.AspNetCore.SignalR;

namespace UiRtc.Domain.HubMap.Interface
{
    internal interface IConnectionInvokeService
    {
        Task OnConnectedAsync(string hubName, HubCallerContext Context);
        Task OnDisconnectedAsync(string hubName, HubCallerContext Context, Exception? exception);
    }
}
