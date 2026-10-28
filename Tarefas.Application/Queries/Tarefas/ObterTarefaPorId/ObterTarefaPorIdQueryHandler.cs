
using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

public sealed class ObterTarefaPorIdQueryHandler
    : IRequestHandler<ObterTarefaPorIdQuery, ObterTarefaPorIdResultDto?>
{
    private readonly ITarefaQueryRepository _queryRepo;

    public ObterTarefaPorIdQueryHandler(
        ITarefaQueryRepository queryRepo)
    {
        _queryRepo = queryRepo;
    }

    public Task<ObterTarefaPorIdResultDto?> Handle(
        ObterTarefaPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return _queryRepo.ObterPorIdAsync(
            request.TarefaId,
            cancellationToken);
    }
}
