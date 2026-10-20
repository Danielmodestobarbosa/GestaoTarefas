using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Usuarios.Entities;

namespace Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;

public sealed class CriarUsuarioCommandHandler
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CriarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<CriarUsuarioResultDto> HandleAsync(
        CriarUsuarioCommand command, 
        CancellationToken cancellationToken = default)
    {
        var usuario = new Usuario(
            command.Nome,
            command.Email);

        await _usuarioRepository.AdicionarAsync(
            usuario,
            cancellationToken);

        return new CriarUsuarioResultDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Status);
    }
}

