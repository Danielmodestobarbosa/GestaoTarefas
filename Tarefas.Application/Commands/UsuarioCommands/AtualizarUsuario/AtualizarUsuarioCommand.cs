using MediatR;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;

public sealed class AtualizarUsuarioCommand : IRequest<AtualizarUsuarioResultDto>
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }

    public AtualizarUsuarioCommand(
        Guid usuarioId, 
        NomeUsuario nome, 
        Email email)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("UsuarioId inválido", nameof(usuarioId));

        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
    }
}

