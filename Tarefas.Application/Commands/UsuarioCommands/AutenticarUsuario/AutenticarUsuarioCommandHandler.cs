
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
    private readonly ITokenService _tokenService;

    public AutenticarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
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

        var accessToken = _tokenService.GerarAccessToken(
            usuario.Id,
            usuario.Nome.Nome,
            usuario.Email.Endereco,
            usuario.Perfil);

        return new AutenticarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            accessToken);
    }
}
