using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;

public sealed class AtivarUsuarioResultDto
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }
    public StatusUsuario Status { get; }

    public AtivarUsuarioResultDto(
        Guid usuarioId, 
        NomeUsuario nome, 
        Email email, StatusUsuario status)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
        Status = status;
    }
}

