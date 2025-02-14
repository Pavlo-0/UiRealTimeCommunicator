namespace UiRtc.Typing.PublicInterface
{
    public interface IUiRtcConnection<THub> where THub : IUiRtcHub
    {
        Task OnConnectedAsync(IUiRtcProxyContext context);
        Task OnDisconnectedAsync(IUiRtcProxyContext context, Exception? exception);
    }
}
