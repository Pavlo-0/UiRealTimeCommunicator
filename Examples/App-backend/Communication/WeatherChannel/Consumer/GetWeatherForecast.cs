using App_backend.Communication.WeatherChannel.Models;
using UiRtc.Domain.Sender.Interface;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.WeatherChannel.Consumer
{
    [UiRtcMethod("GetWeatherForecast")]
    public class GetWeatherForecastHandler(ILogger<GetWeatherForecastHandler> logger, ISenderService senderService) : IUiRtcHandler<WeatherHub, WeatherForecastRequestModel>
    {
        private static readonly string[] Summaries = new[]
               {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public async Task ConsumeAsync(WeatherForecastRequestModel model)
        {
            logger.LogInformation($"Get Weather request for city: {model.City}");
            await senderService.Send<WeatherChannelContract>().SendWeatherForecastSender(new WeatherForecastResponseModel
            {
                WeatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            }
            );
        }
    }
}
