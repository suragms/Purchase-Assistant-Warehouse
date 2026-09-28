# Stock Engine Specification

## Concepts
- **System Stock**: The calculated truth based on purchases and documented adjustments.
- **Physical Stock**: What is physically counted on the shelves.
- **Stock Version**: Essential concurrency token (optimistic locking) to prevent lost updates or double deductions.

## Rules
- Updates to stock MUST use the current `StockVersion`. If a conflict occurs, return HTTP 409 and instruct the client to pull the latest state and allow user to retry.
- Stock adjustments always generate a `StockAuditLog`.
- `SystemStock` and `PhysicalStock` updates are separate workflows.

## Low Stock
- Triggered when `CurrentStock <= ReorderLevel`.
- Generates notifications via BackgroundService / cron job or on mutation.
