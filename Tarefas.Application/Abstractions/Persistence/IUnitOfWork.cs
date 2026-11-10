namespace Tarefas.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    //Inicia uma transação no banco 
    Task BeginTransationAsync(
        CancellationToken cancellationToken = default);

    //Salva as alterações e confirma a transação.
    Task<int> CommitAsync (
        CancellationToken cancellationToken = default);

    //Desfaz as alterações da trasação
    Task RollBackAsync(
        CancellationToken cancellationToken = default);
}

