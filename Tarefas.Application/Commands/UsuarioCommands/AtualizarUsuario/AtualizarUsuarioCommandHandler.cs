using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;

namespace Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;

public sealed class AtualizarUsuarioCommandHandler 
    : IRequestHandler<AtualizarUsuarioCommand, AtualizarUsuarioResultDto>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AtualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AtualizarUsuarioResultDto> Handle (
        AtualizarUsuarioCommand request, 
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            request.UsuarioId,
            cancellationToken);

        if(usuario is null)
        {
            throw new InvalidOperationException(
                "Usuario não encontrado");
        }

        usuario.AtualizarPerfil(
            request.Nome,
            request.Email);

        await _usuarioRepository.AtualizarAsync(
            usuario,
            cancellationToken);

        return new AtualizarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);
    }
}

