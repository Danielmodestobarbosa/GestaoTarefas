using MediatR;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;

public sealed class AtivarUsuarioCommandHandler 
    : IRequestHandler<AtivarUsuarioCommand, AtivarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtivarUsuarioCommandHandler(
        IUsuarioRepository usuarioRepository, 
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AtivarUsuarioResultDto> Handle (
        AtivarUsuarioCommand request, 
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            request.UsuarioId,
            cancellationToken);

        if(usuario is null)
        {
            throw new InvalidOperationException("Usuario não encontrado");
        }

        usuario.AtivarUsuario();

        await _unitOfWork.CommitAsync(cancellationToken);

        return new AtivarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);
    }
}

