using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyerService.Migrations
{
    /// <inheritdoc />
    public partial class BuyerDbSetup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WishLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Items = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WishListsProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WishListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishListsProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishListsProductMappings_WishLists_WishListId",
                        column: x => x.WishListId,
                        principalTable: "WishLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartProductMappings_CartId",
                table: "CartProductMappings",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListsProductMappings_WishListId",
                table: "WishListsProductMappings",
                column: "WishListId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartProductMappings_Carts_CartId",
                table: "CartProductMappings",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartProductMappings_Carts_CartId",
                table: "CartProductMappings");

            migrationBuilder.DropTable(
                name: "WishListsProductMappings");

            migrationBuilder.DropTable(
                name: "WishLists");

            migrationBuilder.DropIndex(
                name: "IX_CartProductMappings_CartId",
                table: "CartProductMappings");
        }
    }
}
