
namespace Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioResultDto
{
    public Guid UsuarioId { get; }
    public string Nome { get; }
    public string Email { get; }
    public string AccessToken { get; }

    public AutenticarUsuarioResultDto(
        Guid usuarioId,
        string nome,
        string email,
        string accessToken)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
        AccessToken = accessToken;
    }
}
