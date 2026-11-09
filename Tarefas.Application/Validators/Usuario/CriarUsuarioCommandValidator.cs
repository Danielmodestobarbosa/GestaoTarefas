
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
    }
}
