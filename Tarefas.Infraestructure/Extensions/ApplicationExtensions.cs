using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Application.Queries.Tarefas.ListarTarefas;
using Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

namespace Tarefas.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Registro dos handlers do MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(CriarTarefaCommandHandler).Assembly));

        // Query Handlers existentes
        services.AddScoped<ObterTarefaPorIdQueryHandler>();
        services.AddScoped<ListarTarefasQueryHandler>();

        return services;
    }
}