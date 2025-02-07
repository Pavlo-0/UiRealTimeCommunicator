using Tapper;

namespace App_backend.Communication.WeatherChannel.Models
{
    [TranspilationSource]
    public class WeatherForecastDetail
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}
