using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Abstractions.Persistence;

public interface ITarefaQueryRepository
{
    Task<ObterTarefaPorIdResultDto?> ObterPorIdAsync(
        Guid tarefaId,
        CancellationToken ct = default);

    Task<PagedResult<TarefaResumoDto>> ListarAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);
}

