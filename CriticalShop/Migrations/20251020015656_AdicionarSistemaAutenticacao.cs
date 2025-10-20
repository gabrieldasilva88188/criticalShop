using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriticalShop.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarSistemaAutenticacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            // Adicionar campos faltantes na tabela Usuarios
            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Usuarios",
                type: "TEXT",
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Usuarios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Atualizar constraints das colunas existentes
            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Usuarios",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Senha",
                table: "Usuarios",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Endereco",
                table: "Usuarios",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

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

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "Endereco",
                table: "Usuarios",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
