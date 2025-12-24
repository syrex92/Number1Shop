using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdersService.Data.Migrations
{
    /// <inheritdoc />
    public partial class productsandordercost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cost",
                table: "OrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "OrderItems");
        }
    }
}
