using Tarefas.Domain.Common.Base;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Common.Validations;

namespace Tarefas.Domain.Usuarios.ValueObjects;

public sealed class NomeUsuario : ValueObject
{
    public string Nome { get; private set; }

    public NomeUsuario(string nome)
    {
        Guard.AgainstNullOrWhiteSpace(
            nome, nameof(nome), 
            "O nome do usuário não pode ser nulo ou vazio.");

        nome = nome.Trim();

        Guard.Against<DomainException>(
            nome.Length < 3 || nome.Length > 100,
            "O nome deve possuir entre 3 e 100 caracteres");

        Nome = nome;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Nome;
    }

    public override string ToString() => Nome;
}

