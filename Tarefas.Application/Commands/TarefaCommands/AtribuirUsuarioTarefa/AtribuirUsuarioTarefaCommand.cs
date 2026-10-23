using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;

public sealed class AtribuirUsuarioTarefaCommand
{
    public Guid TarefaId { get; }
    public Guid UsuarioId { get; }

    public AtribuirUsuarioTarefaCommand(
        Guid tarefaId, 
        Guid usuarioId)
    {
        TarefaId = tarefaId;
        UsuarioId = usuarioId;
    }
}

