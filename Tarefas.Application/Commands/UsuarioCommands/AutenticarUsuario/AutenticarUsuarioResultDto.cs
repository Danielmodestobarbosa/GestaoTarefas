using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioResultDto
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }
    public string AccessToken { get; }

    public AutenticarUsuarioResultDto(
        Guid usuarioId,
        NomeUsuario nome,
        Email email,
        string accessToken)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
        AccessToken = accessToken;
    }
}