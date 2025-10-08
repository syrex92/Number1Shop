using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.FavoriteService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FavoriteListId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteItem_FavoriteList_FavoriteListId",
                        column: x => x.FavoriteListId,
                        principalTable: "FavoriteList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteItem_FavoriteListId",
                table: "FavoriteItem",
                column: "FavoriteListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteItem");

            migrationBuilder.DropTable(
                name: "FavoriteList");
        }
    }
}
