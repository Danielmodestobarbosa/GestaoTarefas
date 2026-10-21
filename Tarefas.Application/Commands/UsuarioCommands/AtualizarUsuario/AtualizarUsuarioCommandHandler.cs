using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;

public sealed class AtualizarUsuarioCommandHandler
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AtualizarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AtualizarUsuarioResultDto> HandleAsync (
        AtualizarUsuarioCommand command, 
        CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            command.UsuarioId,
            cancellationToken);

        if(usuario is null)
        {
            throw new InvalidOperationException(
                "Usuario não encontrado");
        }

        usuario.AtualizarPerfil(
            command.Nome,
            command.Email);

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

