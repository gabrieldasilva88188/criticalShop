using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriticalShop.Migrations
{
    /// <inheritdoc />
    public partial class CRUDProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Usuarios_UserId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Categorias_ParentId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Descontos_DescontoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "Produtos");

            migrationBuilder.AlterColumn<double>(
                name: "Preco",
                table: "Produtos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AlterColumn<double>(
                name: "Nota",
                table: "Produtos",
                type: "decimal(3,1)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImagemBytes",
                table: "Produtos",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoImagem",
                table: "Produtos",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Usuarios_UserId",
                table: "Avaliacoes",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Categorias_ParentId",
                table: "Categorias",
                column: "ParentId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Descontos_DescontoId",
                table: "Produtos",
                column: "DescontoId",
                principalTable: "Descontos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Usuarios_UserId",
                table: "Avaliacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Categorias_ParentId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Descontos_DescontoId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ImagemBytes",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "TipoImagem",
                table: "Produtos");

            migrationBuilder.AlterColumn<double>(
                name: "Preco",
                table: "Produtos",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Nota",
                table: "Produtos",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "decimal(3,1)");

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Produtos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Usuarios_UserId",
                table: "Avaliacoes",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Categorias_ParentId",
                table: "Categorias",
                column: "ParentId",
                principalTable: "Categorias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Descontos_DescontoId",
                table: "Produtos",
                column: "DescontoId",
                principalTable: "Descontos",
                principalColumn: "Id");
        }
    }
}
