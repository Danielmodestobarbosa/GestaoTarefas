
using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Abstractions.Security;
using Tarefas.Domain.Usuarios.Enums;

namespace Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioCommandHandler
    : IRequestHandler<AutenticarUsuarioCommand, AutenticarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AutenticarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AutenticarUsuarioResultDto> Handle(
        AutenticarUsuarioCommand request,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(
            request.Email.Endereco,
            cancellationToken);

        if (usuario is null)
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        if (usuario.Status != StatusUsuario.Ativo)
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        var senhaValida = _passwordHasher.VerifyPassword(
            usuario.PasswordHash,
            request.Senha);

        if (!senhaValida)
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        return new AutenticarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email);
    }
}
