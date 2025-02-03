using App_backend.Communication.RandonNumberChannel.Models;
using UiRtc.Domain.Sender.Interface;
using UiRtc.Typing.PublicInterface;

namespace App_backend.Communication.RandonNumberChannel.Consumer
{
    public class GenerateNewNumberHandler(ISenderService senderService) : IUiRtcHandler<RandomHub>
    {
        public Task ConsumeAsync()
        {
            var _random = new Random();
            var model = new RandomValueResponseModel
            {
                Value = _random.Next(100)
            };

            senderService.Send<IRandomNumberContract>().SendRandomNumberMessage(model);

            return Task.CompletedTask;
        }
    }

}
