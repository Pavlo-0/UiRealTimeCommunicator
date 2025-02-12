using Microsoft.AspNetCore.SignalR;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.HubMap.Interface
{
    internal interface IConnectionInvokeService
    {
        Task OnConnectedAsync(string hubName, IUiRtcProxyContext Context);
        Task OnDisconnectedAsync(string hubName, IUiRtcProxyContext Context, Exception? exception);
    }
}
