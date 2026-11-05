using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Usuarios.Entities;

namespace Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;

public sealed class CriarUsuarioCommandHandler 
    : IRequestHandler<CriarUsuarioCommand, CriarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CriarUsuarioResultDto> Handle(
        CriarUsuarioCommand request, 
        CancellationToken cancellationToken = default)
    {
        var usuario = new Usuario(
            request.Nome,
            request.Email);

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

