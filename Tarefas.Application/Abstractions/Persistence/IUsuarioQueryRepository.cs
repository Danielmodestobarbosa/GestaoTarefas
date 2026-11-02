using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Abstractions.Persistence;

public interface IUsuarioQueryRepository
{
    Task<ObterUsuarioPorIdResultDto?> ObterPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ObterUsuariosResultDto>> ObterTodosAsync(
        CancellationToken cancellationToken = default);
}
