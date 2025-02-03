using App_backend.Communication.RandonNumberChannel.Models;
using UiRtc.Typing.PublicInterface;

namespace App_backend.Communication.RandonNumberChannel
{

    public interface IRandomNumberContract : IUiRtcSenderContract<RandomHub>
    {
        Task SendRandomNumberMessage(RandomValueResponseModel model);
    }
}
