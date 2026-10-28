
using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ListarTarefas;

public sealed class ListarTarefasQueryHandler
    : IRequestHandler<ListarTarefasQuery, IReadOnlyList<TarefaResumoDto>>
{
    private readonly ITarefaQueryRepository _queryRepo;

    public ListarTarefasQueryHandler(
        ITarefaQueryRepository queryRepo)
    {
        _queryRepo = queryRepo;
    }

    public Task<IReadOnlyList<TarefaResumoDto>> Handle(
        ListarTarefasQuery request,
        CancellationToken cancellationToken)
    {
        return _queryRepo.ListarAsync(cancellationToken);
    }
}
