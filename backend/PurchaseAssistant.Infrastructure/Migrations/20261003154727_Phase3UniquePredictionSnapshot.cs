using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase3UniquePredictionSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MlPredictionLogs_UniqueForecast",
                table: "MlPredictionLogs",
                columns: new[] { "BusinessId", "CatalogItemId", "StartDate", "Horizon", "ModelVersion", "InputVersion" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MlPredictionLogs_UniqueForecast",
                table: "MlPredictionLogs");
        }
    }
}
