
using Microsoft.EntityFrameworkCore;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Common.Pagination;
using Tarefas.Application.Queries.Tarefas.DTOs;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Repositories;

public sealed class TarefaQueryRepository : ITarefaQueryRepository
{
    private readonly TarefasDbContext _context;

    public TarefaQueryRepository(TarefasDbContext context)
    {
        _context = context;
    }

    public async Task<ObterTarefaPorIdResultDto?> ObterPorIdAsync(
        Guid tarefaId,
        CancellationToken ct = default)
    {
        return await _context.Tarefas
            .AsNoTracking()
            .Where(t => t.Id == tarefaId)
            .Select(t => new ObterTarefaPorIdResultDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                Status = t.Status.ToString(),
                Prioridade = t.Prioridade.ToString()
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<PagedResult<TarefaResumoDto>> ListarAsync(
    int pageNumber,
    int pageSize,
    CancellationToken ct = default)
    {
        // Garante valores válidos para a paginação
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        // Monta a consulta sem executar no banco
        var query = _context.Tarefas
            .AsNoTracking()
            .Select(t => new TarefaResumoDto
            {
                TarefaId = t.Id,
                Titulo = t.Titulo,
                Status = t.Status.ToString(),
                Prioridade = t.Prioridade.ToString()
            });

        // Obtém a quantidade total de tarefas
        var totalCount = await query.CountAsync(ct);

        // Busca somente os registros da página solicitada
        var items = await query
            .OrderBy(t => t.Titulo)
            .ThenBy(t => t.TarefaId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<TarefaResumoDto>(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }
}
