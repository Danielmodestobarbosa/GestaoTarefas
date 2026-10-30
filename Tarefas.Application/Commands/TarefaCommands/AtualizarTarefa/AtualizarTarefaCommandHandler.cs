using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;

namespace Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;

public sealed class AtualizarTarefaCommandHandler 
    : IRequestHandler<AtualizarTarefaCommand, AtualizarTarefaResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;

    public AtualizarTarefaCommandHandler(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<AtualizarTarefaResultDto> Handle(
        AtualizarTarefaCommand request, 
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            request.TarefaId,
            cancellationToken);

        if(tarefa is null)
        {
            throw new InvalidOperationException(
                "Tarefa não encontrada");
        }

        tarefa.Atualizar(
            request.Titulo,
            request.Descricao,
            request.Prioridade);

        await _tarefaRepository.AtualizarAsync(
            tarefa,
            cancellationToken);

        return new AtualizarTarefaResultDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Prioridade);
    }
}

