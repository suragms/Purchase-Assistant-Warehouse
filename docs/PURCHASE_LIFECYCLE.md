# Purchase Lifecycle Workflow

The Purchase mechanism guarantees stock integrity by separating the intention to purchase from the actual stock realization.

## States
1. **Draft:** 
   - Non-canonical. Stored on backend so users can resume. No stock impact.
2. **Pending:** 
   - Confirmed purchase. Sent to supplier.
3. **Dispatched:**
   - Supplier shipped goods.
4. **In Transit:**
   - On the way.
5. **Arrived:** 
   - Goods hit the warehouse yard.
6. **Staff Verifying:**
   - Workflow initiated by Staff to scan barcodes and confirm received vs ordered quantities.
7. **Staff Verified:** 
   - Verification complete. Variances flagged.
8. **Stock Committed (Terminal):**
   - The transaction officially applies to `CatalogItems.CurrentStock`. 
   - Generates `StockAuditLogs`.
   - Immutable line additions.
9. **Cancelled / Returned:**
   - Aborts stock. Allowed only before `StockCommitted`.

## Validation & Business Rules
- **Duplication Check:** On transitioning from `Draft` to `Pending`, the system fuzzy-matches existing purchases by Date, Supplier, and Total amount. Emits HTTP 409 if suspected duplicate, requiring explicit `force_create: true` flag.
- **Idempotency:** The POST request contains an idempotency key. Safely handles double-clicks.
- **Concurrency:** Committing stock uses optimistic concurrency (`RowVersion`) on the item record. If conflict occurs, read latest, calculate diff, apply, and save.