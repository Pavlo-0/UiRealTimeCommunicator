using App_backend.Communication.RandomNumber;
using App_backend.Communication.RandomNumber.Models;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.RandomNumberChannel;


public interface IRandomNumberSenderContract : IUiRtcSenderContract<RandomHub>
{
    [UiRtcMethod("RandomNumber")]
    Task SendRandomNumber(RandomValueResponseModel model);
}
