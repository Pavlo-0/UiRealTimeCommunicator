using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.SimpleDateTimeField
{
    public class SimpleDateTimeHub : IUiRtcHub
    {
    }

    public interface SimpleDateTimeSender : IUiRtcSenderContract<SimpleDateTimeHub>
    {
        Task SimpleAnswer(SimpleDateTimeResponseMessage message);
    }

    public class SimpleDateTimeHandler(IUiRtcSenderService senderService) : IUiRtcHandler<SimpleDateTimeHub, SimpleDateTimeRequestMessage>
    {
        public async Task ConsumeAsync(SimpleDateTimeRequestMessage Model)
        {
            await senderService.Send<SimpleDateTimeSender>().SimpleAnswer(new SimpleDateTimeResponseMessage { CorrelationId = Model.CorrelationId, ParamDateTime = Model.ParamDateTime });
        }
    }

    [TranspilationSource]
    public class SimpleDateTimeRequestMessage
    {
        public required string CorrelationId { get; set; }
        public required DateTime ParamDateTime { get; set; }
    }

    [TranspilationSource]
    public class SimpleDateTimeResponseMessage
    {
        public required string CorrelationId { get; set; }

        public required DateTime ParamDateTime { get; set; }
    }
}
