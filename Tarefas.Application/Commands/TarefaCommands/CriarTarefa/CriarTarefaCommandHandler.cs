
using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Tarefas.Entities;

namespace Tarefas.Application.Commands.TarefaCommands.CriarTarefa;

public sealed class CriarTarefaCommandHandler
    : IRequestHandler<CriarTarefaCommand, CriarTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;

    public CriarTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<CriarTarefaResultDto> Handle(
        CriarTarefaCommand request,
        CancellationToken cancellationToken)
    {
        var tarefa = new Tarefa(
            request.Titulo,
            request.Descricao,
            request.Prioridade);

        await _tarefaRepository.AdicionarAsync(
            tarefa,
            cancellationToken);

        return new CriarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Prioridade);
    }
}
