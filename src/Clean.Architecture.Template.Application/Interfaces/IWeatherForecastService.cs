using System;
using System.Threading.Tasks;

namespace Clean.Architecture.Template.Application.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<Domain.Models.WeatherForecast> GetWeatherForecastAsync(DateTime dateTime);
    }
}
