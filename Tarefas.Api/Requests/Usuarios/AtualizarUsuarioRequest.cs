namespace Tarefas.Api.Requests.Usuarios;

public sealed record AtualizarUsuarioRequest(
    string Nome,
    string Email);