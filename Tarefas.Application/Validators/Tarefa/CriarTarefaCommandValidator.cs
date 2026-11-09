
using FluentValidation;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;

namespace Tarefas.Application.Validators.Tarefa;

public sealed class CriarTarefaCommandValidator
    : AbstractValidator<CriarTarefaCommand>
{
    public CriarTarefaCommandValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("O título da tarefa é obrigatório.")
            .MaximumLength(150)
            .WithMessage("O título deve possuir no máximo 150 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .WithMessage("A descrição da tarefa é obrigatória.")
            .MaximumLength(1000)
            .WithMessage("A descrição deve possuir no máximo 1000 caracteres.");

        RuleFor(x => x.Prioridade)
            .IsInEnum()
            .WithMessage("A prioridade informada é inválida.");
    }
}
