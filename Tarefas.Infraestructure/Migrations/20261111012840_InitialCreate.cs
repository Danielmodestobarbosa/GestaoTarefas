
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tarefas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Banco existente: estrutura já criada anteriormente.
            // Não precisamos recriar as tabelas.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Não excluir as tabelas existentes ao reverter o baseline.
        }
    }
}