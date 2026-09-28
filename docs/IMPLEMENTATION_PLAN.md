# Implementation Plan (Incremental Execution)

The system is built strictly over 17 mapped phases.

1.  **Architecture + Database Design:** (Current) Scaffolding EF Core, Vite, solution setup.
2.  **Authentication & Users:** JWT, Refresh tokens, User entities, Memberships, AuthGuard.
3.  **Catalog Engine:** Items, Categories, Types, Variants, deduplication check capability.
4.  **Contacts:** Suppliers, Brokers, Global Search base.
5.  **Stock Engine Core:** Physical/System stock math, Concurrency testing, Auditing tables.
6.  **Purchase Engine Part 1:** Multi-step wizard UI, Idempotency, Draft support.
7.  **Purchase Engine Part 2:** Delivery Lifecycle, Staff Verification, Atomic Stock Commit.
8.  **Dashboard:** KPI Cards, Cached aggregations, Chart scaffolding.
9.  **Reporting:** High-performance analytical queries, pagination, financial reports.
10. **Notifications & Realtime:** SignalR hubs, Tanstack cache invalidation hooks.
11. **Operations:** Staff checklists, Daily Usage snapshots.
12. **System Exports:** Provider credentials (AI), File exports handling.
13. **Responsive Polish:** Mobile navigation rail, Touch targets, Modals vs Bottom Sheets.
14. **Performance Optimization:** Redundant API call pruning, `AsNoTracking`, index tuning.
15. **Security Finalization:** Content Security Policy, CSRF layers, Tenant isolation audits.
16. **Automated Testing:** xUnit / Playwright configurations.
17. **Deployment Configs:** Dockerizing, environment variables setup.

Each phase must be completed, compiled, UI/Backend linked, and tested before moving to the next. No scaffolding unlinked components.