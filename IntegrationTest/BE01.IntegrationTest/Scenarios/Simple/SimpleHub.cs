using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.Simple
{
    public class SimpleHub: IUiRtcHub
    {
    }

    public interface SimpleSender : IUiRtcSenderContract<SimpleHub>
    {
        Task SimpleAnswer(SimpleResponseMessage message);
    }

    public class SimpleHandler(IUiRtcSenderService senderService) : IUiRtcHandler<SimpleHub, SimpleRequestMessage>
    {
        public async Task ConsumeAsync(SimpleRequestMessage Model)
        {
            await senderService.Send<SimpleSender>().SimpleAnswer(new SimpleResponseMessage { CorrelationId = Model.CorrelationId });
        }
    }

    [TranspilationSource]
    public class SimpleRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class SimpleResponseMessage
    {
        public required string CorrelationId { get; set; }
    }
}
