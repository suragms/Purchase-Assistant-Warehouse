# Warehouse Model Decision — 2026-10-03

## Decision

**Use one logical warehouse and one stock pool per Business.** The current system’s Business is its tenant and warehouse boundary. A membership assigns a user to that Business, which assigns the user to its one stock pool. Do not add a separate Warehouse table or WarehouseId stock partition in this phase.

This describes the architecture the current app supports. It does not claim that one Business can never need multiple physical locations. Adding that capability later requires a separate product and migration decision.

## Current architecture

In the main application:

- Business owns memberships and stores name, status and business profile information.
- Membership.BusinessId is the staff/owner data-access assignment.
- CatalogItem, PurchaseOrder, StockMovement, supplier links and other tenant data carry BusinessId.
- Catalog stock fields (CurrentStock, PhysicalStock, ReservedStock) live directly on CatalogItem; they are not partitioned by location.
- Stock movement rows identify BusinessId and CatalogItemId, not a warehouse.
- API authorization requires the selected businessId; tenant filters and composite foreign keys use the same boundary.
- Development bootstrap creates a Business named “Main Warehouse”; there is no warehouse CRUD API or warehouse table.

In the reference application:

- Business is the workspace/data-ownership boundary; catalog, purchases and stock are scoped to business_id.
- UserListOut.warehouse_name is obtained from Business.name in backend/app/routers/users.py.
- There is no warehouse entity or per-location stock foreign key in the reference models or Alembic schema.
- Warehouse alert/report wording refers to the Business stock pool; it does not demonstrate multiple locations.

## Why this model

Both source trees consistently encode one stock pool per tenant. Neither has per-warehouse quantities, movement destinations, transfer transactions, warehouse-scoped report filters or staff-to-location memberships. Treating a text label or current stock field as evidence of multi-location support would be unsafe. The faithful parity interpretation is therefore one logical warehouse per Business, with membership assignment providing warehouse access.

## Impact

### Database

No schema migration is needed to preserve the selected model. Existing stock rows, balances, purchase lines, supplier links and audit rows remain scoped to their existing BusinessId. Business.IsActive is the logical warehouse availability state; the Business name is the current display label. The business profile remains the place to edit that name.

### API

No parallel warehouses API is added. Existing selected-business routes are the warehouse context. Adding a second location under a Business must not be simulated by another name or a second Business because that would split membership, suppliers, purchases and financial ownership.

### RBAC

Owner/staff access remains controlled by the selected Business membership and permissions. There is no separate warehouse claim or per-location assignment. A future multi-location model will need explicit location permissions in addition to tenant permissions.

### Stock

CurrentStock, PhysicalStock, ReservedStock and the append-only movement ledger describe one logical pool per Business/item. There is no transfer operation between locations. A future expansion must define source/destination movements, atomic transfers, reservation scope, reconciliation sessions and history before implementation.

### Reporting

Current inventory, low-stock and financial reports are Business-wide. Warehouse filters are unnecessary when a Business has exactly one logical warehouse. Adding multiple locations will require report filter semantics and prevention of double-counting.

## Migration and backward compatibility

This decision preserves the current schema and API contracts, so existing data requires no backfill and existing clients remain compatible. “Warehouse access” is the user’s existing Business membership. The main app and reference remain aligned.

If the product later approves multiple warehouses per Business, treat it as a breaking data-model project: add a Warehouse entity; create a deterministic default warehouse for each Business; backfill item balances and movements; define how purchase receipts map; add warehouse-aware composite constraints and membership assignments; update every stock, adjustment, reconciliation, report, export, notification and AI context; and retain compatibility until reconciliation proves old/new balances match. Do not perform that migration implicitly.

## Verification evidence

- Main entity definitions: Business.cs, Membership.cs, CatalogItem.cs, StockMovement.cs.
- Main EF model and migration history contain no Warehouse entity/table/foreign key.
- Reference models/business.py has no warehouse relation; routers/users.py implements _warehouse_name as a query of Business.name.
- Reference stock/catalog/trade queries key data by business_id and item, not warehouse ID.

**Status:** decided for the current product contract; multi-location expansion is deferred and requires explicit approval.
