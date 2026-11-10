
using MediatR;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ListarTarefas;

public sealed record ListarTarefasQuery(
    int PageNumber = 1, 
    int PageSize = 10
    ) : IRequest<PagedResult<TarefaResumoDto>>;
