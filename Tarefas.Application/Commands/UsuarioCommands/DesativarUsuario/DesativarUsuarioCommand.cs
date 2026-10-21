namespace Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioCommand
{
    public Guid UsuarioId { get; }

    public DesativarUsuarioCommand(
        Guid usuarioId)
    {
        UsuarioId = usuarioId;
    }
}

