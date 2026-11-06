using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Exceptions;

namespace Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;

public sealed class AtualizarTarefaCommandHandler 
    : IRequestHandler<AtualizarTarefaCommand, AtualizarTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarTarefaCommandHandler(
        ITarefaRepository tarefaRepository, 
        IUnitOfWork unitOfWork)
    {
        _tarefaRepository = tarefaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AtualizarTarefaResultDto> Handle(
        AtualizarTarefaCommand request, 
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

        tarefa.Atualizar(
            request.Titulo,
            request.Descricao,
            request.Prioridade);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new AtualizarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Prioridade);
    }
}

