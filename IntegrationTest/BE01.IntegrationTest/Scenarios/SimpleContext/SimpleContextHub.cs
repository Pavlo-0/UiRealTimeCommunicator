using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.SimpleContext
{
    public class SimpleContextHub : IUiRtcHub
    {
    }

    public interface SimpleContextSender : IUiRtcSenderContract<SimpleContextHub>
    {
        Task SimpleContextAnswer(SimpleContexResponseMessage message);
    }

    public class SimpleContextHandler(IUiRtcSenderService senderService) : IUiRtcContextHandler<SimpleContextHub, SimpleContexRequestMessage>
    {
        public async Task ConsumeAsync(SimpleContexRequestMessage Model, IUiRtcProxyContext context)
        {
            await senderService.Send<SimpleContextSender>()
                .SimpleContextAnswer(new SimpleContexResponseMessage
                {
                    CorrelationId = Model.CorrelationId,
                    ConnectionId = context.ConnectionId
                });
        }
    }

    [TranspilationSource]
    public class SimpleContexRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class SimpleContexResponseMessage
    {
        public required string CorrelationId { get; set; }
        public required string ConnectionId { get; set; }
    }
}
