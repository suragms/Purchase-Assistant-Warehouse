using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase3MlUsageAndMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "DailyUsageLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            // Only movement-backed historical submissions can be distinguished from generated snapshots.
            migrationBuilder.Sql("""
                UPDATE "DailyUsageLogs" AS usage SET "IsConfirmed" = true
                WHERE EXISTS (SELECT 1 FROM "StockMovements" AS movement
                    WHERE movement."BusinessId" = usage."BusinessId"
                    AND movement."CatalogItemId" = usage."CatalogItemId"
                    AND movement."ReferenceType" = 'DailyUsage'
                    AND movement."ReferenceId" = usage."Id"::text)
                """);

            migrationBuilder.CreateTable(
                name: "MlPredictionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelVersion = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    InputVersion = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Horizon = table.Column<int>(type: "integer", nullable: false),
                    PredictedQuantity = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false),
                    DailyPredictionsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlPredictionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MlPredictionLogs_CatalogItems_BusinessId_CatalogItemId",
                        columns: x => new { x.BusinessId, x.CatalogItemId },
                        principalTable: "CatalogItems",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MlPredictionLogs_BusinessId_CatalogItemId_CreatedAt",
                table: "MlPredictionLogs",
                columns: new[] { "BusinessId", "CatalogItemId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MlPredictionLogs");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "DailyUsageLogs");
        }
    }
}
