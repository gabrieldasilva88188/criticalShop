using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriticalShop.Migrations
{
    /// <inheritdoc />
    public partial class ReverterImagemParaArquivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagemBytes",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "TipoImagem",
                table: "Produtos");

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Produtos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "Produtos");

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
        }
    }
}
