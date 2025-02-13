//using UiRtc.Typing.PublicInterface;

using UiRtc.Typing.PublicInterface;

namespace UiRtc.Public
{
    public interface IUiRtcSenderService
    {
        TContract Send<TContract>() where TContract : IUiRtcSenderContract<IUiRtcHub>;
        TContract Send<TContract>(params string[] userIds) where TContract : IUiRtcSenderContract<IUiRtcHub>;
    }
}
