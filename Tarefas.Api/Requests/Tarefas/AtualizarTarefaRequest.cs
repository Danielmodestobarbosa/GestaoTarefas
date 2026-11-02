using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Api.Requests.Tarefas;

public sealed record AtualizarTarefaRequest(
    string Titulo,
    string Descricao,
    PrioridadeTarefa Prioridade);