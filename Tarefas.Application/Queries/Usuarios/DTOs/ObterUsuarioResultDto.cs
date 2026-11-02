namespace Tarefas.Application.Queries.Usuarios.DTOs;

public sealed record ObterUsuariosResultDto(
    Guid Id,
    string Nome,
    string Email,
    string Status);