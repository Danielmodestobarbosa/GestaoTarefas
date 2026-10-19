using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;

public sealed class ConcluirTarefaResultDto
{
    public Guid TarefaId { get; }
    public string Titulo { get; }
    public StatusTarefa Status { get; }
    public PrioridadeTarefa Prioridade { get; }
    public DateTime? DataConclusao { get; }

    public ConcluirTarefaResultDto(
        Guid tarefaId, 
        string titulo, 
        StatusTarefa status, 
        PrioridadeTarefa prioridade, 
        DateTime? dataConclusao)
    {
        TarefaId = tarefaId;
        Titulo = titulo;
        Status = status;
        Prioridade = prioridade;
        DataConclusao = dataConclusao;
    }

}

