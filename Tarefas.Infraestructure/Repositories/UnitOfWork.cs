using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Infrastructure.Persistence;

namespace Tarefas.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
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

        if(_context.Database.CurrentTransaction is not null)
        throw new InvalidOperationException(
            "O DbContext já participa de uma transação.");

        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(
    CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return await _context.SaveChangesAsync(
                cancellationToken);
        }

        var transaction = _transaction;

        try
        {
            var result = await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception exception)
        {
            try
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);
            }
            catch (Exception rollbackException)
            {
                throw new AggregateException(
                    "Falha ao confirmar a transação e também ao executar rollback.",
                    exception,
                    rollbackException);
            }

            throw;
        }
        finally
        {
            _transaction = null;

            await transaction.DisposeAsync();
        }
    }

    public async Task RollBackAsync(
    CancellationToken cancellationToken = default)
    {
        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction = null;

            await transaction.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        var transaction = _transaction;

        if (transaction is null)
            return;

        try
        {
            await transaction.RollbackAsync(
                CancellationToken.None);
        }
        finally
        {
            _transaction = null;

            await transaction.DisposeAsync();
        }
    }
}

