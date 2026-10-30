using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;

public sealed class CancelarTarefaCommand : IRequest<CancelarTarefaResultDto>
{
    public Guid TarefaId { get; }

    public CancelarTarefaCommand(Guid tarefaId)
    {
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId inválida", nameof(tarefaId));

        TarefaId = tarefaId;
    }
}

