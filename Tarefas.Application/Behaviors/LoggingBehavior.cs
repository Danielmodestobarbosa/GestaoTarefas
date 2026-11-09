
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Tarefas.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var operation = typeof(TRequest).Name;
        var started = Stopwatch.GetTimestamp();

        _logger.LogInformation(
            "Iniciando operação {Operation}",
            operation);

        try
        {
            var response = await next();

            var elapsed = Stopwatch.GetElapsedTime(started);

            _logger.LogInformation(
                "Operação {Operation} concluída em {ElapsedMilliseconds} ms",
                operation,
                elapsed.TotalMilliseconds);

            return response;
        }
        catch (OperationCanceledException)
        {
            var elapsed = Stopwatch.GetElapsedTime(started);

            _logger.LogWarning(
                "Operação {Operation} cancelada após {ElapsedMilliseconds} ms",
                operation,
                elapsed.TotalMilliseconds);

            throw;
        }
        catch (Exception exception)
        {
            var elapsed = Stopwatch.GetElapsedTime(started);

            _logger.LogError(
                exception,
                "Erro na operação {Operation} após {ElapsedMilliseconds} ms",
                operation,
                elapsed.TotalMilliseconds);

            throw;
        }
    }
}
