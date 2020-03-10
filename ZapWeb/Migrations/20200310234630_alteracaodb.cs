using Microsoft.EntityFrameworkCore.Migrations;

namespace ZapWeb.Migrations
{
    public partial class alteracaodb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Usuario",
                table: "Mensagem");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Mensagem",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioJson",
                table: "Mensagem",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Mensagem");

            migrationBuilder.DropColumn(
                name: "UsuarioJson",
                table: "Mensagem");

            migrationBuilder.AddColumn<string>(
                name: "Usuario",
                table: "Mensagem",
                type: "TEXT",
                nullable: true);
        }
    }
}
