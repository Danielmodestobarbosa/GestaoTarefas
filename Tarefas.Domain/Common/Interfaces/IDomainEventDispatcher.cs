using Tarefas.Domain.Common.Interfaces;

namespace Tarefas.Domain.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents,
                       CancellationToken ct = default);
}
