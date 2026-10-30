using MediatR;

namespace Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioCommand : IRequest<DesativarUsuarioResultDto>
{
    public Guid UsuarioId { get; }

    public DesativarUsuarioCommand(
        Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("UsuarioId inválido", nameof(usuarioId));

        UsuarioId = usuarioId;
    }
}

