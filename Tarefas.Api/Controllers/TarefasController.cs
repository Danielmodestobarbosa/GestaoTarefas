using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tarefas.Api.Requests.Tarefas;
using Tarefas.Application.Commands.TarefaCommands.AtribuirUsuarioTarefa;
using Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;
using Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;
using Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;
using Tarefas.Application.Queries.Tarefas.ListarTarefas;
using Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

namespace Tarefas.Api.Controllers;

[ApiController]
[Route("api/tarefas")]
public class TarefasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TarefasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [EndpointSummary("Obtém todas as tarefas cadastradas no sistema.")]
    public async Task<IActionResult> ListarTarefas(
    CancellationToken cancellationToken)
    {
        var query = new ListarTarefasQuery();

        var response = await _mediator.Send(query, cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém os dados de uma tarefa pelo seu identificador.")]
    public async Task<IActionResult> ObterTarefaPorId(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
    {
        var query = new ObterTarefaPorIdQuery(id);

        var response = await _mediator.Send(query, cancellationToken);

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    [HttpPost]
    [EndpointSummary("Cadastra uma nova tarefa no sistema.")]
    public async Task<IActionResult> CriarTarefa(
        [FromBody] CriarTarefaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CriarTarefaCommand(
            request.Titulo,
            request.Descricao,
            request.Prioridade);

        var response = await _mediator.Send(command, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Atualiza os dados de uma tarefa cadastrada.")]
    public async Task<IActionResult> AtualizarTarefa(
    [FromRoute] Guid id,
    [FromBody] AtualizarTarefaRequest request,
    CancellationToken cancellationToken)
    {
        var command = new AtualizarTarefaCommand(
            id,
            request.Titulo,
            request.Descricao,
            request.Prioridade);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/iniciar")]
    [EndpointSummary("Inicia uma tarefa cadastrada.")]
    public async Task<IActionResult> IniciarTarefa(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
    {
        var command = new IniciarTarefaCommand(id);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/concluir")]
    [EndpointSummary("Conclui uma tarefa cadastrada.")]
    public async Task<IActionResult> ConcluirTarefa(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
    {
        var command = new ConcluirTarefaCommand(id);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/cancelar")]
    [EndpointSummary("Cancela uma tarefa cadastrada.")]
    public async Task<IActionResult> CancelarTarefa(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
    {
        var command = new CancelarTarefaCommand(id);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{id:guid}/atribuir-usuario")]
    [EndpointSummary("Atribui um usuário a uma tarefa.")]
    public async Task<IActionResult> AtribuirUsuario(
    [FromRoute] Guid id,
    [FromBody] AtribuirUsuarioTarefaRequest request,
    CancellationToken cancellationToken)
    {
        var command = new AtribuirUsuarioTarefaCommand(
            id,
            request.UsuarioId);

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }
}