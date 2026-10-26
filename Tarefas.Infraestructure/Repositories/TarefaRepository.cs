
using Microsoft.EntityFrameworkCore;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Repositories;

public sealed class TarefaRepository : ITarefaRepository
{
    private readonly TarefasDbContext _context;

    public TarefaRepository(TarefasDbContext context)
    {
        _context = context;
    }

    public async Task<Tarefa?> ObterTarefaPorIdAsync(
        Guid tarefaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tarefas
            .FirstOrDefaultAsync(
                t => t.Id == tarefaId,
                cancellationToken);
    }

    public async Task AdicionarAsync(
        Tarefa tarefa,
        CancellationToken cancellationToken = default)
    {
        await _context.Tarefas.AddAsync(tarefa, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        Tarefa tarefa,
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
