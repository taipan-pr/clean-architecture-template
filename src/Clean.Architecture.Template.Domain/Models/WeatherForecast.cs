using Clean.Architecture.Template.Domain.Enums;
using System;

namespace Clean.Architecture.Template.Domain.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public WeatherSummary Summary { get; set; }
    }
}
