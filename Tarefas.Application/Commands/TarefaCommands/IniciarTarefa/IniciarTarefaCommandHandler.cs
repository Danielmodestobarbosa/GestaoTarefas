using MediatR;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommandHandler
    : IRequestHandler<IniciarTarefaCommand, IniciarTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IniciarTarefaCommandHandler(
        ITarefaRepository tarefaRepository, 
        IUnitOfWork unitOfWork)
    {
        _tarefaRepository = tarefaRepository;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.CommitAsync(cancellationToken);

        return new IniciarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade);
    }
}
