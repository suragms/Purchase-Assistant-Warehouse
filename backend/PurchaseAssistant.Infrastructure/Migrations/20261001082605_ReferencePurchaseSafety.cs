using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReferencePurchaseSafety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // xmin is a PostgreSQL system column; do not add or drop it.

            migrationBuilder.AddColumn<decimal>(
                name: "KgPerUnit",
                table: "PurchaseItems",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LandingCostPerKg",
                table: "PurchaseItems",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "KgPerUnit",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "LandingCostPerKg",
                table: "PurchaseItems");
        }
    }
}
