
using FluentValidation;

namespace Tarefas.Application.Commands.UsuarioCommands.AutenticarUsuario;

public sealed class AutenticarUsuarioCommandValidator
    : AbstractValidator<AutenticarUsuarioCommand>
{
    public AutenticarUsuarioCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .WithMessage("O e-mail é obrigatório.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .Must(senha => !string.IsNullOrWhiteSpace(senha))
            .WithMessage("A senha não pode conter apenas espaços.");
    }
}

