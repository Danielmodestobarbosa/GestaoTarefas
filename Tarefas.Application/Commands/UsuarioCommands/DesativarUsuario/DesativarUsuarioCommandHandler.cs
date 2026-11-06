using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Exceptions;

namespace Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioCommandHandler 
    : IRequestHandler<DesativarUsuarioCommand, DesativarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesativarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DesativarUsuarioResultDto> Handle (
        DesativarUsuarioCommand request, 
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            request.UsuarioId,
            cancellationToken);

        if(usuario is null)
        {
            throw new NotFoundException(
                "Usuario não encontrado");
        }

        usuario.DesativarPerfil();

        await _unitOfWork.CommitAsync(cancellationToken);

        return new DesativarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);

    }
}

