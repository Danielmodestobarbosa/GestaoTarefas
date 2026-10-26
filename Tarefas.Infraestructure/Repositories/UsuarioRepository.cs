
using Microsoft.EntityFrameworkCore;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Usuarios.Entities;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Repositories;

public sealed class UsuarioRepository : IUsuarioRepository
{
    private readonly TarefasDbContext _context;

    public UsuarioRepository(TarefasDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterUsuarioPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(
                u => u.Id == usuarioId,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
