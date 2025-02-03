using App_backend.Communication.RandonNumberChannel.Models;
using UiRtc.Domain.Sender.Interface;
using UiRtc.Typing.PublicInterface;

namespace App_backend.Communication.RandonNumberChannel.Consumer
{
    public class GenerateNewRangeNumberHandler(ISenderService senderService) : IUiRtcHandler<RandomHub, RandomRangeRequestModel>
    {
        public Task ConsumeAsync(RandomRangeRequestModel rangeModel)
        {
            var _random = new Random();
            var model = new RandomValueResponseModel
            {
                Value = rangeModel.MinValue + _random.Next(rangeModel.MaxValue - rangeModel.MinValue),
            };

            senderService.Send<IRandomNumberContract>().SendRandomNumberMessage(model);

            return Task.CompletedTask;
        }
    }
}
