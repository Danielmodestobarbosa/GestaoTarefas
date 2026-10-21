using System;
using System.Collections.Generic;
using System.Text;

namespace Tarefas.Application.Queries.Tarefas.DTOs;

public sealed class TarefaResumoDto
{
    public Guid TarefaId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Prioridade { get; init; } = string.Empty;

}

