using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceSessionFamilies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FamilyId",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "TokenDigest",
                table: "RefreshTokens",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_FamilyId_RevokedAt",
                table: "RefreshTokens",
                columns: new[] { "UserId", "FamilyId", "RevokedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_TokenDigest",
                table: "RefreshTokens",
                columns: new[] { "UserId", "TokenDigest" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId_FamilyId_RevokedAt",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId_TokenDigest",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "TokenDigest",
                table: "RefreshTokens");
        }
    }
}
