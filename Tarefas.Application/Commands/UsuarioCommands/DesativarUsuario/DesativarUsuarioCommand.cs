using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioCommand
{
    public Guid UsuarioId { get; }

    public DesativarUsuarioCommand(
        Guid usuarioId)
    {
        UsuarioId = usuarioId;
    }
}

