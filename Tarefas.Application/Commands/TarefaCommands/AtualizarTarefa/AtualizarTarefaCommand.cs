using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;

public sealed class AtualizarTarefaCommand : IRequest<AtualizarTarefaResultDto>
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
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId inválida", nameof(tarefaId));

        TarefaId = tarefaId;
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
    }
}

