using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarios;

public sealed class ObterUsuariosQueryHandler
    : IRequestHandler<ObterUsuariosQuery, PagedResult<ObterUsuariosResultDto>>
{
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;

    public ObterUsuariosQueryHandler(
        IUsuarioQueryRepository usuarioQueryRepository)
    {
        _usuarioQueryRepository = usuarioQueryRepository;
    }

    public async Task<PagedResult<ObterUsuariosResultDto>> Handle(
        ObterUsuariosQuery request,
        CancellationToken cancellationToken)
    {
        return await _usuarioQueryRepository.ListarAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}