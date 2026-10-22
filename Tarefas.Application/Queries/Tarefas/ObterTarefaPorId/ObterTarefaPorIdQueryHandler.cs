using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;
    
public sealed class ObterTarefaPorIdQueryHandler
{
    private readonly ITarefaQueryRepository _queryRepo;

    public ObterTarefaPorIdQueryHandler(
        ITarefaQueryRepository queryRepo) => _queryRepo = queryRepo;

    public Task<ObterTarefaPorIdResultDto?> HandleAsync(
        ObterTarefaPorIdQuery query,
        CancellationToken ct = default)
    {
        return _queryRepo.ObterPorIdAsync(query.TarefaId, ct);
    }
}

