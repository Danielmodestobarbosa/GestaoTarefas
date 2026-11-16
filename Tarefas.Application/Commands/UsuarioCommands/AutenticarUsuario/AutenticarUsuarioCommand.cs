
using MediatR;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioCommand
    : IRequest<AutenticarUsuarioResultDto>
{
    public Email Email { get; }
    public string Senha { get; }

    public AutenticarUsuarioCommand(
        Email email,
        string senha)
    {
        Email = email;
        Senha = senha;
    }
}
