namespace Tarefas.Application.Queries.Usuarios.DTOs;

public sealed record ObterUsuarioPorIdResultDto(
    Guid Id,
    string Nome,
    string Email,
    string Status);