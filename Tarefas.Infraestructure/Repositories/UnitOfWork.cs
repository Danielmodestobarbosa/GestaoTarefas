using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly TarefasDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(TarefasDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException(
                "Já existe uma transação em andamento.");

        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _context.SaveChangesAsync(
            cancellationToken);

        if(_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);

            await _transaction.DisposeAsync();
            _transaction = null;
        }

        return result;
    }

    public async Task RollBackAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

    }
}

