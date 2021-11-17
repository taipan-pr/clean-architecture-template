using FluentValidation;
using System;
using System.Globalization;

namespace Clean.Architecture.Template.Application.WeatherForecast.Queries.WeatherForecastQuery
{
    internal class WeatherForecastQueryValidator : AbstractValidator<WeatherForecastQuery>
    {
        public WeatherForecastQueryValidator()
        {
            RuleFor(e => e.Date)
                .Must(e => DateTime.TryParseExact(e, "yyyy-MM-dd", null, DateTimeStyles.None, out _))
                .WithMessage("Invalid date format, use yyyy-MM-dd format");

            RuleFor(e => e.Days)
                .GreaterThan(0);
        }
    }
}
