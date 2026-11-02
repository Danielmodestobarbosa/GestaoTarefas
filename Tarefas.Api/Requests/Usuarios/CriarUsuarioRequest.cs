namespace Tarefas.Api.Requests.Usuarios;

public sealed record CriarUsuarioRequest(
    string Nome,
    string Email);