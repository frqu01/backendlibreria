using MediatR;
using PagoDirecto.Application.Interfaces;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogRecorder _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogRecorder logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            _logger.Warning($"Performance en {requestName}: {elapsedMilliseconds} milisegundos");
        }

        return response;
    }
}
