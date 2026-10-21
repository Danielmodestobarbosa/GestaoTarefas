using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;
    
public sealed class ObterTarefaPorIdQueryHandler
{
    private readonly ITarefaQueryRepository _queryRepo;

    public ObterTarefaPorIdQueryHandler(
        ITarefaQueryRepository queryRepo) => _queryRepo = queryRepo;

    public async Task<ObterTarefaPorIdResultDto?> HandleAsync(
        ObterTarefaPorIdQuery query,
        CancellationToken ct = default) => await _queryRepo.ObterPorIdAsync(query.TarefaId, ct); 
}

