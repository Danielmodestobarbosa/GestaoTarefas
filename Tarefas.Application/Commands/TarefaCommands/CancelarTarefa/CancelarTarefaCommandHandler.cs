using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Exceptions;

namespace Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;

public sealed class CancelarTarefaCommandHandler : 
    IRequestHandler<CancelarTarefaCommand, CancelarTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelarTarefaCommandHandler(
        ITarefaRepository tarefaRepository, 
        IUnitOfWork unitOfWork)
    {
        _tarefaRepository = tarefaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelarTarefaResultDto> Handle (
        CancelarTarefaCommand request, 
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            request.TarefaId,
            cancellationToken);

        if(tarefa is null)
        {
            throw new NotFoundException(
                "Tarefa não encontrada");
        }

        tarefa.Cancelar();

        await _unitOfWork.CommitAsync(cancellationToken);

        return new CancelarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade);
    }
}

