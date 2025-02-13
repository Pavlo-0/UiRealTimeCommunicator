using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace BE01.IntegrationTest.Scenarios.TwoHandlers
{
    [UiRtcHub("TwoHandlers")]
    public class TwoHandlersHub :IUiRtcHub
    {
    }

    public interface TwoHandlersSender: IUiRtcSenderContract<TwoHandlersHub> {
        Task HandlerAnswer(TwoHandlersResponse model);
    }

    [UiRtcMethod("TwoHandler")]
    public class TwoHandlers1Handler(IUiRtcSenderService sender) : IUiRtcHandler<TwoHandlersHub>
    {
        public async Task ConsumeAsync()
        {
            await sender.Send<TwoHandlersSender>().HandlerAnswer(new TwoHandlersResponse() { HandlerNumber = 1 });
        }
    }

    [UiRtcMethod("TwoHandler")]
    public class TwoHandlers2Handler(IUiRtcSenderService sender) : IUiRtcHandler<TwoHandlersHub>
    {
        public async Task ConsumeAsync()
        {
            await sender.Send<TwoHandlersSender>().HandlerAnswer(new TwoHandlersResponse() { HandlerNumber = 2 });
        }
    }

    [TranspilationSource]
    public class TwoHandlersResponse
    {
        public int HandlerNumber { get; set; }
    }
}
