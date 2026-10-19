using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;

public sealed class AtualizarTarefaCommand
{
    public Guid TarefaId { get; }
    public string Titulo { get; }
    public string Descricao { get; }
    public PrioridadeTarefa Prioridade { get; }

    public AtualizarTarefaCommand(
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

