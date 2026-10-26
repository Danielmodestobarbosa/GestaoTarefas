
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tarefas.Domain.Usuarios.Entities;

namespace Tarefas.Infrastructure.Persistence.Configurations;

public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        // O identificador é gerado pela aplicação.
        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        // Mapeia o objeto de valor NomeUsuario.
        builder.OwnsOne(u => u.Nome, nome =>
        {
            nome.Property(n => n.Nome)
                .HasColumnName("Nome")
                .IsRequired()
                .HasMaxLength(100);
        });

        // Mapeia o objeto de valor Email.
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Endereco)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(254);
        });

        // Persiste o enum como texto.
        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.DataCriacao)
            .IsRequired();

        builder.Property(u => u.DataAtualizacao)
            .IsRequired(false);

        // Os eventos de domínio não são colunas da tabela.
        builder.Ignore(u => u.DomainEvents);
    }
}
