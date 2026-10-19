using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaResultDto
{
    public Guid TarefaId { get; }
    public string Titulo { get; }
    public StatusTarefa Status { get; }

    public PrioridadeTarefa Prioridade { get; }

    public IniciarTarefaResultDto(
        Guid tarefaId, 
        string titulo, 
        StatusTarefa status, PrioridadeTarefa prioridade)
    {
        TarefaId = tarefaId;
        Titulo = titulo;
        Status = status;
        Prioridade = prioridade;
    }

    
}

