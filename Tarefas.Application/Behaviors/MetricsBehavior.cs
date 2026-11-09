
using MediatR;
using System.Diagnostics;

namespace Tarefas.Application.Behaviors;

public sealed class MetricsBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ApplicationMetrics _metrics;

    public MetricsBehavior(ApplicationMetrics metrics)
    {
        _metrics = metrics;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var operation = typeof(TRequest).Name;

        var started = Stopwatch.GetTimestamp();

        var succeeded = false;

        try
        {
            var response = await next();

            succeeded = true;

            return response;
        }
        finally
        {
            var elapsed = Stopwatch.GetElapsedTime(started);

            _metrics.RecordRequest(
                operation,
                elapsed.TotalMilliseconds,
                succeeded);
        }
    }
}
