//using UiRtc.Typing.PublicInterface;

using UiRtc.Typing.PublicInterface;

namespace UiRtc.Public
{
    public interface ISenderService
    {
        TContract Send<TContract>() where TContract : IUiRtcSenderContract<IUiRtcHub>;
    }
}
