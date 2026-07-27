using MediatR;
using PagoDirecto.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PagoDirecto.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogRecorder _logger;

    public LoggingBehavior(ILogRecorder logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.Information($"Manejando comando: {requestName}");

        var response = await next();

        _logger.Information($"Comando manejado: {requestName}");

        return response;
    }
}
