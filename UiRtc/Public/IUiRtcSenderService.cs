//using UiRtc.Typing.PublicInterface;

using UiRtc.Typing.PublicInterface;

namespace UiRtc.Public
{
    public interface IUiRtcSenderService
    {
        TContract Send<TContract>() where TContract : IUiRtcSenderContract<IUiRtcHub>;
    }
}
