using App_backend.Communication.RandomNumber.Models;
using App_backend.Communication.RandomNumberChannel;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.RandomNumber.Handlers
{
    [UiRtcMethod("RequestNewRangeNumber")]
    public class GenerateNewRangeNumberHandler(ISenderService senderService) : IUiRtcHandler<RandomHub, RandomRangeRequestModel>
    {
        public Task ConsumeAsync(RandomRangeRequestModel rangeModel)
        {
            var _random = new Random();
            var model = new RandomValueResponseModel
            {
                Value = rangeModel.MinValue + _random.Next(rangeModel.MaxValue - rangeModel.MinValue),
            };

            senderService.Send<IRandomNumberSenderContract>().SendRandomNumber(model);

            return Task.CompletedTask;
        }
    }
}
