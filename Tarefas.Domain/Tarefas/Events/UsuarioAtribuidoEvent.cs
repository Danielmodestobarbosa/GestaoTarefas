using Tarefas.Domain.Common.Base;

namespace Tarefas.Domain.Tarefas.Events;

public sealed record UsuarioAtribuidoEvent(
    Guid TarefaId,
    Guid UsuarioId
    ) : DomainEventBase;


