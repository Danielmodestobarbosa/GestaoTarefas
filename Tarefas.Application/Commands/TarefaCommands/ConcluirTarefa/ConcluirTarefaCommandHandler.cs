using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;

public sealed class ConcluirTarefaCommandHandler
{
    private readonly ITarefaRepository _tarefaRepository;

    public ConcluirTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        this._tarefaRepository = tarefaRepository;
    }

    public async Task<ConcluirTarefaResultDto> HandleAsync(
        ConcluirTarefaCommand command, 
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

        tarefa.Concluir();

        await _tarefaRepository.AtualizarAsync(
            tarefa,
            cancellationToken);

        return new ConcluirTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Status,
            tarefa.Prioridade,
            tarefa.DataConclusao);
            
    }

}

