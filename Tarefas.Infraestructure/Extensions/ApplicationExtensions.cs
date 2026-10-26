
using Microsoft.Extensions.DependencyInjection;
using Tarefas.Application.Commands.TarefaCommands.AtualizarTarefa;
using Tarefas.Application.Commands.TarefaCommands.CancelarTarefa;
using Tarefas.Application.Commands.TarefaCommands.ConcluirTarefa;
// Tarefa Commands
using Tarefas.Application.Commands.TarefaCommands.CriarTarefa;
using Tarefas.Application.Commands.TarefaCommands.IniciarTarefa;
using Tarefas.Application.Commands.UsuarioCommands.AtivarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.AtualizarUsuario;
// Usuario Commands
using Tarefas.Application.Commands.UsuarioCommands.CriarUsuario;
using Tarefas.Application.Commands.UsuarioCommands.DesativarUsuario;
using Tarefas.Application.Queries.Tarefas.ListarTarefas;
using Tarefas.Application.Queries.Tarefas.ObterTarefaPorIdQuery;

namespace Tarefas.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Commands de Tarefa
        services.AddScoped<CriarTarefaCommandHandler>();
        services.AddScoped<IniciarTarefaCommandHandler>();
        services.AddScoped<AtualizarTarefaCommandHandler>();
        services.AddScoped<ConcluirTarefaCommandHandler>();
        services.AddScoped<CancelarTarefaCommandHandler>();

        // Commands de Usuario
        services.AddScoped<CriarUsuarioCommandHandler>();
        services.AddScoped<AtualizarUsuarioCommandHandler>();
        services.AddScoped<AtivarUsuarioCommandHandler>();
        services.AddScoped<DesativarUsuarioCommandHandler>();

        // Query Handlers — Consultas de Tarefas
        services.AddScoped<ObterTarefaPorIdQueryHandler>();
        services.AddScoped<ListarTarefasQueryHandler>();

        return services;
    }
}
