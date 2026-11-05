using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Tarefas.Application.Exceptions;
using Tarefas.Domain.Common.Eexceptions;

namespace Tarefas.Api.Exceptions;

public sealed class TarefasExceptionHandler : IExceptionHandler
{
    private readonly ILogger<TarefasExceptionHandler> _logger;

    public TarefasExceptionHandler(
        ILogger<TarefasExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            NotFoundException =>
                (404, "Recurso não encontrado"),

            DomainException =>
                (422, "Regra de negócio violada"),

            InvalidOperationException =>
                (409, "Operação não permitida"),

            ArgumentException =>
                (400, "Requisição inválida"),

            _ =>
                (500, "Erro interno do servidor")
        };

        if (status == 500)
        {
            _logger.LogError(
                exception,
                "Erro inesperado: {Mensagem}",
                exception.Message);
        }
        else
        {
            _logger.LogWarning(
                "Exceção tratada [{Status}]: {Mensagem}",
                status,
                exception.Message);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken);

        return true;
    }
}
