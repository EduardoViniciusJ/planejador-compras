using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanejadorCompras.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equalizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceShoppingListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ShoppingListName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedByEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    BestChoiceTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BestCompleteSupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BestCompleteSupplierTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedEconomy = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equalizations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceShoppingListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceEqualizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ShoppingListName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BuyerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BuyerEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EqualizationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedEqualizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceShoppingItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EqualizationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EqualizationItems_Equalizations_SavedEqualizationId",
                        column: x => x.SavedEqualizationId,
                        principalTable: "Equalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceShoppingItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EqualizationQuotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedEqualizationItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceSupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsLowest = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EqualizationQuotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EqualizationQuotes_EqualizationItems_SavedEqualizationItemId",
                        column: x => x.SavedEqualizationItemId,
                        principalTable: "EqualizationItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EqualizationItems_SavedEqualizationId",
                table: "EqualizationItems",
                column: "SavedEqualizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EqualizationItems_SavedEqualizationId_Position",
                table: "EqualizationItems",
                columns: new[] { "SavedEqualizationId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EqualizationQuotes_SavedEqualizationItemId",
                table: "EqualizationQuotes",
                column: "SavedEqualizationItemId");

            migrationBuilder.CreateIndex(
                name: "IX_EqualizationQuotes_SavedEqualizationItemId_SourceSupplierId",
                table: "EqualizationQuotes",
                columns: new[] { "SavedEqualizationItemId", "SourceSupplierId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equalizations_Code",
                table: "Equalizations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equalizations_UserId_CreatedAtUtc",
                table: "Equalizations",
                columns: new[] { "UserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Equalizations_UserId_RequestId",
                table: "Equalizations",
                columns: new[] { "UserId", "RequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_Code",
                table: "PurchaseOrders",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SourceEqualizationId",
                table: "PurchaseOrders",
                column: "SourceEqualizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_UserId_CreatedAtUtc",
                table: "PurchaseOrders",
                columns: new[] { "UserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_UserId_SourceShoppingListId_SupplierId",
                table: "PurchaseOrders",
                columns: new[] { "UserId", "SourceShoppingListId", "SupplierId" },
                unique: true,
                filter: "[Status] <> 3 AND [SourceShoppingListId] IS NOT NULL AND [SupplierId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EqualizationQuotes");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "EqualizationItems");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Equalizations");
        }
    }
}
