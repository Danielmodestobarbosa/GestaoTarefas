
using Microsoft.Extensions.DependencyInjection;
using Tarefas.Application.Abstractions.Security;
using Tarefas.Infrastructure.Security;

namespace Tarefas.Infrastructure.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        return services;
    }
}
