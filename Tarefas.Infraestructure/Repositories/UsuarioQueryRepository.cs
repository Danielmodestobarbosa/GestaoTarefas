using Microsoft.EntityFrameworkCore;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Persistence.Repositories;

public sealed class UsuarioQueryRepository : IUsuarioQueryRepository
{
    private readonly TarefasDbContext _context;

    public UsuarioQueryRepository(TarefasDbContext context)
    {
        _context = context;
    }

    public async Task<ObterUsuarioPorIdResultDto?> ObterPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Id == usuarioId,
                cancellationToken);

        if (usuario is null)
            return null;

        return new ObterUsuarioPorIdResultDto(
            usuario.Id,
            usuario.Nome.Nome,
            usuario.Email.Endereco,
            usuario.Status.ToString());
    }
    public async Task<IReadOnlyList<ObterUsuariosResultDto>> ObterTodosAsync(
    CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Select(usuario => new ObterUsuariosResultDto(
                usuario.Id,
                usuario.Nome.Nome,
                usuario.Email.Endereco,
                usuario.Status.ToString()))
            .ToListAsync(cancellationToken);
    }

}
