
using MediatR;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.CriarTarefa;

public sealed class CriarTarefaCommand : IRequest<CriarTarefaResultDto>
{
    public string Titulo { get; }
    public string Descricao { get; }
    public PrioridadeTarefa Prioridade { get; }

    public CriarTarefaCommand(
        string titulo,
        string descricao,
        PrioridadeTarefa prioridade)
    {
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
    }
}
