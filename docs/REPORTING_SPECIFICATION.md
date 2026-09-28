# Reporting Specification

Data integrity demands ONE source of truth: The Backend calculations.

## 1. Mathematical Standards (Backend Only)
1. **Weight Lines**: `quantity_purchased * kg_per_unit * landing_cost_per_kg`
2. **Standard Lines**: `quantity_purchased * landing_cost_per_unit`
3. **Total Profit calculation**: `SellingPriceTotal - LandingCostTotal` for that operation.

*Important:* The Frontend NEVER calculates summary totals to save into the database. It only sums for UI display previews.

## 2. Standardized Reports
- **Dashboard Snapshot**: Pre-calculated daily via `HostedService` and stored in `DailySnapshots` to prevent summing millions of rows on Dashboard load.
- **Period Comparison**: Queries `SUM(TotalAmount)` where `Date BETWEEN A AND B` vs `Date BETWEEN C AND D`. Used extensively in trend charts.
- **Supplier Analytics**: Averages of `landing_cost` weighted by `quantity` for a specific item per supplier. Generates the "Best available price" metric.

## 3. Operations Reporting
- **Usage Tracking**: Generates historical burn-down charts. Evaluates how much of an item was removed (AdjustmentType = Usage) per day.

## 4. UI Representation
- React Recharts for standard visualization.
- Standard Date Picker for all endpoints.
- Paginated table exports to CSV directly via standard API format (Streaming CSV generation from backend).