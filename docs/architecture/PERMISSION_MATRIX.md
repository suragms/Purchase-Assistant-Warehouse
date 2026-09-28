# Permission Matrix

## Roles
- `Owner`: Full access to everything including subscriptions, tenant settings, and audits.
- `Admin`: Full operations, user management, and reporting.
- `Manager`: Manage purchases, inventory, suppliers, view reports, staff tasks.
- `Staff`: Create purchase drafts, complete operational checklists, physical stock counts.

| Permission | Owner | Admin | Manager | Staff |
|---|---|---|---|---|
| purchase_create | Yes | Yes | Yes | Draft Only |
| purchase_edit | Yes | Yes | Yes | No |
| purchase_commit | Yes | Yes | Yes | No |
| stock_view | Yes | Yes | Yes | Yes |
| stock_adjust | Yes | Yes | Yes | No |
| physical_count | Yes | Yes | Yes | Yes |
| reports_view | Yes | Yes | Yes | No |
| users_manage | Yes | Yes | No | No |
