# Owner, Staff and Tenant Security Matrix — 2026-10-03

## How to read this matrix

“Browser coverage” refers to Playwright route/layout/state checks. Those tests stub API traffic and verify UI permission behavior; they do not prove persistence or server authorization. “Backend coverage” refers to unit/endpoint tests; tests using mocked services are not database persistence checks. PostgreSQL integration tests use a private database and unique tenant fixtures. “Partial” is deliberately used where a full frontend → authenticated HTTP request → authorization → business logic → database → refreshed UI trace was not executed.

## Current model

The user’s assigned warehouse is the selected Business membership; there is one stock pool per Business. Warehouse switch is therefore the existing selected-business token switch. Separate warehouse-ID denial tests do not apply to the current schema. Cross-business IDs remain an isolation boundary and must be rejected regardless of hidden navigation.

## Owner workflow

| Operation | Frontend / route evidence | API / authorization evidence | Database / audit evidence | Result |
|---|---|---|---|---|
| Login, dashboard, business selection | Existing desktop/mobile navigation and session/browser specs; current browser run result is recorded in the audit | Auth controller, refresh rotation, selected-business token, active membership and permissions are unit/endpoint covered | PostgreSQL dashboard tests verify selected Business aggregation and another Business isolation | Partial; no successful staged owner login |
| Warehouse / Business | Business profile and selected-business UI; no separate Warehouse CRUD exists by decision | Membership and current `businessId` scope protect Business routes | Same BusinessId is used for stock and purchase records | Contract complete for one stock pool; no location-transfer workflow in this model |
| Products / suppliers | Routes and dialogs exist; supplier item-link management added; e2e route and state tests stub API calls | Catalog/supplier policies and supplier-item tenant CRUD tests; cross-tenant association/duplicate cases covered | Stock and purchase PostgreSQL tests seed current-business catalog/supplier rows; no complete owner UI persistence trace | Partial |
| Purchases / stock | Browser specs cover purchase review, invalid/duplicate submission guards, variants and receiving UI with mocked APIs | Purchase/commit/verify/stock permissions are enforced in controller policies and service checks | 57/57 PostgreSQL tests cover purchase transactions, stock movement, rollback, concurrency and dashboard tenant isolation | Strong workflow-service coverage; not full real-browser-to-DB owner signoff |
| Reports / financials / exports | Report and CSV routes render in UI/browser role checks; API calls are stubbed | Report/financial role filtering and owner/scoped SuperAdmin export policies have endpoint tests; OwnerFinancialResultFilter is active | Dashboard and report query tests cover selected aggregation; no representative restored financial dataset comparison | Partial; staged exact-total and timezone matrix outstanding |
| Staff, roles and permissions | User-management and permissions screens render; actual persistence not exercised by browser suite | `UserServiceRBACStaffTests`, permission handler, operations/settings endpoint and parity endpoint tests cover selected role/permission changes and denial | No full create → login → permission edit → relogin scenario in a staged DB | Partial; full owner CRUD, disable/reactivate, session revocation and audit checks outstanding |
| Settings and audit logs | Settings/backup/help route browser tests cover visibility and errors using mocks | Owner-only settings, encrypted credential status/update, backup/export routes have endpoint tests | Credential changes write audit records; transactional damage audit integration has PostgreSQL coverage | Partial; production Data Protection key-ring and retention are unverified |
| AI / ML | Purchase assistant review/error/manual-fallback UI is unit-tested; no ML UI is present | AI endpoint requires purchase-create permission, AI limiter and current-business parsing context; missing provider configuration is skipped before network calls | No AI write dependencies; AI usage metadata is tenant-scoped; external provider not invoked in this run | AI code path covered with mocks; external service blocked; ML not implemented |

## Staff workflow and negative cases

| Area | Evidence exercised | Result / gap |
|---|---|---|
| Navigation and allowed/hidden routes | Mobile/desktop role fixtures, membership-permission override and direct browser route guards | Browser-only; backend API authorization is separately enforced |
| Staff tasks and operational actions | Operations/settings endpoint tests and browser task assignment/rejection UI tests | Partial; no actual authenticated staff session in a staging browser |
| Catalog, supplier and purchase writes | Controller/service policies and domain validation; purchase/stock services exercise tenant-scoped PostgreSQL data | Need a complete direct-HTTP Staff positive/negative route matrix across all actions |
| Stock physical/system adjustment and damage | Stock policies; PostgreSQL stock arithmetic/transaction/concurrency tests; damage create/resolve audit and stale conflict changes | Selected paths verified; exact staff permission combination and UI refresh remain incomplete |
| Finance, user administration and owner settings | Owner-only/scoped financial filter and role denial tests; hidden frontend routes | Selected denial/redaction tests exist; every direct API route/object-ID substitution is not covered |
| Cross-tenant IDs | Dashboard, stock, purchase and historical preview tests use distinct Business IDs; supplier-item cross-tenant validation is tested | No blanket proof across every controller/report/export/notification/settings route. H1 remains open until a route-by-route direct API matrix passes. |

## Security and production boundary

- Server-side authorization remains the security boundary; frontend route hiding is convenience only.
- Current membership permissions are loaded server-side and replace stale token permissions.
- Business-scoped EF filters and composite tenant relationships provide defense in depth for core entities. Every new query/ID lookup still needs a tenant check.
- AI parsing receives only the authorized current Business context and has no purchase or stock write dependency.
- Unauthenticated database restore smoke check returned 401 after restore; this is not a successful Owner/Staff login test.
- No cross-tenant production data, staging accounts or production deployment were available. No claim is made that every route is security-certified.

## Required closure evidence

To close H1/H2, seed two Businesses and Owner/Staff memberships in a staging-like disposable PostgreSQL DB; exercise each listed workflow through the actual frontend and API; issue direct requests with an alternate Business’s IDs; assert correct `401/403/404` behavior and safe response fields; compare persisted stock/financial/audit rows before and after denied attempts; and verify the owning browser refreshes after allowed edits. Record route, permission, tenant, expected/actual response and database delta. Do not substitute UI-only tests or mocked provider results for this evidence.
