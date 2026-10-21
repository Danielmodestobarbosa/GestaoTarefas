using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Tarefas.Entities;

namespace Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;

public sealed class IniciarTarefaCommandHandler
{
    private readonly ITarefaRepository _tarefaRepository;

    public IniciarTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<IniciarTarefaResultDto> HandleAsync (
        IniciarTarefaCommand command, 
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            command.TarefaId,
            cancellationToken);

        if(tarefa is null)
        {
            throw new InvalidOperationException(
                "Tarefa não encontrada");
        }

        tarefa.Iniciar();

        await _tarefaRepository.AtualizarAsync(
            tarefa,
            cancellationToken);

        return new IniciarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade);

    }
}

