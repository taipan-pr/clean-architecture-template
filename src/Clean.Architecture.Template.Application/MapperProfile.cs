using AutoMapper;
using Clean.Architecture.Template.Application.WeatherForecast.Queries.WeatherForecastQuery;

namespace Clean.Architecture.Template.Application
{
    internal class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Domain.Models.WeatherForecast, WeatherForecastQueryResult>();
        }
    }
}
