using MediatR;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarioExterno;

public sealed record ObterUsuarioExternoQuery(int UsuarioId)
    : IRequest<ObterUsuarioExternoResultDto?>;