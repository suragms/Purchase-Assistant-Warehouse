using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteOperationsAndCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "NormalizedName", table: "CatalogVariants");
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CatalogVariants",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(name: "NormalizedName", table: "CatalogVariants", type: "text", nullable: true, computedColumnSql: "lower(btrim(\"Name\"))", stored: true);
            migrationBuilder.CreateIndex(name: "IX_CatalogVariants_BusinessId_CatalogItemId_NormalizedName", table: "CatalogVariants", columns: new[] { "BusinessId", "CatalogItemId", "NormalizedName" }, unique: true);
            migrationBuilder.CreateTable(
                name: "ProviderCredential",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EncryptedValue = table.Column<string>(type: "text", nullable: false),
                    LastFour = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderCredential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderCredential_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderCredential_Users_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReferenceId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Rejected = table.Column<bool>(type: "boolean", nullable: false),
                    CorrectionNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffTask_Memberships_BusinessId_StaffId",
                        columns: x => new { x.BusinessId, x.StaffId },
                        principalTable: "Memberships",
                        principalColumns: new[] { "BusinessId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffTask_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderCredential_BusinessId_CredentialType",
                table: "ProviderCredential",
                columns: new[] { "BusinessId", "CredentialType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderCredential_UpdatedById",
                table: "ProviderCredential",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StaffTask_BusinessId_StaffId_Status_AssignedAt",
                table: "StaffTask",
                columns: new[] { "BusinessId", "StaffId", "Status", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffTask_CreatedById",
                table: "StaffTask",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderCredential");

            migrationBuilder.DropTable(
                name: "StaffTask");

            migrationBuilder.DropColumn(name: "NormalizedName", table: "CatalogVariants");
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CatalogVariants",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);
            migrationBuilder.AddColumn<string>(name: "NormalizedName", table: "CatalogVariants", type: "text", nullable: true, computedColumnSql: "lower(btrim(\"Name\"))", stored: true);
            migrationBuilder.CreateIndex(name: "IX_CatalogVariants_BusinessId_CatalogItemId_NormalizedName", table: "CatalogVariants", columns: new[] { "BusinessId", "CatalogItemId", "NormalizedName" }, unique: true);
        }
    }
}

