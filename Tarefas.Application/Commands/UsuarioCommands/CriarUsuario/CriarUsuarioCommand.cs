using MediatR;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;

public sealed class CriarUsuarioCommand : IRequest<CriarUsuarioResultDto>
{
    public NomeUsuario Nome { get; }
    public Email Email { get; }

    public CriarUsuarioCommand(NomeUsuario nome, Email email)
    {
        Nome = nome;
        Email = email;
    }
}

