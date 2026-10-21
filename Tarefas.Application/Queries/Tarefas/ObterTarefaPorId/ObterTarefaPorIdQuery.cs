using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

public sealed class ObterTarefaPorIdQuery
{
    public Guid TarefaId { get; }

    public ObterTarefaPorIdQuery(Guid tarefaId)
    {
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId iválida", nameof(tarefaId));

        TarefaId = tarefaId;
    }
}

