using App_backend.Communication.WeatherChannel.Models;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.WeatherChannel
{

    [UiRtcHub("Weather")]
    public interface WeatherChannelContract: IUiRtcHub
    {
        Task SendWeatherForecast(WeatherForecastResponseModel forecast);
    }
}
