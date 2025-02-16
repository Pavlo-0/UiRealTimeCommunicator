using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.UnsubscribeSimple
{
    public class UnsubscribeSimpleHub: IUiRtcHub
    {
    }

    public interface UnsubscribeSimpleSender : IUiRtcSenderContract<UnsubscribeSimpleHub>
    {
        Task UnsubscribeSimpleAnswer();
    }

    public class UnsubscribeSimpleHandler(IUiRtcSenderService senderService) : IUiRtcHandler<UnsubscribeSimpleHub>
    {
        public async Task ConsumeAsync()
        {
            await senderService.Send<UnsubscribeSimpleSender>().UnsubscribeSimpleAnswer();
        }
    }
}
