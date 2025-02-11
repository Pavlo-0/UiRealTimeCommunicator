using Microsoft.AspNetCore.SignalR;

namespace UiRtc.Typing.PublicInterface
{
    public interface IUiRtcConnection<THub> where THub : IUiRtcHub
    {
        Task OnConnectedAsync(string ConnectinId, HubCallerContext Context);
        Task OnDisconnectedAsync(string ConnectinId, Exception? exception, HubCallerContext Context);
    }
}
