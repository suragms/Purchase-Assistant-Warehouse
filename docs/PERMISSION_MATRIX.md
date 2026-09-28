# Permission Matrix

## Roles
1. **Super Admin**: System-wide configuration (usually cross-tenant, or tenant 0).
2. **Owner**: God-mode within a specific `BusinessId`.
3. **Admin**: Can do almost everything Owner can, minus billing/deleting the business.
4. **Manager**: Oversees staff operations, purchases, and stock, views reports.
5. **Staff**: Can read catalog, process deliveries, update specific checklists.

## Granular Resource Matrix

| Feature / Action | Owner | Admin | Manager | Staff |
|------------------|-------|-------|---------|-------|
| **Catalog**      | C/R/U/D | C/R/U/D | C/R/U | R     |
| **Suppliers**    | C/R/U/D | C/R/U/D | C/R/U | R     |
| **Purchases**    | C/R/U/D | C/R/U/D | C/R/U | R (Verify Only) |
| **Purchase Pay** | Yes   | Yes     | Yes     | No      |
| **Stock View**   | Yes   | Yes     | Yes     | Yes     |
| **Stock Adjust** | Yes   | Yes     | Yes     | No      |
| **Physical Count**| Yes   | Yes     | Yes     | Yes     |
| **Reports**      | Yes   | Yes     | Yes     | No      |
| **User Manage**  | Yes   | Yes     | No      | No      |
| **Settings**     | Yes   | Yes     | No      | No      |
| **Audit Logs**   | Yes   | Yes     | Yes      | No      |

*Permissions check logic in backend:*
`[Authorize(Policy = "RequirePurchaseEdit")]` mapped to specific roles or explicitly granted overrides in `Membership.Permissions` JSON.

*Permissions check in frontend:*
`const { hasPermission } = useAuth();`
`<RequirePermission permission="stock_adjust"> ... </RequirePermission>`