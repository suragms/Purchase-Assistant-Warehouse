# Reporting Specification

## Principles
- All financial calculations happen server-side based on `TradePurchaseLines`.
- Frontend never calculates profit or totals locally.

## Math
- **Weight-Based Line**: `Amount = Qty * KgPerUnit * LandingCostPerKg`
- **Unit Line**: `Amount = Qty * LandingCost`
- **Expected Profit**: `SellingValue - LineAmount`

## Core Reports
1. **Purchase Summary**: Grouped by period, totals, profit.
2. **Item Insights**: Best performing items, highest margin.
3. **Supplier / Broker Analysis**: Deals count, avg arrival time, total spend.
4. **Stock Movement**: Historical trajectory of inventory levels.
