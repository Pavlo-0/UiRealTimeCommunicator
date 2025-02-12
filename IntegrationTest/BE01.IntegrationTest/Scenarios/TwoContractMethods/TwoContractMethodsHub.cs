using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace BE01.IntegrationTest.Scenarios.TwoContractMethods
{
    public class TwoContractMethodsHub : IUiRtcHub
    {
    }

    public interface TwoContractMethodsSender : IUiRtcSenderContract<TwoContractMethodsHub>
    {
        Task TwoContractMethodsAnswer1(TwoContractMethodsResponseMessage message);
        Task TwoContractMethodsAnswer2(TwoContractMethodsResponseMessage message);
    }

    [UiRtcMethod("TwoContractMethods")]
    public class TwoContractMethodsHandler(IUiRtcSenderService senderService) : IUiRtcHandler<TwoContractMethodsHub, TwoContractMethodsRequestMessage>
    {
        public async Task ConsumeAsync(TwoContractMethodsRequestMessage Model)
        {
            await senderService.Send<TwoContractMethodsSender>().TwoContractMethodsAnswer1(new TwoContractMethodsResponseMessage { CorrelationId = Model.CorrelationId });
            await senderService.Send<TwoContractMethodsSender>().TwoContractMethodsAnswer2(new TwoContractMethodsResponseMessage { CorrelationId = Model.CorrelationId });
        }
    }

    [TranspilationSource]
    public class TwoContractMethodsRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class TwoContractMethodsResponseMessage
    {
        public required string CorrelationId { get; set; }
    }
}
