using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanejadorCompras.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "ItemQuotes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                """
                INSERT INTO Suppliers (Id, UserId, Name, CreatedAt)
                SELECT NEWID(), lists.UserId, LTRIM(RTRIM(quotes.SupplierName)), MIN(quotes.CreatedAt)
                FROM ItemQuotes AS quotes
                INNER JOIN ShoppingItems AS items ON items.Id = quotes.ShoppingItemId
                INNER JOIN ShoppingLists AS lists ON lists.Id = items.ShoppingListId
                GROUP BY lists.UserId, LTRIM(RTRIM(quotes.SupplierName));

                UPDATE quotes
                SET quotes.SupplierId = suppliers.Id
                FROM ItemQuotes AS quotes
                INNER JOIN ShoppingItems AS items ON items.Id = quotes.ShoppingItemId
                INNER JOIN ShoppingLists AS lists ON lists.Id = items.ShoppingListId
                INNER JOIN Suppliers AS suppliers
                    ON suppliers.UserId = lists.UserId
                    AND suppliers.Name = LTRIM(RTRIM(quotes.SupplierName));
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "ItemQuotes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "SupplierName",
                table: "ItemQuotes");

            migrationBuilder.CreateIndex(
                name: "IX_ItemQuotes_SupplierId",
                table: "ItemQuotes",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_UserId_Name",
                table: "Suppliers",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemQuotes_Suppliers_SupplierId",
                table: "ItemQuotes",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierName",
                table: "ItemQuotes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE quotes
                SET quotes.SupplierName = suppliers.Name
                FROM ItemQuotes AS quotes
                INNER JOIN Suppliers AS suppliers ON suppliers.Id = quotes.SupplierId;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "SupplierName",
                table: "ItemQuotes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_ItemQuotes_Suppliers_SupplierId",
                table: "ItemQuotes");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_ItemQuotes_SupplierId",
                table: "ItemQuotes");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "ItemQuotes");

        }
    }
}
