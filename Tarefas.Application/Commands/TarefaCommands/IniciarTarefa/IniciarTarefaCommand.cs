

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommand
{
    public Guid TarefaId { get; }

    public IniciarTarefaCommand(Guid tarefaId)
    {
        TarefaId = tarefaId;
    }

}

