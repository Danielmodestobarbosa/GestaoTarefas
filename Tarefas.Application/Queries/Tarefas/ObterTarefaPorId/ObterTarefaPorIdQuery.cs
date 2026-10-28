using MediatR;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

public sealed class ObterTarefaPorIdQuery
    : IRequest<ObterTarefaPorIdResultDto?>
{
    public Guid TarefaId { get; }

    public ObterTarefaPorIdQuery(Guid tarefaId)
    {
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId iválida", nameof(tarefaId));

        TarefaId = tarefaId;
    }
}
