using MediatR;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;

public sealed class ConcluirTarefaCommandHandler 
    : IRequestHandler<ConcluirTarefaCommand, ConcluirTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConcluirTarefaCommandHandler(
        ITarefaRepository tarefaRepository, 
        IUnitOfWork unitOfWork)
    {
        this._tarefaRepository = tarefaRepository;
        this._unitOfWork = unitOfWork;
    }

    public async Task<ConcluirTarefaResultDto> Handle(
        ConcluirTarefaCommand request, 
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            request.TarefaId,
            cancellationToken);

        if(tarefa is null)
        {
            throw new InvalidOperationException(
                "Tarefa não encontrada");
        }

        tarefa.Concluir();

        await _unitOfWork.CommitAsync(cancellationToken);

        return new ConcluirTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade,
            tarefa.DataConclusao);
            
    }

}

