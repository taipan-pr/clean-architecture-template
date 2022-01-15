using Clean.Architecture.Template.Application.WeatherForecast.Queries.WeatherForecastQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace Clean.Architecture.Template.Controllers.V1_0
{
    [ApiController]
    [Route("{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public WeatherForecastController(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string date, [FromQuery] int days)
        {
            var results = await _mediator.Send(new WeatherForecastQuery
            {
                Date = date,
                Days = days
            });
            return Ok(results);
        }
    }
}
