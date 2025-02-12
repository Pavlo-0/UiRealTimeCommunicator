using Microsoft.AspNetCore.SignalR;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.OnConnection
{
    public class OnConnectionHub: IUiRtcHub
    {
    }

    public interface OnConnectionSender : IUiRtcSenderContract<OnConnectionHub>
    {
        Task OnConnectionInit();
        Task OnConnectionAnswer();
        Task OnDisconnectedAnswer();
    }

    public class OnConnectionStart(IUiRtcSenderService senderService) : IUiRtcConnection<OnConnectionHub>
    {
        public async Task OnConnectedAsync(IUiRtcProxyContext context)
        {
            await senderService.Send<OnConnectionSender>().OnConnectionAnswer();
        }

        public async Task OnDisconnectedAsync(IUiRtcProxyContext context, Exception? exception)
        {
            await senderService.Send<OnConnectionSender>().OnDisconnectedAnswer();
        }
    }

    public class OnConnectionHandler(IUiRtcSenderService senderService) : IUiRtcHandler<OnConnectionHub>
    {
        public async Task ConsumeAsync()
        {
            await senderService.Send<OnConnectionSender>().OnConnectionInit();
        }
    }
}
