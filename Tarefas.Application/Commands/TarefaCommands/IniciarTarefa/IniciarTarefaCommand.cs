using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommand
{
    public Guid TarefaId { get; }

    public IniciarTarefaCommand(Guid tarefaId)
    {
        TarefaId = tarefaId;
    }

}

