using Tarefas.Domain.Common.Eexceptions;

namespace Tarefas.Domain.Common.Validations;

public static class Guard
{
    //Verifica se o identificador recebido está vazio 
    public static void AgainstEmptyGuid(
        Guid id, 
        string paramName, 
        string? message = null)
    {
        if(id == Guid.Empty)
            throw new DomainException(message ?? $"O parâmetro '{paramName}' não pode ser um Guid vazio.");
    }

    //Verifica se o valor recebido é nulo
    public static void AgainstNull<T>(
        T value, 
        string paramName) 
    { 
        if(value == null)
            throw new DomainException(
                $"O parâmetro '{paramName}' não pode ser nulo.");
    }

    //Versão com mensagem customizada
    public static void AgainstNull<T>(
        T value, 
        string paramName, 
        string message)
    {
        if (value == null)
            throw new DomainException(message);
    }

    //Verifica se uma string está nula, vazia ou contem apenas espaços em branco.
    public static void AgainstNullOrWhiteSpace(
        string value, 
        string paramName, 
        string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                message ?? $"O parâmetro '{paramName}' não pode ser nulo ou vazio.");
    }

    public static void Against<TException>(
        bool condition, 
        string message) 
        where TException : Exception
    {
        if (condition)
            throw (TException)Activator.CreateInstance(typeof(TException), message)!;
    }
}

