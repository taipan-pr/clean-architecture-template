using Clean.Architecture.Template.Application.Exceptions;
using Clean.Architecture.Template.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using IMapper = AutoMapper.IMapper;

namespace Clean.Architecture.Template.Application.WeatherForecast.Queries.WeatherForecastQuery
{
    public class WeatherForecastQuery : IRequest<List<WeatherForecastQueryResult>>
    {
        public string Date { get; set; }
        public int Days { get; set; }
    }

    internal class WeatherForecastQueryHandler : IRequestHandler<WeatherForecastQuery, List<WeatherForecastQueryResult>>
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly IMapper _mapper;

        public WeatherForecastQueryHandler(IWeatherForecastService weatherForecastService, IMapper mapper)
        {
            _weatherForecastService = weatherForecastService;
            _mapper = mapper;
        }

        public async Task<List<WeatherForecastQueryResult>> Handle(WeatherForecastQuery request, CancellationToken cancellationToken)
        {
            switch (request.Days)
            {
                case > 20:
                    throw new Exception("Way too many days");
                case > 10:
                    throw new OverLimitException("Too many days");
            }

            var results = new List<WeatherForecastQueryResult>();
            var datetime = DateTime.ParseExact(request.Date, "yyyy-MM-dd", null, DateTimeStyles.None);
            for (var i = 0; i < request.Days; i++)
            {
                var forecast = await _weatherForecastService.GetWeatherForecastAsync(datetime);
                results.Add(_mapper.Map<WeatherForecastQueryResult>(forecast));
                datetime = datetime.AddDays(1);
            }
            return results;
        }
    }
}
