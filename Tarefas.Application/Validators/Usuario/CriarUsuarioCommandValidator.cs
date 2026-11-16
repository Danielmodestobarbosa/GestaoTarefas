
using FluentValidation;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;

namespace Tarefas.Application.Validators.Usuario;

public sealed class CriarUsuarioCommandValidator
    : AbstractValidator<CriarUsuarioCommand>
{
    public CriarUsuarioCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotNull()
            .WithMessage("O nome do usuário é obrigatório.");

        RuleFor(x => x.Email)
            .NotNull()
            .WithMessage("O e-mail do usuário é obrigatório.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("A senha do usuário é obrigatória.")
            .Must(senha => !string.IsNullOrWhiteSpace(senha))
            .WithMessage("A senha não pode conter apenas espaços.")
            .MinimumLength(8)
            .WithMessage("A senha deve possuir no mínimo 8 caracteres.")
            .MaximumLength(128)
            .WithMessage("A senha deve possuir no máximo 128 caracteres.");
    }
}
