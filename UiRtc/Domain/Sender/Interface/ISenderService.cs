using UiRtc.Typing.PublicInterface;

namespace UiRtc.Domain.Sender.Interface
{
    public interface ISenderService
    {
        TContract Send<TContract>() where TContract : IUiRtcHub;
    }
}
