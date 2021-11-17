using Clean.Architecture.Template.Application.Interfaces;
using Clean.Architecture.Template.Domain.Enums;
using Clean.Architecture.Template.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Clean.Architecture.Template.Infrastructure.Services
{
    internal class MemoryWeatherForecastService : IWeatherForecastService
    {
        public Task<WeatherForecast> GetWeatherForecastAsync(DateTime dateTime)
        {
            var rng = new Random();
            var tempC = rng.Next(-20, 55);
            return Task.FromResult(new WeatherForecast
            {
                Date = dateTime,
                Summary = (WeatherSummary)rng.Next(9),
                TemperatureC = tempC,
                TemperatureF = 32 + (int)(tempC / 0.5556)
            });
        }
    }
}
