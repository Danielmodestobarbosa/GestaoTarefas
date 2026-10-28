
using Microsoft.EntityFrameworkCore;
using Tarefas.Domain.Common.Base;
using Tarefas.Domain.Common.Interfaces;
using Tarefas.Domain.Tarefas.Entities;
using Tarefas.Domain.Usuarios.Entities;

namespace Tarefas.Infrastructure.Persistence;

public sealed class TarefasDbContext : DbContext
{
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    private readonly IDomainEventDispatcher _dispatcher;

    public TarefasDbContext(
        DbContextOptions<TarefasDbContext> options,
        IDomainEventDispatcher dispatcher)
        : base(options)
    {
        _dispatcher = dispatcher;
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        // 1. Coleta os eventos de domínio antes de salvar.
        var eventos = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // 2. Persiste as alterações no banco.
        var result = await base.SaveChangesAsync(ct);

        // 3. Limpa os eventos para evitar que sejam despachados novamente.
        foreach (var entry in ChangeTracker.Entries<Entity>())
            entry.Entity.ClearDomainEvents();

        // 4. Despacha os eventos somente após salvar com sucesso.
        if (eventos.Any())
            await _dispatcher.DispatchAsync(eventos, ct);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica as configurações do assembly Infrastructure.
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TarefasDbContext).Assembly);
    }
}

