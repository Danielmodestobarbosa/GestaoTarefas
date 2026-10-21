

namespace Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;

public sealed class AtivarUsuarioCommand
{
    public Guid UsuarioId { get; }
    public AtivarUsuarioCommand(Guid usuarioId)
    {
        UsuarioId = usuarioId;
    }
}

