using MediatR;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;

public sealed class DesativarUsuarioCommandHandler 
    : IRequestHandler<DesativarUsuarioCommand, DesativarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public DesativarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        this._usuarioRepository = usuarioRepository;
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
            throw new InvalidOperationException(
                "Usuario não enconrado");
        }

        usuario.DesativarPerfil();

        await _usuarioRepository.AtualizarAsync(
            usuario,
            cancellationToken);

        return new DesativarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);

    }
}

