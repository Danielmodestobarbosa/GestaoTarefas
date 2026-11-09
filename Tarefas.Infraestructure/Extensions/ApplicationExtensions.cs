
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Domain.Integration.Usuarios;

namespace Tarefas.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Registro automático dos Command e Query Handlers.
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(CriarTarefaCommandHandler).Assembly));

        // Registro das dependências de integração.
        services.AddScoped<UsuarioAcl>();

        return services;
    }
}
