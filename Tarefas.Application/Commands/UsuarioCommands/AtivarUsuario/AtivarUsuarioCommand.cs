

using MediatR;
using Tarefas.Domain.Tarefas.Entities;

namespace Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;

public sealed class AtivarUsuarioCommand : IRequest<AtivarUsuarioResultDto>
{
    public Guid UsuarioId { get; }
    public AtivarUsuarioCommand(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("UsuarioId inválido", nameof(usuarioId));

        UsuarioId = usuarioId;
    }
}

