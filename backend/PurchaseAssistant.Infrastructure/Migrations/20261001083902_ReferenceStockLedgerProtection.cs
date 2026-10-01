using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceStockLedgerProtection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_CatalogItems_CatalogItemId",
                table: "StockMovements");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_CatalogItems_CatalogItemId",
                table: "StockMovements",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_CatalogItems_CatalogItemId",
                table: "StockMovements");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_CatalogItems_CatalogItemId",
                table: "StockMovements",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
