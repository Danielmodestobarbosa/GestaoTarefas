
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tarefas.Application.Abstractions.Persistence;
using Tarefas.Domain.Common.Interfaces;
using Tarefas.Infrastructure.Event;
using Tarefas.Infrastructure.Persistence;
using Tarefas.Infrastructure.Repositories;

namespace Tarefas.Infrastructure.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada.");
        }

        services.AddDbContext<TarefasDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ITarefaRepository, TarefaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITarefaQueryRepository, TarefaQueryRepository>();

        services.AddScoped<
            IDomainEventDispatcher,
            InMemoryDomainEventDispatcher>();

        return services;
    }
}
