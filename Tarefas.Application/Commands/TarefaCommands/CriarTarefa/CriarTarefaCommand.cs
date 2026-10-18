using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Domain.Tarefas.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.CriarTarefa;

public sealed class CriarTarefaCommand
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

