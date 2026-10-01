using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceCatalogVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "KgPerUnit",
                table: "CatalogVariants",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RowVersion",
                table: "CatalogVariants",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "CatalogVariants",
                type: "text",
                nullable: true,
                computedColumnSql: "lower(btrim(\"Name\"))",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVariants_BusinessId_CatalogItemId_NormalizedName",
                table: "CatalogVariants",
                columns: new[] { "BusinessId", "CatalogItemId", "NormalizedName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatalogVariants_BusinessId_CatalogItemId_NormalizedName",
                table: "CatalogVariants");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "CatalogVariants");

            migrationBuilder.DropColumn(
                name: "KgPerUnit",
                table: "CatalogVariants");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CatalogVariants");
        }
    }
}
