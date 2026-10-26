
using Microsoft.EntityFrameworkCore;
using Tarefas.Application.Abstractions.Persistence;
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

    public async Task<IReadOnlyList<TarefaResumoDto>> ListarAsync(
        CancellationToken ct = default)
    {
        return await _context.Tarefas
            .AsNoTracking()
            .Select(t => new TarefaResumoDto
            {
                TarefaId = t.Id,
                Titulo = t.Titulo,
                Status = t.Status.ToString(),
                Prioridade = t.Prioridade.ToString()
            })
            .OrderBy(t => t.Titulo)
            .ToListAsync(ct);
    }
}
