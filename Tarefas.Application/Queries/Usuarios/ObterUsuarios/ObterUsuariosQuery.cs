using MediatR;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarios;

public sealed record ObterUsuariosQuery(
    int PageNumber = 1, 
    int PageSize = 10)
    : IRequest<PagedResult<ObterUsuariosResultDto>>;