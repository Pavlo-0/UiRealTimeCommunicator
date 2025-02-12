using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.SimpleEmpty
{
    public class SimpleEmptyHub: IUiRtcHub
    {
    }

    public interface SimpleEmptySender : IUiRtcSenderContract<SimpleEmptyHub>
    {
        Task SimpleEmptyAnswer();
    }

    public class SimpleEmptyHandler(IUiRtcSenderService senderService) : IUiRtcHandler<SimpleEmptyHub>
    {
        public async Task ConsumeAsync()
        {
            await senderService.Send<SimpleEmptySender>().SimpleEmptyAnswer();
        }
    }
}
