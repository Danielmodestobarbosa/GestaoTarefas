

using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;

public sealed class AtualizarUsuarioResultDto
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }
    public StatusUsuario Status { get; }

    public AtualizarUsuarioResultDto(
        Guid usuarioId, 
        NomeUsuario nome, 
        Email email, 
        StatusUsuario status)
    {
        UsuarioId = usuarioId;
        Nome = nome;
        Email = email;
        Status = status;
    }
}

