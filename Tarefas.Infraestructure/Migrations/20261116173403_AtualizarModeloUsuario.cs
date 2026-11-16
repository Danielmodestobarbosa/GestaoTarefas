using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tarefas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarModeloUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Perfil",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil",
                table: "Usuarios");
        }
    }
}
