namespace PurchaseAssistant.Domain.Constants
{
    public static class Permissions
    {
        public const string CatalogView = "catalog.view";
        public const string CatalogCreate = "catalog.create";
        public const string CatalogEdit = "catalog.edit";
        public const string CatalogArchive = "catalog.archive";

        public const string SupplierView = "supplier.view";
        public const string SupplierCreate = "supplier.create";
        public const string SupplierEdit = "supplier.edit";
        public const string SupplierDelete = "supplier.delete";

        public const string BrokerView = "broker.view";
        public const string BrokerCreate = "broker.create";
        public const string BrokerEdit = "broker.edit";
        public const string BrokerDelete = "broker.delete";

        public const string PurchaseView = "purchase.view";
        public const string PurchaseCreate = "purchase.create";
        public const string PurchaseEdit = "purchase.edit";
        public const string PurchaseDelete = "purchase.delete";
        public const string PurchasePayment = "purchase.payment";
        public const string PurchaseDelivery = "purchase.delivery";
        public const string PurchaseVerify = "purchase.verify";
        public const string PurchaseCommit = "purchase.commit";

        public const string StockView = "stock.view";
        public const string StockAdjust = "stock.adjust";
        public const string StockPhysical = "stock.physical";
        public const string StockSystem = "stock.system";

        public const string ReportsView = "reports.view";
        public const string UsersView = "users.view";
        public const string UsersManage = "users.manage";
        public const string RolesManage = "roles.manage";
        public const string SettingsManage = "settings.manage";
        public const string ProvidersManage = "providers.manage";
    }
}