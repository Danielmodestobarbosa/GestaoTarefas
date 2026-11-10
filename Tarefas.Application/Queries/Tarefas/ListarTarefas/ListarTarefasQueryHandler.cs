using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ListarTarefas;

public sealed class ListarTarefasQueryHandler
    : IRequestHandler<
        ListarTarefasQuery,
        PagedResult<TarefaResumoDto>>
{
    private readonly ITarefaQueryRepository _queryRepo;

    public ListarTarefasQueryHandler(
        ITarefaQueryRepository queryRepo)
    {
        _queryRepo = queryRepo;
    }

    public Task<PagedResult<TarefaResumoDto>> Handle(
        ListarTarefasQuery request,
        CancellationToken cancellationToken)
    {
        return _queryRepo.ListarAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
