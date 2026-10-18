using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.CriarTarefa;

public sealed class CriarTarefaResultDto
{
    public Guid TarefaId { get; }
    public string Titulo { get; }
    public string Descricao { get; }

    public PrioridadeTarefa Prioridade { get; }

    public CriarTarefaResultDto(
        Guid tarefaId, 
        string titulo, 
        string descricao, 
        PrioridadeTarefa prioridade)
    {
        TarefaId = tarefaId;
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
    }
}

