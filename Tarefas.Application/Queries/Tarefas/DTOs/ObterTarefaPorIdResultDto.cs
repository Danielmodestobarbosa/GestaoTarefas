using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Queries.Tarefas.DTOs;

public sealed class ObterTarefaPorIdResultDto
{
    public Guid Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Descricao { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;
}

