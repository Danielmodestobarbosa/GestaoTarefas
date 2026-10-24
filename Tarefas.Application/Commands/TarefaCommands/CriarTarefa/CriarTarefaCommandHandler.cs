using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Tarefas.Entities;

namespace Tarefas.Application.Commands.TarefaCommands.CriarTarefa;

public sealed class CriarTarefaCommandHandler
{
    private readonly ITarefaRepository _tarefaRepository;

    public CriarTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<CriarTarefaResultDto> HandleAsync(
        CriarTarefaCommand command, 
        CancellationToken cancellationToken = default)
    {
        var tarefa = new Tarefa(
            command.Titulo,
            command.Descricao,
            command.Prioridade);

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

