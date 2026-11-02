using MediatR;

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommand
    : IRequest<IniciarTarefaResultDto>
{
    public Guid TarefaId { get; }

    public IniciarTarefaCommand(Guid tarefaId)
    {
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId inválida", nameof(tarefaId));

        TarefaId = tarefaId;
    }
}

