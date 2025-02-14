using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.SimpleEmptyContext
{
    public class SimpleEmptyContextHub : IUiRtcHub
    {
    }

    public interface SimpleEmptyContextSender : IUiRtcSenderContract<SimpleEmptyContextHub>
    {
        Task SimpleEmptyContextAnswer();
    }

    public class SimpleEmptyContextHandler(IUiRtcSenderService senderService) : IUiRtcContextHandler<SimpleEmptyContextHub>
    {
        public async Task ConsumeAsync(IUiRtcProxyContext context)
        {
            await senderService.Send<SimpleEmptyContextSender>()
                .SimpleEmptyContextAnswer();
        }
    }  
}
