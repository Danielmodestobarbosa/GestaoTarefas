using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;

public sealed class AtualizarUsuarioCommand
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }

    public AtualizarUsuarioCommand(
        Guid usuarioId, 
        NomeUsuario nome, 
        Email email)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
    }
}

