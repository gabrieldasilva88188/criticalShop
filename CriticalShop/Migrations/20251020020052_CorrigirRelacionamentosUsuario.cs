using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriticalShop.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirRelacionamentosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Usuarios_UsuarioId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Carrinhos_Usuarios_UsuarioId1",
                table: "Carrinhos");

            migrationBuilder.DropIndex(
                name: "IX_Carrinhos_UsuarioId1",
                table: "Carrinhos");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacoes_UsuarioId",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Carrinhos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Avaliacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId1",
                table: "Carrinhos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Avaliacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carrinhos_UsuarioId1",
                table: "Carrinhos",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_UsuarioId",
                table: "Avaliacoes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Usuarios_UsuarioId",
                table: "Avaliacoes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carrinhos_Usuarios_UsuarioId1",
                table: "Carrinhos",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
