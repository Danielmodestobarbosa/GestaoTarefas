using MediatR;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarios;

public sealed record ObterUsuariosQuery
    : IRequest<IReadOnlyList<ObterUsuariosResultDto>>;