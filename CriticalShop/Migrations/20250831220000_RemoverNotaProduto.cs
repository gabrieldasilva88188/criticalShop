using Microsoft.EntityFrameworkCore.Migrations;

namespace CriticalShop.Migrations
{
    public partial class RemoverNotaProduto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nota",
                table: "Produtos");
        }

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
