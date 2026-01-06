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
        Task SimpleContextAnswer(SimpleContextResponseMessage message);
    }

    public class SimpleContextHandler(IUiRtcSenderService senderService) : IUiRtcContextHandler<SimpleContextHub, SimpleContextRequestMessage>
    {
        public async Task ConsumeAsync(SimpleContextRequestMessage Model, IUiRtcProxyContext context)
        {
            await senderService.Send<SimpleContextSender>()
                .SimpleContextAnswer(new SimpleContextResponseMessage
                {
                    CorrelationId = Model.CorrelationId,
                    ConnectionId = context.ConnectionId
                });
        }
    }

    [TranspilationSource]
    public class SimpleContextRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class SimpleContextResponseMessage
    {
        public required string CorrelationId { get; set; }
        public required string ConnectionId { get; set; }
    }
}
