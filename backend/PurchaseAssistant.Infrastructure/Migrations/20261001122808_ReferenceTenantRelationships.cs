using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseAssistant.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReferenceTenantRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerSuppliers_Brokers_BrokerId",
                table: "BrokerSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_BrokerSuppliers_Suppliers_SupplierId",
                table: "BrokerSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Brokers_LastBrokerId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Categories_CategoryId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_CategoryTypes_TypeId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Suppliers_LastSupplierId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogVariants_CatalogItems_CatalogItemId",
                table: "CatalogVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryTypes_Categories_CategoryId",
                table: "CategoryTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_CatalogItems_CatalogItemId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseOrderId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Brokers_BrokerId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_CatalogItems_CatalogItemId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItemPrices_CatalogItems_CatalogItemId",
                table: "SupplierItemPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItemPrices_Suppliers_SupplierId",
                table: "SupplierItemPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItems_CatalogItems_CatalogItemId",
                table: "SupplierItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItems_Suppliers_SupplierId",
                table: "SupplierItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItems_CatalogItemId",
                table: "SupplierItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItems_SupplierId",
                table: "SupplierItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItemPrices_CatalogItemId",
                table: "SupplierItemPrices");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItemPrices_SupplierId",
                table: "SupplierItemPrices");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_CatalogItemId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_BrokerId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_SupplierId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_CatalogItemId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_PurchaseOrderId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_CategoryTypes_CategoryId",
                table: "CategoryTypes");

            migrationBuilder.DropIndex(
                name: "IX_CatalogVariants_CatalogItemId",
                table: "CatalogVariants");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_CategoryId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_LastBrokerId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_LastSupplierId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_TypeId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_BrokerSuppliers_BrokerId",
                table: "BrokerSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_BrokerSuppliers_SupplierId",
                table: "BrokerSuppliers");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Suppliers_BusinessId_Id",
                table: "Suppliers",
                columns: new[] { "BusinessId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Purchases_BusinessId_Id",
                table: "Purchases",
                columns: new[] { "BusinessId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CategoryTypes_BusinessId_CategoryId_Id",
                table: "CategoryTypes",
                columns: new[] { "BusinessId", "CategoryId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_BusinessId_Id",
                table: "Categories",
                columns: new[] { "BusinessId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CatalogItems_BusinessId_Id",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Brokers_BusinessId_Id",
                table: "Brokers",
                columns: new[] { "BusinessId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItems_BusinessId_CatalogItemId",
                table: "SupplierItems",
                columns: new[] { "BusinessId", "CatalogItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemPrices_BusinessId_CatalogItemId",
                table: "SupplierItemPrices",
                columns: new[] { "BusinessId", "CatalogItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemPrices_BusinessId_SourcePurchaseId",
                table: "SupplierItemPrices",
                columns: new[] { "BusinessId", "SourcePurchaseId" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_BusinessId_BrokerId",
                table: "Purchases",
                columns: new[] { "BusinessId", "BrokerId" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_BusinessId_SupplierId",
                table: "Purchases",
                columns: new[] { "BusinessId", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_BusinessId_CatalogItemId",
                table: "PurchaseItems",
                columns: new[] { "BusinessId", "CatalogItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_BusinessId_CategoryId_TypeId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "CategoryId", "TypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_BusinessId_LastBrokerId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "LastBrokerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_BusinessId_LastSupplierId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "LastSupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_BrokerSuppliers_BusinessId_SupplierId",
                table: "BrokerSuppliers",
                columns: new[] { "BusinessId", "SupplierId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerSuppliers_Brokers_BusinessId_BrokerId",
                table: "BrokerSuppliers",
                columns: new[] { "BusinessId", "BrokerId" },
                principalTable: "Brokers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerSuppliers_Suppliers_BusinessId_SupplierId",
                table: "BrokerSuppliers",
                columns: new[] { "BusinessId", "SupplierId" },
                principalTable: "Suppliers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Brokers_BusinessId_LastBrokerId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "LastBrokerId" },
                principalTable: "Brokers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Categories_BusinessId_CategoryId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "CategoryId" },
                principalTable: "Categories",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_CategoryTypes_BusinessId_CategoryId_TypeId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "CategoryId", "TypeId" },
                principalTable: "CategoryTypes",
                principalColumns: new[] { "BusinessId", "CategoryId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Suppliers_BusinessId_LastSupplierId",
                table: "CatalogItems",
                columns: new[] { "BusinessId", "LastSupplierId" },
                principalTable: "Suppliers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogVariants_CatalogItems_BusinessId_CatalogItemId",
                table: "CatalogVariants",
                columns: new[] { "BusinessId", "CatalogItemId" },
                principalTable: "CatalogItems",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryTypes_Categories_BusinessId_CategoryId",
                table: "CategoryTypes",
                columns: new[] { "BusinessId", "CategoryId" },
                principalTable: "Categories",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_CatalogItems_BusinessId_CatalogItemId",
                table: "PurchaseItems",
                columns: new[] { "BusinessId", "CatalogItemId" },
                principalTable: "CatalogItems",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Purchases_BusinessId_PurchaseOrderId",
                table: "PurchaseItems",
                columns: new[] { "BusinessId", "PurchaseOrderId" },
                principalTable: "Purchases",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Brokers_BusinessId_BrokerId",
                table: "Purchases",
                columns: new[] { "BusinessId", "BrokerId" },
                principalTable: "Brokers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Suppliers_BusinessId_SupplierId",
                table: "Purchases",
                columns: new[] { "BusinessId", "SupplierId" },
                principalTable: "Suppliers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_CatalogItems_BusinessId_CatalogItemId",
                table: "StockMovements",
                columns: new[] { "BusinessId", "CatalogItemId" },
                principalTable: "CatalogItems",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItemPrices_CatalogItems_BusinessId_CatalogItemId",
                table: "SupplierItemPrices",
                columns: new[] { "BusinessId", "CatalogItemId" },
                principalTable: "CatalogItems",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItemPrices_Purchases_BusinessId_SourcePurchaseId",
                table: "SupplierItemPrices",
                columns: new[] { "BusinessId", "SourcePurchaseId" },
                principalTable: "Purchases",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItemPrices_Suppliers_BusinessId_SupplierId",
                table: "SupplierItemPrices",
                columns: new[] { "BusinessId", "SupplierId" },
                principalTable: "Suppliers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItems_CatalogItems_BusinessId_CatalogItemId",
                table: "SupplierItems",
                columns: new[] { "BusinessId", "CatalogItemId" },
                principalTable: "CatalogItems",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItems_Suppliers_BusinessId_SupplierId",
                table: "SupplierItems",
                columns: new[] { "BusinessId", "SupplierId" },
                principalTable: "Suppliers",
                principalColumns: new[] { "BusinessId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerSuppliers_Brokers_BusinessId_BrokerId",
                table: "BrokerSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_BrokerSuppliers_Suppliers_BusinessId_SupplierId",
                table: "BrokerSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Brokers_BusinessId_LastBrokerId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Categories_BusinessId_CategoryId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_CategoryTypes_BusinessId_CategoryId_TypeId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Suppliers_BusinessId_LastSupplierId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogVariants_CatalogItems_BusinessId_CatalogItemId",
                table: "CatalogVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryTypes_Categories_BusinessId_CategoryId",
                table: "CategoryTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_CatalogItems_BusinessId_CatalogItemId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Purchases_BusinessId_PurchaseOrderId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Brokers_BusinessId_BrokerId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Suppliers_BusinessId_SupplierId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_CatalogItems_BusinessId_CatalogItemId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItemPrices_CatalogItems_BusinessId_CatalogItemId",
                table: "SupplierItemPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItemPrices_Purchases_BusinessId_SourcePurchaseId",
                table: "SupplierItemPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItemPrices_Suppliers_BusinessId_SupplierId",
                table: "SupplierItemPrices");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItems_CatalogItems_BusinessId_CatalogItemId",
                table: "SupplierItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierItems_Suppliers_BusinessId_SupplierId",
                table: "SupplierItems");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Suppliers_BusinessId_Id",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItems_BusinessId_CatalogItemId",
                table: "SupplierItems");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItemPrices_BusinessId_CatalogItemId",
                table: "SupplierItemPrices");

            migrationBuilder.DropIndex(
                name: "IX_SupplierItemPrices_BusinessId_SourcePurchaseId",
                table: "SupplierItemPrices");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Purchases_BusinessId_Id",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_BusinessId_BrokerId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_BusinessId_SupplierId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_BusinessId_CatalogItemId",
                table: "PurchaseItems");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CategoryTypes_BusinessId_CategoryId_Id",
                table: "CategoryTypes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_BusinessId_Id",
                table: "Categories");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CatalogItems_BusinessId_Id",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_BusinessId_CategoryId_TypeId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_BusinessId_LastBrokerId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_BusinessId_LastSupplierId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_BrokerSuppliers_BusinessId_SupplierId",
                table: "BrokerSuppliers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Brokers_BusinessId_Id",
                table: "Brokers");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItems_CatalogItemId",
                table: "SupplierItems",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItems_SupplierId",
                table: "SupplierItems",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemPrices_CatalogItemId",
                table: "SupplierItemPrices",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemPrices_SupplierId",
                table: "SupplierItemPrices",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CatalogItemId",
                table: "StockMovements",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_BrokerId",
                table: "Purchases",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_SupplierId",
                table: "Purchases",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_CatalogItemId",
                table: "PurchaseItems",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_PurchaseOrderId",
                table: "PurchaseItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTypes_CategoryId",
                table: "CategoryTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogVariants_CatalogItemId",
                table: "CatalogVariants",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CategoryId",
                table: "CatalogItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_LastBrokerId",
                table: "CatalogItems",
                column: "LastBrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_LastSupplierId",
                table: "CatalogItems",
                column: "LastSupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_TypeId",
                table: "CatalogItems",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerSuppliers_BrokerId",
                table: "BrokerSuppliers",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerSuppliers_SupplierId",
                table: "BrokerSuppliers",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerSuppliers_Brokers_BrokerId",
                table: "BrokerSuppliers",
                column: "BrokerId",
                principalTable: "Brokers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerSuppliers_Suppliers_SupplierId",
                table: "BrokerSuppliers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Brokers_LastBrokerId",
                table: "CatalogItems",
                column: "LastBrokerId",
                principalTable: "Brokers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Categories_CategoryId",
                table: "CatalogItems",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_CategoryTypes_TypeId",
                table: "CatalogItems",
                column: "TypeId",
                principalTable: "CategoryTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Suppliers_LastSupplierId",
                table: "CatalogItems",
                column: "LastSupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogVariants_CatalogItems_CatalogItemId",
                table: "CatalogVariants",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryTypes_Categories_CategoryId",
                table: "CategoryTypes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_CatalogItems_CatalogItemId",
                table: "PurchaseItems",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Purchases_PurchaseOrderId",
                table: "PurchaseItems",
                column: "PurchaseOrderId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Brokers_BrokerId",
                table: "Purchases",
                column: "BrokerId",
                principalTable: "Brokers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId",
                table: "Purchases",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_CatalogItems_CatalogItemId",
                table: "StockMovements",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItemPrices_CatalogItems_CatalogItemId",
                table: "SupplierItemPrices",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItemPrices_Suppliers_SupplierId",
                table: "SupplierItemPrices",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItems_CatalogItems_CatalogItemId",
                table: "SupplierItems",
                column: "CatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierItems_Suppliers_SupplierId",
                table: "SupplierItems",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
