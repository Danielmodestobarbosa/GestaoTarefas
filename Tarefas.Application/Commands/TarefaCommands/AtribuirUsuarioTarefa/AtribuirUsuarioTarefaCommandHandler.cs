using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Usuarios.Enums;

namespace Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;

public sealed class AtribuirUsuarioTarefaCommandHandler
    : IRequestHandler<AtribuirUsuarioTarefaCommand, AtribuirUsuarioResultDto>
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtribuirUsuarioTarefaCommandHandler(
        ITarefaRepository tarefaRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AtribuirUsuarioResultDto> Handle(
        AtribuirUsuarioTarefaCommand request,
        CancellationToken cancellationToken)
    {
        var tarefa = await _tarefaRepository.ObterTarefaPorIdAsync(
            request.TarefaId,
            cancellationToken);

        if (tarefa is null)
        {
            throw new InvalidOperationException(
                "Tarefa não encontrada");
        }

        var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(
            request.UsuarioId,
            cancellationToken);

        if (usuario is null)
        {
            throw new InvalidOperationException(
                "Usuário não encontrado");
        }

        if (usuario.Status != StatusUsuario.Ativo)
        {
            throw new InvalidOperationException(
                "Não é possível atribuir uma tarefa a um usuário inativo");
        }

        tarefa.AtribuirUsuario(usuario.Id);

        await _unitOfWork.CommitAsync(cancellationToken);

        return new AtribuirUsuarioResultDto(
            tarefa.Id,
            usuario.Id,
            tarefa.DataAtualizacao);
    }
}