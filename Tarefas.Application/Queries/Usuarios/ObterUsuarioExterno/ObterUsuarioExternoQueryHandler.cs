using MediatR;
using Tarefas.Application.Queries.Usuarios.DTOs;
using Tarefas.Domain.Integration.Usuarios;

namespace Tarefas.Application.Queries.Usuarios.ObterUsuarioExterno;

public sealed class ObterUsuarioExternoQueryHandler
    : IRequestHandler<ObterUsuarioExternoQuery, ObterUsuarioExternoResultDto?>
{
    private readonly IUsuarioGateway _usuarioGateway;
    private readonly UsuarioAcl _usuarioAcl;

    public ObterUsuarioExternoQueryHandler(
        IUsuarioGateway usuarioGateway,
        UsuarioAcl usuarioAcl)
    {
        _usuarioGateway = usuarioGateway;
        _usuarioAcl = usuarioAcl;
    }

    public async Task<ObterUsuarioExternoResultDto?> Handle(
        ObterUsuarioExternoQuery request,
        CancellationToken cancellationToken)
    {
        var usuarioExterno = await _usuarioGateway.ObterUsuarioPorIdAsync(
            request.UsuarioId,
            cancellationToken);

        if (usuarioExterno is null)
            return null;


        var usuarioTraduzido = _usuarioAcl.TraduzirUsuario(usuarioExterno);

        return new ObterUsuarioExternoResultDto(
            usuarioExterno.Id,
            usuarioTraduzido.Nome,
            usuarioTraduzido.Email);
    }
}