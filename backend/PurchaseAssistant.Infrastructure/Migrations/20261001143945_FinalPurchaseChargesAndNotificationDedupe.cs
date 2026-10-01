using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalPurchaseChargesAndNotificationDedupe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BilltyCharge",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CommissionMode",
                table: "Purchases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercent",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveredCharge",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FreightType",
                table: "Purchases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "HeaderDiscountPercent",
                table: "Purchases",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BilltyCharge",
                table: "PurchaseItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveredCharge",
                table: "PurchaseItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FreightAmount",
                table: "PurchaseItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreightType",
                table: "PurchaseItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "PurchaseItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DedupeKey",
                table: "Notifications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BilltyCharge",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CommissionMode",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CommissionPercent",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "DeliveredCharge",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "FreightType",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "HeaderDiscountPercent",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "BilltyCharge",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "DeliveredCharge",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "FreightAmount",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "FreightType",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "DedupeKey",
                table: "Notifications");
        }
    }
}
