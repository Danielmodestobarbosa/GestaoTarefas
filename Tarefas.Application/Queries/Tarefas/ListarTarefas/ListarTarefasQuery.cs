
using MediatR;
using Tarefas.Application.Queries.Tarefas.DTOs;

namespace Tarefas.Application.Queries.Tarefas.ListarTarefas;

public sealed record ListarTarefasQuery
    : IRequest<IReadOnlyList<TarefaResumoDto>>;
