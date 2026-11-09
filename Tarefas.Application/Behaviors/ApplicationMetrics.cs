
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Tarefas.Application.Behaviors;

public sealed class ApplicationMetrics : IDisposable
{
    private readonly Meter _meter;

    private readonly Counter<long> _requestsCounter;

    private readonly Histogram<double> _durationHistogram;

    public ApplicationMetrics()
    {
        _meter = new Meter("Tarefas.Application", "1.0.0");

        _requestsCounter = _meter.CreateCounter<long>(
            "tarefas.requests.total",
            description: "Total de operações executadas.");

        _durationHistogram = _meter.CreateHistogram<double>(
            "tarefas.requests.duration",
            unit: "ms",
            description: "Duração das operações em milissegundos.");
    }

    public void RecordRequest(
        string operation,
        double elapsedMilliseconds,
        bool succeeded)
    {
        var tags = new TagList
        {
            { "operation", operation },
            { "outcome", succeeded ? "success" : "failure" }
        };

        _requestsCounter.Add(1, tags);

        _durationHistogram.Record(
            elapsedMilliseconds,
            tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
