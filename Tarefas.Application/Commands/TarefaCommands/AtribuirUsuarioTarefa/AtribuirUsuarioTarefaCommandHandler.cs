using System;
using System.Collections.Generic;
using System.Text;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Usuarios.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;

public sealed class AtribuirUsuarioTarefaCommandHandler
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public AtribuirUsuarioTarefaCommandHandler(ITarefaRepository tarefaRepository, IUsuarioRepository usuarioRepository)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<AtribuirUsuarioResultDto> HandleAsync(
        AtribuirUsuarioTarefaCommand command,
        CancellationToken cancellationToken = default)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            command.TarefaId,
            cancellationToken);

        if (tarefa is null)
        {
            throw new InvalidOperationException("Tarefa não encontrada");
        }

        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            command.UsuarioId,
            cancellationToken);

        if(usuario is null)
        {
            throw new InvalidOperationException("Usuário não encontrado");
        }

        if(usuario.Status != StatusUsuario.Ativo)
        {
            throw new InvalidOperationException("Não é possível atribuir uma tarefa a um usuário inativo");
        }

        tarefa.AtribuirUsuario(usuario.Id);

        await _tarefaRepository.AtualizarAsync(
            tarefa,
            cancellationToken);

        return new AtribuirUsuarioResultDto(
            tarefa.Id,
            usuario.Id,
            tarefa.DataAtualizacao);
    }
}


