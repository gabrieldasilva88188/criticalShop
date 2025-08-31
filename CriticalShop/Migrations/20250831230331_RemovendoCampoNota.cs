using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriticalShop.Migrations
{
    /// <inheritdoc />
    public partial class RemovendoCampoNota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nota",
                table: "Produtos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Nota",
                table: "Produtos",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
