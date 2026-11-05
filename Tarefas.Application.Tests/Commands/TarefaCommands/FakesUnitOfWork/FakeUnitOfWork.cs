using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Tests.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int QuantidadeDeCommits { get; private set; }

    public CancellationToken UltimoTokenCommit { get; private set; }

    public Exception? ExcecaoAoCommit { get; set; }

    public Task<int> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        QuantidadeDeCommits++;

        UltimoTokenCommit = cancellationToken;

        if (ExcecaoAoCommit is not null)
            throw ExcecaoAoCommit;

        return Task.FromResult(1);
    }
}