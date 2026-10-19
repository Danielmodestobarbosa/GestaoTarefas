using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;

public sealed class ConcluirTarefaCommand
{

    public Guid TarefaId { get; }

    public ConcluirTarefaCommand(Guid tarefaId)
    {
        TarefaId = tarefaId;
    }
}

