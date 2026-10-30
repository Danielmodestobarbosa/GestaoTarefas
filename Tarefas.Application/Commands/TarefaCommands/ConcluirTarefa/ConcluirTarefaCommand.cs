using MediatR;

namespace Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;

public sealed class ConcluirTarefaCommand : IRequest<ConcluirTarefaResultDto>
{

    public Guid TarefaId { get; }

    public ConcluirTarefaCommand(Guid tarefaId)
    {
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId inválida", nameof(tarefaId));

        TarefaId = tarefaId;
    }
}

