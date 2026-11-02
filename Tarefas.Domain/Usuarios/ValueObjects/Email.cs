using Tarefas.Domain.Common.Base;
using Tarefas.Domain.Common.Eexceptions;
using Tarefas.Domain.Common.Validations;

namespace Tarefas.Domain.Usuarios.ValueObjects;

public sealed class Email : ValueObject
{ 
    public string Endereco { get; private set; }

    public Email (string endereco)
    {
        Guard.AgainstNullOrWhiteSpace(
            endereco, nameof(endereco), 
            "O endereço de email é obrigatório");

        endereco = endereco.Trim();

        Guard.Against<DomainException>(
            !System.Text.RegularExpressions.Regex.IsMatch(
                endereco,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"),
            "O e-mail informado é inválido.");

        Endereco = endereco;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Endereco;
    }

    public override string ToString() => Endereco;
}

