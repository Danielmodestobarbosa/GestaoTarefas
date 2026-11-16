
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioResultDto
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }

    public AutenticarUsuarioResultDto(
        Guid usuarioId,
        NomeUsuario nome,
        Email email)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
    }
}
