using Clean.Architecture.Template.Domain.Enums;
using System;

namespace Clean.Architecture.Template.Application.WeatherForecast.Queries.WeatherForecastQuery
{
    public class WeatherForecastQueryResult
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public WeatherSummary Summary { get; set; }
    }
}
