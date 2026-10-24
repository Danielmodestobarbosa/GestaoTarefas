using Tarefas.Domain.Usuarios.Entities;

namespace Tarefas.Application.Abstractions.Persistence;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterUsuarioPorIdAsync(
        Guid usuarioId, 
        CancellationToken cancellationToken = default);

    Task AdicionarAsync(
        Usuario usuario, 
        CancellationToken cancellationToken = default);

    Task AtualizarAsync(
        Usuario usuario, 
        CancellationToken cancellationToken = default);
}

