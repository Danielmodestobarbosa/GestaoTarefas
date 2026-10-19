using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;

public sealed class CancelarTarefaCommand
{
    public Guid TarefaId { get; }

    public CancelarTarefaCommand(Guid tarefaId)
    {
        TarefaId = tarefaId;
    }
}

