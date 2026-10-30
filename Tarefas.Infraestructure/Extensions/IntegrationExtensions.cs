using Microsoft.Extensions.DependencyInjection;
using Tarefas.Domain.Integration.Usuarios;
using Tarefas.Infrastructure.Services.Integration.Usuarios;

namespace Tarefas.Infrastructure.Extensions;

public static class IntegrationExtensions
{
    public static IServiceCollection AddIntegrations(
        this IServiceCollection services)
    {
        services.AddHttpClient<IUsuarioGateway, UsuarioGateway>(client =>
        {
            client.BaseAddress = new Uri(
                "https://jsonplaceholder.typicode.com/");
        });

        return services;
    }
}