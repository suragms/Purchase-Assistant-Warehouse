# Purchase Lifecycle

## Statuses
- Draft
- Pending
- Dispatched
- In Transit
- Arrived
- Staff Verifying
- Staff Verified
- Stock Committed
- Cancelled

## Flow
1. **Creation**: Wizard -> Idempotent API call -> `TradePurchase` generated (`Pending`).
2. **Delivery**: Moves through `Dispatched` -> `InTransit` -> `Arrived`.
3. **Verification**: Physical goods checked vs Purchase Lines -> `StaffVerified`.
4. **Commitment**: Hard validation, optimistic lock on stock -> Updates Item Stock -> Audits generated -> `StockCommitted`.
