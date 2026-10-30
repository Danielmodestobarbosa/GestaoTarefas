using MediatR;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;

public sealed class AtivarUsuarioCommandHandler 
    : IRequestHandler<AtivarUsuarioCommand, AtivarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AtivarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AtivarUsuarioResultDto> Handle (
        AtivarUsuarioCommand request, 
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            request.UsuarioId,
            cancellationToken);

        if(usuario is null)
        {
            throw new InvalidOperationException("Usuario não encontrado");
        }

        usuario.AtivarUsuario();

        await _usuarioRepository.AtualizarAsync(
            usuario,
            cancellationToken);

        return new AtivarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);
    }
}

