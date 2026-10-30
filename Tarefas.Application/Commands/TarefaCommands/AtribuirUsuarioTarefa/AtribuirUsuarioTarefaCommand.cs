using MediatR;

namespace Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;

public sealed class AtribuirUsuarioTarefaCommand
    : IRequest<AtribuirUsuarioResultDto>
{
    public Guid TarefaId { get; }
    public Guid UsuarioId { get; }

    public AtribuirUsuarioTarefaCommand(
        Guid tarefaId,
        Guid usuarioId)
    {
        if (tarefaId == Guid.Empty)
            throw new ArgumentException("TarefaId inválida", nameof(tarefaId));

        if (usuarioId == Guid.Empty)
            throw new ArgumentException("UsuarioId inválido", nameof(usuarioId));

        TarefaId = tarefaId;
        UsuarioId = usuarioId;
    }
}