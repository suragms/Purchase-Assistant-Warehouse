# Database Design Specification

Uses PostgreSQL with Entity Framework Core.
Schema relies on UUIDs for PKs and soft-delete conventions.
Every business-centric table contains `BusinessId` indexed for multi-tenant isolation.

## 1. Core Tables

### Businesses
- `Id` (UUID, PK)
- `Name` (String)
- `CreatedAt`, `UpdatedAt`

### Users & Memberships
- **Users**: `Id`, `Email`, `PasswordHash`, `IsActive`
- **Memberships**: `Id`, `BusinessId`, `UserId`, `Role` (Enum), `Permissions` (JSONB)
- **RefreshTokens**: `Token` (String), `UserId`, `ExpiresAt`, `IsRevoked`

### Catalog (Items, Categories, Types)
- **Categories**: `Id`, `BusinessId`, `Name`
- **CategoryTypes**: `Id`, `BusinessId`, `CategoryId`, `Name`
- **CatalogItems**: `Id`, `BusinessId`, `CategoryId`, `TypeId`, `ItemCode` (Unique DB constraint per business), `Barcode` (Unique per business), `Name`, `DefaultUnit`, `KgPerUnit` (Decimal), `ReorderLevel` (Decimal), `CurrentStock` (Calculated System Stock wrapper), `RowVersion` (Concurrency token).

### Contacts
- **Suppliers**: `Id`, `BusinessId`, `Name`, `Phone`, `Address`, `IsActive`
- **Brokers**: `Id`, `BusinessId`, `Name`, `IsActive`

### Stock Management
- **StockAuditLogs**: `Id`, `BusinessId`, `CatalogItemId`, `UserId`, `OldQuantity`, `NewQuantity`, `AdjustmentType` (Enum: Receive, Manual, Void), `Reason`
- **DailySnapshots**: `Id`, `BusinessId`, `Date`, `TotalValue`, `SnapshotData` (JSONB)

### Purchases
- **TradePurchases**: 
  - `Id`, `BusinessId`
  - `HumanId` (e.g., PUR-2026-0001)
  - `SupplierId`, `BrokerId`
  - `Status` (Enum: Draft, Pending, Dispatched, Arrived, StaffVerified, StockCommitted, Cancelled)
  - `PaymentStatus` (Enum: Pending, DueSoon, Overdue, Paid)
  - `TotalAmount` (Decimal)
  - `RowVersion`
- **TradePurchaseLines**:
  - `Id`, `TradePurchaseId`, `CatalogItemId`
  - `Quantity` (Decimal), `Unit` (Enum)
  - `KgPerUnit` (Decimal)
  - `LandingCost` (Decimal), `SellingPrice` (Decimal)
  - `LineTotal` (Decimal - Calculated)

### System / Audit
- **IdempotencyKeys**: `Key` (String, PK), `Method`, `Path`, `ResponseCode`, `ResponseBody`, `CreatedAt`
- **Notifications**: `Id`, `BusinessId`, `TargetRole/UserId`, `Title`, `Body`, `IsRead`

## 2. Integrity Rules
- UTC Timestamps everywhere.
- `Decimal(18,4)` for quantities and pricing.
- No cross-business Foreign Keys. 
- EF Core `HasQueryFilter(e => e.BusinessId == currentTenantId)` applied to all tenant entities.