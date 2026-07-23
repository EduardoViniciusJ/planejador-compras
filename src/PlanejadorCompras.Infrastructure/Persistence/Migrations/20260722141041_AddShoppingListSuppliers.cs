using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanejadorCompras.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingListSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingListSuppliers",
                columns: table => new
                {
                    ShoppingListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingListSuppliers", x => new { x.ShoppingListId, x.SupplierId });
                    table.ForeignKey(
                        name: "FK_ShoppingListSuppliers_ShoppingLists_ShoppingListId",
                        column: x => x.ShoppingListId,
                        principalTable: "ShoppingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingListSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO ShoppingListSuppliers (ShoppingListId, SupplierId, CreatedAt)
                SELECT items.ShoppingListId, quotes.SupplierId, MIN(quotes.CreatedAt)
                FROM ItemQuotes AS quotes
                INNER JOIN ShoppingItems AS items ON items.Id = quotes.ShoppingItemId
                GROUP BY items.ShoppingListId, quotes.SupplierId;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListSuppliers_SupplierId",
                table: "ShoppingListSuppliers",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingListSuppliers");
        }
    }
}
