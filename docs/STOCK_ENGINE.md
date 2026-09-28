# Stock Engine Design

The stock engine is the heart of the operational ERP, ensuring absolute correctness.

## Concepts
- **System Stock:** Calculated expected value of items based on committed Purchases and Usage Deductions.
- **Physical Stock:** Disconnected value representing the actual human count on hand.
- **Available Stock:** Safe representation for usage (usually matches System Stock).

## Concurrency
Entity Framework `[Timestamp]` / PostgreSQL `xmin` or manual UUID `RowVersion`.
Every read yields a `StockVersion`. Every write requires sending it back.
`UPDATE CatalogItems SET Stock = ..., RowVersion = new_uuid WHERE Id = ... AND RowVersion = old_version`

## Audit Trail
Modifying stock directly is FORBIDDEN. It must be done via specific Actions:
1. `PurchaseStockCommitCommand`
2. `ManualSystemStockAdjustmentCommand` (Requires Reason)
3. `PhysicalCountAdjustmentCommand`
4. `UsageRecordingCommand`

Each creates a `StockAuditLog`.
**Formula check:** Summing `StockAuditLog` since the beginning of time MUST equal `CurrentSystemStock`. 

## Front-End Optimizations
- **No Storms:** Invalidation of TanStack stock tables only occurs when `SignalR` notifies of an actual change.
- **Optimistic UI:** Stock increment changes can use query invalidation rather than full page reloads.

## Reports & Alerts
- **Low Stock:** Evaluated at commit time `CurrentStock <= ReorderLevel`. Dispatches notification.
- **Dead Stock:** Derived from `StockAuditLog` showing zero outgoing events over X days.