# Frontend Route Map

Root architecture utilizing React Router v7 (or v6 Data API).

## Public Routes
* `/login` - Authentication, forgot password logic.

## Protected Routes (require Auth setup)
Wrapped by `<AuthGuard>` and `<AppShell>` (Sidebar, Header, Layout).

### Dashboard
* `/dashboard` - Main KPI tracking

### Purchases
* `/purchases` - List view with tabs (Active, History, Drafts)
* `/purchases/new` - Purchase Wizard
* `/purchases/:id` - Read-only details / lifecycle manager
* `/purchases/:id/edit` - Modify drafts
* `/purchases/:id/delivery` - Manage logistics transition
* `/purchases/:id/verify` - Staff verification sheet
* `/purchases/:id/payment` - Payment management modal/page

### Inventory
* `/inventory/stock` - Master stock virtualized table
* `/inventory/low-stock`
* `/inventory/physical` - Physical count adjustment UI
* `/inventory/system` - System adjustment UI
* `/inventory/audits` - Immutable audit log
* `/inventory/items/:id/activity` - Timeline view

### Catalog
* `/catalog/items`
* `/catalog/items/new`
* `/catalog/categories`
* `/catalog/types`
* `/catalog/variants`
* `/catalog/duplicates` - Fuzzy AI duplicate dashboard

### Contacts
* `/suppliers`
* `/suppliers/:id`
* `/brokers`
* `/brokers/:id`

### Reports
* `/reports/summary`
* `/reports/items`
* `/reports/suppliers`
* `/reports/categories`
* `/reports/profit`
* `/reports/movement`
* `/reports/activity` (Chronological feed)

### Operations
* `/operations/checklist`
* `/operations/usage`
* `/operations/snapshots`

### Settings & Staff
* `/staff/users` - Roster management
* `/staff/users/:id`
* `/staff/tasks`
* `/staff/activity`
* `/notifications`
* `/settings/profile`
* `/settings/business`
* `/settings/providers` (AI Keys)

## UX Modals / Drawers
- Global Search (Command + K)
- Notification Center Side Panel
- Quick Barcode Scan Modal