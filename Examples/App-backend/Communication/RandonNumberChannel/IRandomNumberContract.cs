using App_backend.Communication.RandonNumberChannel.Models;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.RandonNumberChannel
{
    [UiRtcHub("RandomHub")]
    public interface IRandomNumberContract : IUiRtcHub
    {
        Task SendRandomNumberMessage(RandomValueResponseModel model);
    }
}
