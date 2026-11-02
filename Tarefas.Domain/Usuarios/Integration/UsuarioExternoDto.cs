using System.Text.Json.Serialization;

namespace Tarefas.Domain.Integration.Usuarios;

public sealed class UsuarioExternoDto
{
    public int Id { get; }

    [JsonPropertyName("name")]
    public string Nome { get; }

    public string Email { get; }

    public UsuarioExternoDto(int id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
    }
}