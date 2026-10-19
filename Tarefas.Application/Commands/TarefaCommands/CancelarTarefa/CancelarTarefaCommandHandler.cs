using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;

public sealed class CancelarTarefaCommandHandler
{
    private readonly ITarefaRepository _tarefaRepository;

    public CancelarTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<CancelarTarefaResultDto> HandleAsync (
        CancelarTarefaCommand command, 
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

        tarefa.Cancelar();

        await _tarefaRepository.AtualizarAsync(
            tarefa, 
            cancellationToken);

        return new CancelarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade);
    }
}

