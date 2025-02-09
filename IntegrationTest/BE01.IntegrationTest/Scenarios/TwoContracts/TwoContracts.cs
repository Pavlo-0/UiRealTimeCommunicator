using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace BE01.IntegrationTest.Scenarios.TwoHubsTwoContracts
{
    public class TwoContractsHub: IUiRtcHub
    {
    }

    public interface TwoContractsSender : IUiRtcSenderContract<TwoContractsHub>
    {
        Task TwoContractsAnswer1(TwoContractsResponseMessage message);
        Task TwoContractsAnswer2(TwoContractsResponseMessage message);
    }

    [UiRtcMethod("TwoContracts")]
    public class TwoContractsHandler(IUiRtcSenderService senderService) : IUiRtcHandler<TwoContractsHub, TwoContractsRequestMessage>
    {
        public async Task ConsumeAsync(TwoContractsRequestMessage Model)
        {
            await senderService.Send<TwoContractsSender>().TwoContractsAnswer1(new TwoContractsResponseMessage { CorrelationId = Model.CorrelationId });
            await senderService.Send<TwoContractsSender>().TwoContractsAnswer2(new TwoContractsResponseMessage { CorrelationId = Model.CorrelationId });
        }
    }

    [TranspilationSource]
    public class TwoContractsRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class TwoContractsResponseMessage
    {
        public required string CorrelationId { get; set; }
    }
}
