using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;

public sealed class AtualizarTarefaResultDto
{
    public Guid TarefaId { get; }
    public string Titulo { get; }
    public string Descricao { get; }
    public PrioridadeTarefa Prioridade { get; }

    public AtualizarTarefaResultDto(
        Guid tarefaId, 
        string titulo, 
        string descricao, 
        PrioridadeTarefa prioridade)
    {
        TarefaId = tarefaId;
        Titulo = titulo;
        Descricao = descricao;
        Prioridade = prioridade;
    }
}

