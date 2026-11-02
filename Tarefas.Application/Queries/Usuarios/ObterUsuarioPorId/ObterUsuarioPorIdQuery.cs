using MediatR;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarioPorId;

public sealed record ObterUsuarioPorIdQuery(Guid UsuarioId)
    : IRequest<ObterUsuarioPorIdResultDto?>;