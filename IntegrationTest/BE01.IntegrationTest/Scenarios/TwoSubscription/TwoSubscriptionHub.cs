using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;

namespace BE01.IntegrationTest.Scenarios.TwoSubscription
{
    public class TwoSubscriptionHub: IUiRtcHub
    {
    }

    public interface TwoSubscriptionSender : IUiRtcSenderContract<TwoSubscriptionHub>
    {
        Task TwoSubscriptionAnswer(TwoSubscriptionResponseMessage message);
    }

    public class TwoSubscriptionHandler(IUiRtcSenderService senderService) : IUiRtcHandler<TwoSubscriptionHub, TwoSubscriptionRequestMessage>
    {
        public async Task ConsumeAsync(TwoSubscriptionRequestMessage Model)
        {
            await senderService.Send<TwoSubscriptionSender>().TwoSubscriptionAnswer(new TwoSubscriptionResponseMessage { CorrelationId = Model.CorrelationId });
        }
    }

    [TranspilationSource]
    public class TwoSubscriptionRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class TwoSubscriptionResponseMessage
    {
        public required string CorrelationId { get; set; }
    }
}
