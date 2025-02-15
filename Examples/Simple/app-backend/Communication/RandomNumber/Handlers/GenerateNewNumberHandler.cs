using App_backend.Communication.RandomNumber.Models;
using App_backend.Communication.RandomNumberChannel;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.RandomNumber.Handlers
{
    [UiRtcMethod("RequestNewNumber")]
    public class GenerateNewNumberHandler(IUiRtcSenderService senderService) : IUiRtcHandler<RandomHub>
    {
        public Task ConsumeAsync()
        {
            var _random = new Random();
            var model = new RandomValueResponseModel
            {
                Value = _random.Next(100)
            };

            senderService.Send<IRandomNumberSenderContract>().SendRandomNumber(model);

            return Task.CompletedTask;
        }
    }

}
