using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalOperationsSettingsImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Businesses",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandingLogoUrl",
                table: "Businesses",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandingTitle",
                table: "Businesses",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Businesses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GstNumber",
                table: "Businesses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoStorageKey",
                table: "Businesses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Businesses",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Version",
                table: "Businesses",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Brokers",
                type: "text",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Memberships_BusinessId_UserId",
                table: "Memberships",
                columns: new[] { "BusinessId", "UserId" });

            migrationBuilder.CreateTable(
                name: "ChecklistCompletion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Slot = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    TaskKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistCompletion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistCompletion_Users_CompletedByUserId",
                        column: x => x.CompletedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Slot = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyOperationSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalChecklistTasks = table.Column<int>(type: "integer", nullable: false),
                    CompletedChecklistTasks = table.Column<int>(type: "integer", nullable: false),
                    ChecklistCompletionRate = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    TotalItemsUsed = table.Column<int>(type: "integer", nullable: false),
                    TotalQuantityUsed = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    DeadStockItems = table.Column<int>(type: "integer", nullable: false),
                    FastMovingItems = table.Column<int>(type: "integer", nullable: false),
                    SlowMovingItems = table.Column<int>(type: "integer", nullable: false),
                    MaterializedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyOperationSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyUsageLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CatalogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpeningQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    PurchasedQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    UsedQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ClosingQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    LoggedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyUsageLogs_CatalogItems_BusinessId_CatalogItemId",
                        columns: x => new { x.BusinessId, x.CatalogItemId },
                        principalTable: "CatalogItems",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DailyUsageLogs_Users_LoggedByUserId",
                        column: x => x.LoggedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    NotificationKindsJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Memberships_BusinessId_UserId",
                        columns: x => new { x.BusinessId, x.UserId },
                        principalTable: "Memberships",
                        principalColumns: new[] { "BusinessId", "UserId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompletion_BusinessId_CompletedByUserId_Date_Slot_~",
                table: "ChecklistCompletion",
                columns: new[] { "BusinessId", "CompletedByUserId", "Date", "Slot", "TaskKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistCompletion_CompletedByUserId",
                table: "ChecklistCompletion",
                column: "CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplate_BusinessId_Slot_Key",
                table: "ChecklistTemplate",
                columns: new[] { "BusinessId", "Slot", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyOperationSnapshots_BusinessId_Date",
                table: "DailyOperationSnapshots",
                columns: new[] { "BusinessId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyUsageLogs_BusinessId_CatalogItemId",
                table: "DailyUsageLogs",
                columns: new[] { "BusinessId", "CatalogItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyUsageLogs_BusinessId_Date_CatalogItemId",
                table: "DailyUsageLogs",
                columns: new[] { "BusinessId", "Date", "CatalogItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyUsageLogs_LoggedByUserId",
                table: "DailyUsageLogs",
                column: "LoggedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_BusinessId_UserId",
                table: "UserSettings",
                columns: new[] { "BusinessId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistCompletion");

            migrationBuilder.DropTable(
                name: "ChecklistTemplate");

            migrationBuilder.DropTable(
                name: "DailyOperationSnapshots");

            migrationBuilder.DropTable(
                name: "DailyUsageLogs");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Memberships_BusinessId_UserId",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "BrandingLogoUrl",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "BrandingTitle",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "GstNumber",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "LogoStorageKey",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Brokers");

}
    }
}
