using MediatR;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Application.Queries.Usuarios.DTOs;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarioPorId;

public sealed class ObterUsuarioPorIdQueryHandler
    : IRequestHandler<ObterUsuarioPorIdQuery, ObterUsuarioPorIdResultDto?>
{
    private readonly IUsuarioQueryRepository _usuarioQueryRepository;

    public ObterUsuarioPorIdQueryHandler(
        IUsuarioQueryRepository usuarioQueryRepository)
    {
        _usuarioQueryRepository = usuarioQueryRepository;
    }

    public async Task<ObterUsuarioPorIdResultDto?> Handle(
        ObterUsuarioPorIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _usuarioQueryRepository.ObterPorIdAsync(
            request.UsuarioId,
            cancellationToken);
    }
}