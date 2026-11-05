using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TarefasDbContext _context;

    public UnitOfWork(TarefasDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

