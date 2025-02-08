using Tapper;

namespace App_backend.Communication.WeatherChannel.Models
{
    [TranspilationSource]
    public class WeatherForecastModel
    {
        public required IEnumerable<WeatherForecastDetail> WeatherForecast { get; set; }
        public required string City { get; set; }
    }
}
