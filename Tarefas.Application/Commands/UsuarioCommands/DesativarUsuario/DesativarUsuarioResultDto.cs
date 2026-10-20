using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Usuarios.Enums;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioResultDto
{
    public Guid UsuarioId { get; }
    public NomeUsuario Nome { get; }
    public Email Email { get; }
    public StatusUsuario Status { get; }

    public DesativarUsuarioResultDto(
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

