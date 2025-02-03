using App_backend.Communication.WeatherChannel.Models;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace App_backend.Communication.WeatherChannel
{

    public interface WeatherChannelContract: IUiRtcSenderContract<WeatherHub>
    {
        [UiRtcMethod("SendWeatherForecast")]
        Task SendWeatherForecastSender(WeatherForecastResponseModel forecast);
    }
}
