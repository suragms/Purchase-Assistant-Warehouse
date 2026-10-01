# Final Menu Parity Audit

This audit compares the frontend routes/menu structure against the reference repositories.

| Menu | Route | API Endpoint | Permissions Required | Role Required | Reference Status | Current Status | Action |
|---|---|---|---|---|---|---|---|
| Dashboard | `/dashboard` | `/api/v1/reports/home-overview` | `reports.view` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| Purchases | `/purchases/list` | `/api/v1/purchases` | `purchase.view` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| New Purchase | `/purchases/new` | `/api/v1/purchases` | `purchase.create` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| Stock | `/stock/dashboard` | `/api/v1/stock/dashboard` | `stock.view` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| Catalog | `/catalog/items` | `/api/v1/catalog/items` | `catalog.view` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| Suppliers | `/suppliers` | `/api/v1/catalog/suppliers` | `supplier.view` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| Brokers | `/brokers` | `/api/v1/catalog/brokers` | `broker.view` | Owner/Admin/Manager/Staff | COMPLETE | COMPLETE | - |
| Users | `/users` | `/api/v1/users` | `users.view` | Owner/Admin/Manager | COMPLETE | COMPLETE | - |
| Damage | `/damage` | `/api/v1/damage-reports` | `purchase.damage_report` | Owner/Admin/Manager/Staff | MISSING | MISSING | PHASE 4 |
| Daily Ops | `/operations` | `/api/v1/operations` | N/A | Owner/Admin | MISSING | MISSING | PHASE 4 |
| Export | `/export` | `/api/v1/reports/export` | `reports.view` | Owner/Admin | MISSING | MISSING | PHASE 5 |
| Settings | `/settings` | `/api/v1/settings` | `settings.manage` | Owner/Admin | MISSING | MISSING | PHASE 6 |
