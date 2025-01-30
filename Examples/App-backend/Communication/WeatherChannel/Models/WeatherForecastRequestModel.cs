using Tapper;

namespace App_backend.Communication.WeatherChannel.Models
{
    [TranspilationSource]
    public class WeatherForecastRequestModel
    {
        public string City { get; set; }
    }
}
