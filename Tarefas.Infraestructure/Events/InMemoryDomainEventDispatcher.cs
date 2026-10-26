
using Microsoft.Extensions.Logging;
using Tarefas.Domain.Common.Interfaces;

namespace Tarefas.Infrastructure.Event;

public sealed class InMemoryDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ILogger<InMemoryDomainEventDispatcher> _logger;

    public InMemoryDomainEventDispatcher(
        ILogger<InMemoryDomainEventDispatcher> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken ct = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            ct.ThrowIfCancellationRequested();

            _logger.LogInformation(
                "EVENTO DE DOMÍNIO DESPACHADO | Tipo: {Tipo} | Ocorrido em: {Timestamp}",
                domainEvent.GetType().Name,
                domainEvent.DateOccurred);
        }

        return Task.CompletedTask;
    }
}
