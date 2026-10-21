using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Abstractions.Persistence;

public interface ITarefaQueryRepository
{
    Task<ObterTarefaPorIdResultDto?> ObterPorIdAsync(
        Guid tarefaId,
        CancellationToken ct = default);

    Task<IReadOnlyList<TarefaResumoDto>> ListarAsync(
        CancellationToken ct = default);
}

