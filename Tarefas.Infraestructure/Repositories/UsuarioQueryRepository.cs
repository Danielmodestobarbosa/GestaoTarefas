using Microsoft.EntityFrameworkCore;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Common.Pagination;
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
    public async Task<PagedResult<ObterUsuariosResultDto>> ListarAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // Garante valores válidos para a paginação.
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Usuarios
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(usuario => usuario.Nome.Nome)
            .ThenBy(usuario => usuario.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(usuario => new ObterUsuariosResultDto(
                usuario.Id,
                usuario.Nome.Nome,
                usuario.Email.Endereco,
                usuario.Status.ToString()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ObterUsuariosResultDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

}
