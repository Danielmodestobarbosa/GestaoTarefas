using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Api.Requests.Tarefas;

public sealed record CriarTarefaRequest(
    string Titulo,
    string Descricao,
    PrioridadeTarefa Prioridade);