# Database Design

## Core Principles
- Migrations managed via EF Core.
- Soft Deletes where necessary (e.g. `IsDeleted`, `DeletedAt`).
- All tenant-owned records have `BusinessId`.
- UUIDs (Guid) for primary keys.
- Timestamps in UTC.
- All monetary/financial values stored as `decimal(18,4)`.

## Entities & Relationships

### Identity & Multi-Tenancy
- **Businesses**: `Id`, `Name`, `CreatedAt`, `IsActive`
- **Users**: `Id`, `Email`, `PasswordHash`, `IsActive`
- **Memberships**: `Id`, `BusinessId`, `UserId`, `Role` (Owner, Admin, Manager, Staff), `Permissions` (jsonb/flags)

### Catalog
- **Categories**: `Id`, `BusinessId`, `Name`, `IsActive`
- **CategoryTypes**: `Id`, `BusinessId`, `CategoryId`, `Name`
- **CatalogItems**: `Id`, `BusinessId`, `ItemCode`, `Barcode`, `Name`, `CategoryId`, `TypeId`, `DefaultUnit`, `KgPerUnit`, `ReorderLevel`, `IsActive`
- **CatalogVariants**: `Id`, `ItemId`, `Name`, `SKU`

### CRM
- **Suppliers**: `Id`, `BusinessId`, `Name`, `Phone`, `Address`, `IsActive`
- **Brokers**: `Id`, `BusinessId`, `Name`, `Phone`, `IsActive`

### Stock Engine
- **ItemStock**: `Id`, `ItemId`, `BusinessId`, `SystemStock`, `PhysicalStock`, `Version` (Concurrency Token)
- **StockAuditLogs**: `Id`, `ItemId`, `UserId`, `BusinessId`, `OldQuantity`, `NewQuantity`, `Reason`, `Type` (PhysicalCount, SystemCorrection, Purchase), `CreatedAt`

### Purchasing
- **TradePurchases**: `Id`, `BusinessId`, `SupplierId`, `BrokerId`, `PurchaseNumber`, `Status`, `PaymentStatus`, `TotalAmount`, `CreatedAt`, `CreatedById`
- **TradePurchaseLines**: `Id`, `TradePurchaseId`, `ItemId`, `Quantity`, `Unit`, `LandingCost`, `Weight`, `SellingPrice`, `LineAmount`
- **PurchaseLifecycleEvents**: `Id`, `TradePurchaseId`, `Status`, `CreatedAt`, `UserId`, `Notes`
- **PurchaseDamageReports**: `Id`, `TradePurchaseId`, `ItemId`, `ReportedQuantity`, `Reason`, `Status`

### Operational
- **IdempotencyKeys**: `Key`, `ResponseCode`, `ResponseBody`, `CreatedAt`
- **Notifications**: `Id`, `BusinessId`, `UserId` (Optional), `Title`, `Body`, `IsRead`, `CreatedAt`
- **StaffTasks** & **ChecklistCompletions**: Daily operational checks.
