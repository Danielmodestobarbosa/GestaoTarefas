namespace Tarefas.Application.Queries.Usuarios.DTOs;

public sealed class ObterUsuarioExternoResultDto
{
    public int UsuarioId { get; init; }
    public string Nome { get; init; }
    public string Email { get; init; }

    public ObterUsuarioExternoResultDto(
        int usuarioId,
        string nome,
        string email)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
    }
}