using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseDamageReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseDamageReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    ItemName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    QtyDamaged = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    DamageType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    ReportedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseDamageReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseDamageReports_CatalogItems_BusinessId_CatalogItemId",
                        columns: x => new { x.BusinessId, x.CatalogItemId },
                        principalTable: "CatalogItems",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PurchaseDamageReports_Purchases_BusinessId_PurchaseOrderId",
                        columns: x => new { x.BusinessId, x.PurchaseOrderId },
                        principalTable: "Purchases",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseDamageReports_Users_ReportedByUserId",
                        column: x => x.ReportedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDamageReports_BusinessId_CatalogItemId",
                table: "PurchaseDamageReports",
                columns: new[] { "BusinessId", "CatalogItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDamageReports_BusinessId_PurchaseOrderId",
                table: "PurchaseDamageReports",
                columns: new[] { "BusinessId", "PurchaseOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDamageReports_BusinessId_PurchaseOrderId_CreatedAt",
                table: "PurchaseDamageReports",
                columns: new[] { "BusinessId", "PurchaseOrderId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDamageReports_BusinessId_Status",
                table: "PurchaseDamageReports",
                columns: new[] { "BusinessId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseDamageReports_ReportedByUserId",
                table: "PurchaseDamageReports",
                column: "ReportedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseDamageReports");
        }
    }
}
