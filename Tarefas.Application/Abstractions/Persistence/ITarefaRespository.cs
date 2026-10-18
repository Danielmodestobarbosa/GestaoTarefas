using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Entities;

namespace Tarefas.Application.Abstractions.Persistence;

public interface ITarefaRepository
{
    Task<Tarefa?> ObterTarefaPorId(
        Guid tarefaId, 
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Tarefa tarefa, 
        CancellationToken cancellationToken = default);
}

