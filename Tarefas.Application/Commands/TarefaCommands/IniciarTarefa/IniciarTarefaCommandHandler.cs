using MediatR;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommandHandler
    : IRequestHandler<IniciarTarefaCommand, IniciarTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;

    public IniciarTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<IniciarTarefaResultDto> Handle(
        IniciarTarefaCommand request,
        CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            request.TarefaId,
            cancellationToken);

        if (tarefa is null)
        {
            throw new InvalidOperationException("Tarefa não encontrada");
        }

        tarefa.Iniciar();

        await _tarefaRepository.AtualizarAsync(
            tarefa,
            cancellationToken);

        return new IniciarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade);
    }
}
