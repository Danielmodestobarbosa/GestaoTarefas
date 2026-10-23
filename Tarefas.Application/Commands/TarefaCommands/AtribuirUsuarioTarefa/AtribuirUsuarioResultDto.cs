using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;

public sealed class AtribuirUsuarioResultDto
{
    public Guid TarefaId { get; }
    public Guid UsuarioId { get; }
    public DateTime? DataAtualizacao { get; }

    public AtribuirUsuarioResultDto(
        Guid tarefaId, 
        Guid usuarioId, 
        DateTime? dataAtualizacao)
    {
        TarefaId = tarefaId;
        UsuarioId = usuarioId;
        DataAtualizacao = dataAtualizacao;
    }

}

