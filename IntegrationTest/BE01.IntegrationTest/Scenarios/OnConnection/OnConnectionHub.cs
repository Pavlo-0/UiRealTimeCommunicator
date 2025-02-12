using Microsoft.AspNetCore.SignalR;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.OnConnection
{
    public class OnConnectionHub: IUiRtcHub
    {
    }

    public interface OnConnectionDummyContract: IUiRtcSenderContract<OnConnectionHub>
    {
        Task DummyMethod();
    }

    public class OnConnectionStart(IUiRtcSenderService sender) : IUiRtcConnection<OnConnectionHub>
    {
        public async Task OnConnectedAsync(IUiRtcProxyContext context)
        {
            await sender.Send<OnConnectionManagerSender>().UpdateStatus(new OnConnectionStatusModel() { IsConnected = true });
        }

        public async Task OnDisconnectedAsync(IUiRtcProxyContext context, Exception? exception)
        {
            await sender.Send<OnConnectionManagerSender>().UpdateStatus(new OnConnectionStatusModel() { IsConnected = false });
        }
    }
}
