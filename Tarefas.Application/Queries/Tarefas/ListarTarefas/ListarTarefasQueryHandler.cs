using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ListarTarefas;

public sealed class ListarTarefasQueryHandler
{
    private readonly ITarefaQueryRepository _queryRepo;

    public ListarTarefasQueryHandler(
        ITarefaQueryRepository queryRepo) 
        => _queryRepo = queryRepo;

    public Task<IReadOnlyList<TarefaResumoDto>> HandleAsync(
        ListarTarefasQuery query,
        CancellationToken ct = default) 
        => _queryRepo.ListarAsync(ct);
}

