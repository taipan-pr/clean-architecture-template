using MediatR;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Clean.Architecture.Template.Application.PipelineBehaviors
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger _logger;

        public LoggingBehaviour(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stopwatch = Stopwatch.StartNew();
            LogContext.PushProperty("Request", request, true);
            try
            {
                _logger.Information("Start handling {Type}", typeof(TRequest).Name);
                return await next();
            }
            finally
            {
                stopwatch.Stop();
                _logger.Information("Completed handling {Type} - {Elapsed}", typeof(TRequest).Name, stopwatch.Elapsed);
            }
        }
    }
}
