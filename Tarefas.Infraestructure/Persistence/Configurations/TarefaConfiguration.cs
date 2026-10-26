
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarefas.Domain.Tarefas.Entities;

namespace Tarefas.Infrastructure.Persistence.Configurations;

public sealed class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
{
    public void Configure(EntityTypeBuilder<Tarefa> builder)
    {
        builder.ToTable("Tarefas");

        builder.HasKey(t => t.Id);

        // O Id é gerado pela aplicação, não pelo banco.
        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Titulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Descricao)
            .IsRequired(false);

        // Persiste os enums como texto.
        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Prioridade)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // A tarefa pode ser criada sem um usuário atribuído.
        builder.Property(t => t.UsuarioId)
            .IsRequired(false);

        builder.Property(t => t.DataConclusao)
            .IsRequired(false);

        builder.Property(t => t.DataCriacao)
            .IsRequired();

        builder.Property(t => t.DataAtualizacao)
            .IsRequired(false);

        // Eventos de domínio não são persistidos como colunas.
        builder.Ignore(t => t.DomainEvents);
    }
}
