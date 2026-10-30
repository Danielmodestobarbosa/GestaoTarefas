namespace Tarefas.Domain.Integration.Usuarios;

// Representa os dados recebidos de um sistema externo.
// Não é uma entidade do domínio.
// Não possui regras de negócio.
// É apenas um objeto de transporte de dados.
public sealed class UsuarioExternoDto
{
    public int Id { get; }
    public string Nome { get; }
    public string Email { get; }

    public UsuarioExternoDto(int id, string nome, string email)
    {
        Id = id;
        Nome = nome;
        Email = email;
    }
}