
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tarefas.Api.Requests.Usuarios;
using Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;
using Tarefas.Application.Queries.Usuarios.ObterUsuarioExterno;
using Tarefas.Application.Queries.Usuarios.ObterUsuarioPorId;
using Tarefas.Application.Queries.Usuarios.ObterUsuarios;
using Tarefas.Domain.Usuarios.ValueObjects;

namespace Tarefas.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém um usuário cadastrado pelo seu Id")]
    public async Task<IActionResult> ObterUsuarioPorId(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
    {
        var query = new ObterUsuarioPorIdQuery(id);

        var response = await _mediator.Send(query, cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpGet]
    [EndpointSummary("Obtém todos os usuários cadastrados no sistema.")]
    public async Task<IActionResult> ObterUsuarios(
    CancellationToken cancellationToken)
    {
        var query = new ObterUsuariosQuery();

        var response = await _mediator.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpPost]
    [EndpointSummary("Cadastra um novo usuário no sistema.")]
    public async Task<IActionResult> CriarUsuario(
        [FromBody] CriarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CriarUsuarioCommand(
            new NomeUsuario(request.Nome),
            new Email(request.Email));

        var response = await _mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Atualiza os dados de um usuário cadastrado.")]
    public async Task<IActionResult> AtualizarUsuario(
        [FromRoute] Guid id,
        [FromBody] AtualizarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AtualizarUsuarioCommand(
            id,
            new NomeUsuario(request.Nome),
            new Email(request.Email));

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/ativar")]
    [EndpointSummary("Ativa um usuário anteriormente desativado")]
    public async Task<IActionResult> AtivarUsuario(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new AtivarUsuarioCommand(id);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/desativar")]
    [EndpointSummary("Desativa um usuário ativo.")]
    public async Task<IActionResult> DesativarUsuario(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DesativarUsuarioCommand(id);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpGet("externo/{usuarioId:int}")]
    [EndpointSummary("Consulta os dados de um usuário na API externa.")]
    public async Task<IActionResult> ObterUsuarioExterno(
        [FromRoute] int usuarioId,
        CancellationToken cancellationToken)
    {
        var query = new ObterUsuarioExternoQuery(usuarioId);

        var response = await _mediator.Send(query, cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }
}
