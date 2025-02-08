using App_backend.Communication.WeatherChannel.Models;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.WeatherChannel
{

    public interface WeatherChannelSenderContract: IUiRtcSenderContract<WeatherHub>
    {
        [UiRtcMethod("WeatherForecast")]
        Task SendWeatherForecast(WeatherForecastResponseModel forecast);
    }
}
