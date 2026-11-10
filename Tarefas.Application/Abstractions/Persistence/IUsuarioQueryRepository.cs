using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Abstractions.Persistence;

public interface IUsuarioQueryRepository
{
    Task<ObterUsuarioPorIdResultDto?> ObterPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default);

    Task<PagedResult<ObterUsuariosResultDto>> ListarAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
