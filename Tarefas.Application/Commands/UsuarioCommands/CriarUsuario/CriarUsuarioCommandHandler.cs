using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Abstractions.Security;
using Tarefas.Domain.Usuarios.Entities;

namespace Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;

public sealed class CriarUsuarioCommandHandler 
    : IRequestHandler<CriarUsuarioCommand, CriarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CriarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<CriarUsuarioResultDto> Handle(
        CriarUsuarioCommand request, 
        CancellationToken cancellationToken = default)
    {
        var usuario = new Usuario(
            request.Nome,
            request.Email);

        var passwordHash = _passwordHasher.HashPassword(request.Senha);

        usuario.DefinirSenha(passwordHash);

        await _usuarioRepository.AdicionarAsync(
            usuario,
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new CriarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);
    }
}

