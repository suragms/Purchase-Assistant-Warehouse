# Warehouse Purchase Assistant — Current Feature Audit

Date: 2026-10-03 (Asia/Calcutta)

This is the current summary of the master build plan against the application source and supplied reference repository. It reconciles the older historical parity tables with the present implementation; those historical records remain unchanged. Statuses below describe the shipped scope and its evidence, not a claim of full production acceptance.

## Audit basis

- Main app: ASP.NET Core/.NET 10, EF Core/PostgreSQL and React/TypeScript/Vite.
- Reference: Flutter web/PWA and FastAPI/PostgreSQL source under `reference-repo/`; a second 1,340-file copy is present under `backend/reference-repo/`. The reference architecture differs from the target, so its behavior must be adapted rather than copied wholesale.
- Existing source crosswalks: `REFERENCE_FULL_FEATURE_INVENTORY.md`, `REFERENCE_IMPLEMENTATION_STATUS.md`, `REFERENCE_VS_CURRENT_GAP_MATRIX.md`, and `REFERENCE_PARITY_REMAINING_WORK.md`.
- Pre-Phase-2 checkpoint (historical): 388 backend unit/endpoint tests passed; 106 frontend tests passed; frontend production build passed; backend Release build passed with zero warnings/errors; frontend lint exited successfully with 20 warnings. PostgreSQL integration tests skipped because no dedicated `PURCHASE_ASSISTANT_TEST_DATABASE` was configured. Phase 2 results below supersede this checkpoint; no production environment was accessed.

## Feature matrix

| Module | Status | Current implementation | Remaining work or boundary |
|---|---|---|---|
| Authentication and sessions | PARTIAL | Password hashing, login, refresh rotation, logout/logout-all, selected-business tokens and active-membership/session validation are implemented and covered by unit/endpoint tests. | Password recovery has no verified delivery provider; production cookie, key-ring and session-revocation operation remain unverified. |
| RBAC and API authorization | PARTIAL | Owner/Admin/Manager/Staff/SuperAdmin permissions are enforced server-side; current membership permissions replace token claims. Financial response filtering and exports now consistently allow Owner or scoped SuperAdmin. | Complete route-by-route permission and field review, live cross-tenant production-data audit and production security signoff remain. |
| Owner and staff workflows | PARTIAL | User management, permission editing, staff tasks, profile, catalog, purchase and stock actions are connected to APIs. | Full account recovery/session administration and every requested owner/staff permission combination are not complete. |
| Warehouse / branch management | COMPLETE for supported contract | Business is the tenant and owns its catalog, suppliers, purchases, stock and memberships. | There is no separate Warehouse/Branch entity or location-level stock; the selected Business membership identifies the single logical stock pool. | Multi-location inventory is explicitly future scope and requires a separate product/data-model decision. See `WAREHOUSE_MODEL_DECISION.md`. |
| Products, categories and variants | PARTIAL | Catalog CRUD, archive, categories/types, variants, barcodes and item stock fields exist with tenant checks. | Camera scanning, label printing and some legacy/default supplier workflows are absent; full scanner/device workflow is unverified. |
| Suppliers and brokers | PARTIAL | Tenant-scoped CRUD, selection and purchase/report associations exist. Supplier-item links now have tenant-scoped list/add/edit/remove APIs and a management dialog; supplier item code, notes and preferred supplier are editable. | Purchase-price memory/history is not connected to purchase previews, contact ledgers and complete supplier history are incomplete, and supplier performance analytics remain partial. |
| Purchase lifecycle | PARTIAL | Server preview/review token, draft create/edit, status transitions, receiving, payment, delivery states, damage reports and stock commits are implemented. | Full reference charge/discount/unit semantics, separate purchase date/precision, detailed line verification, damage financial resolution, and carrier metadata remain incomplete. |
| System stock and movement ledger | PARTIAL | Transactional adjustments/receipts, available-stock calculations, concurrency handling and append-only application movement history exist. | Transfer/reservation workflows and broader movement filters/reporting are absent or incomplete; DB-admin writes are outside the application append-only guard. |
| Physical stock and reconciliation | PARTIAL | Physical count updates, discrepancy reconciliation and history APIs/UI exist. | Physical-device interaction and final native keyboard/inset checks remain unverified; production-scale reconciliation has not been exercised. |
| Low-stock workflow | PARTIAL | Low/out-of-stock endpoints and views use available stock and reorder levels. | Severity/category/supplier enrichment and all requested follow-up actions are incomplete. |
| Item activity and audit logs | PARTIAL | Stock activity, purchase activity and security audit rows are persisted; damage report create/resolve now writes audit rows transactionally. | Global staff activity UI, broader filtering/retention and database-enforced audit immutability remain incomplete. |
| Dashboard | PARTIAL | Dashboard and operations views query current API/database data. | The full requested KPI set and cross-report drilldowns are not implemented. |
| Reports and analytics | PARTIAL | Spend, inventory valuation, comparison and supplier/item summaries are backed by database queries. | Saved views, item drilldowns, full supplier ledgers, valuation methodology parity and representative load evidence remain. |
| Notifications | PARTIAL | Per-user in-app notifications, read state, dedupe checks and SignalR invalidation are implemented; recent disposable live replay evidence is recorded in the implementation status. | Scheduled producers, all event classes and external push/WhatsApp delivery are not complete or configured. |
| AI purchase assistance | PARTIAL | Provider failover, encrypted provider credentials, usage metadata and reviewed purchase-intent candidates exist; AI cannot commit purchases or set authoritative totals. | External provider credentials are not available here; OCR/vision and voice lack a verified provider contract. |
| Machine learning | BLOCKED | The reference includes limited item analytics text, but no trained model/inference service was located in either codebase. | A validated model artifact, input/output contract, accuracy criteria and safe serving configuration are required. No prediction is fabricated. |
| Search and filtering | PARTIAL | Global search, filters, pagination and bounded query paths exist for core entities. | Search ranking and all requested cross-module filter/sort coverage are incomplete; duplicate review retains performance work. |
| Exports and backups | PARTIAL | XLSX, PDF, ZIP/JSON backups and CSV endpoints exist with tenant scoping and role redaction. | Restore commit intentionally returns 501 pending an approved production-copy rehearsal; export coverage and backup recovery are not fully qualified. |
| Settings and profile | PARTIAL | Personal/business profiles, branding/logo, notification preferences and encrypted provider credential settings exist. | Appearance preferences and complete account/security recovery workflows remain incomplete; external credential delivery is unconfigured. |
| Daily operations | PARTIAL | Checklists, daily usage, snapshots and staff tasks are connected to the backend. | Operational scheduling, broader reporting and live PostgreSQL verification were not run in this turn. |
| Realtime delivery | PARTIAL | SignalR purchase/notification events are tenant/permission scoped; disposable local runtime evidence exists. | Production multi-instance fanout, load and recovery behavior are unverified. |
| PWA and offline behavior | PARTIAL | Manifest, service worker, public asset cache and offline connection-required page exist. | Authenticated shell is not cached; stock/purchase writes require a connection. Installed PWA and physical-device behavior are unverified. |
| Responsive UI | PARTIAL | Desktop/mobile navigation and responsive flows exist; recent browser suite evidence records 200 passes and the long-email Settings overflow fix. | Final Android emulator rerun was blocked by launcher ANR; physical Android/iOS devices and installed-PWA flows are unavailable. |
| API errors and validation | PARTIAL | Request validation, safe exception responses, authorization failures, conflict handling, rate limits and response bounds exist. Damage resolution now maps stale/repeated actions to 409. | A full endpoint-by-endpoint matrix for every requested HTTP status, retry case and production dependency failure remains. |
| Database and migrations | PARTIAL | PostgreSQL migrations, transactional stock/purchase services and composite tenant relationship constraints exist. Damage status concurrency uses the current status value and adds no schema column. | Private clean-database migration and synthetic backup/restore rehearsals passed. Production/staging row audits, representative snapshot restore and rollback rehearsal remain unavailable. |
| Performance | PARTIAL | Paging, query bounds, selected N+1 fixes and indexes exist. | Duplicate review complexity, some unbounded contact queries and representative production query/load plans remain. |
| Security and production configuration | PARTIAL | Production configuration validation checks connection string, persistent key ring, HTTPS origins and non-Windows certificate configuration. Checked-in database credentials were removed; local config now uses User Secrets/environment configuration. | No production credentials, database copy, deployment or restore environment is available, so runtime production qualification is blocked. |
| Automated tests and builds | PASS locally; production qualification partial | Final Phase 2 results: backend 390 unit/endpoint passed; frontend 106 passed; PostgreSQL integration 57 passed on a clean private DB and 57 passed again on the restored DB, zero skips/failures; Playwright desktop/mobile browser 200 passed. Frontend lint has zero warnings; frontend and backend Release builds pass. | Browser tests use API fixtures/mocks and do not certify physical devices or live staged persistence. See the Phase 2 completion record below. |

## Current-turn fixes

- Damage report creation and resolution now write the report, audit entry and reviewer notifications inside one relational transaction. A concurrent/repeated resolution is protected by status concurrency and returns HTTP 409. Added four focused unit/endpoint tests.
- Supplier-item links now support tenant-scoped management of catalog item associations, supplier codes, notes and preferred supplier state. Added coverage for CRUD, duplicate/cross-tenant rejection and preferred supplier replacement; this reuses the existing table and needs no schema migration.
- Financial API responses now retain money for scoped SuperAdmin memberships, matching the existing server permission and export policy. Added role coverage.
- Removed tracked development database credentials. Development startup now requires `ConnectionStrings:DefaultConnection` from User Secrets or the deployment environment.
- PostgreSQL integration tests now require `PURCHASE_ASSISTANT_TEST_DATABASE` pointing to a dedicated database named with the `wa_test_` prefix; otherwise they skip instead of writing to a developer database. Removed the empty template test.

## Phase 2 completion — 2026-10-03

The final automated validation completed locally:

| Validation | Result |
|---|---|
| Backend Release unit/endpoint tests | 390 passed, 0 skipped, 0 failed |
| PostgreSQL integration tests on a fresh migrated private database | 57 passed, 0 skipped, 0 failed |
| PostgreSQL integration tests after synthetic backup/restore | 57 passed, 0 skipped, 0 failed |
| Frontend unit tests | 106 passed across 12 files |
| Desktop/mobile Playwright browser suite | 200 passed; includes responsive overflow checks from 320px through 1920px |
| Frontend lint | Exit 0, 0 warnings (baseline was 20) |
| Frontend production build | Passed; TypeScript and Vite; 2,108 modules; main JS 459.67 KB raw / 135.69 KB gzip |
| Backend Release build | Passed; 0 warnings, 0 errors |
| Production dependency audit | 0 vulnerabilities (`npm audit --omit=dev`) |
| Full frontend dependency audit | 5 high advisories in Tailwind 3's development dependency chain (`braces`, `chokidar`, `fast-glob`, `micromatch`, `tailwindcss`); npm's available fix requires a breaking Tailwind 4 migration. The braces advisory currently has no published patched version. No forced major upgrade was applied. |

The PostgreSQL restore rehearsal used only generated synthetic records in a temporary private cluster: 28 application-table counts and domain invariants matched after restore; migrations were a no-op; app liveness/readiness passed; the unauthenticated protected route returned 401. This qualifies the local procedure, not a production backup or real owner/staff login.

Phase 2 also records the single-stock-pool-per-Business warehouse contract, provider readiness handling for missing credentials, candidate preparation timestamp/advisory context, current AI/OCR/WhatsApp boundaries and model/artifact search. No ML training/inference feature or model artifact was found. Live AI delivery, OCR/WhatsApp, physical device behavior, production recovery, representative load/query plans, full owner/staff authenticated UI-to-database workflows, and route-by-route direct API IDOR coverage remain unverified or open.

**Production status: RED — Not Production Ready.** Automated local checks do not substitute for staged accounts, a representative data restore, a deployment environment, provider credentials or operational sign-off.

## Genuine blockers

- No staging/production database copy, deployment credentials, persistent production Data Protection key ring or release/rollback owner is available; the local restore used generated data only.
- Owner/staff test identities did not complete a full authenticated browser → HTTP API → database → refreshed UI matrix. Direct API cross-tenant IDOR/redaction coverage remains incomplete across all route families.
- Physical Android/iOS devices and installed-PWA runtime are unavailable; the emulator attempt was blocked by launcher ANRs. Browser viewport tests do not prove native keyboard, camera, installation or offline recovery behavior.
- No non-production provider credentials were supplied. AI delivery, quota, account/model validity and output quality remain unverified; OCR/image/voice and WhatsApp delivery are not implemented in current scope.
- No ML artifact/pipeline was found; an external model registry could not be inspected. No prediction is fabricated or enabled.
- Full frontend audit retains five high-severity development-tool advisories. npm reports no patched `braces` release; resolving through Tailwind 4 requires a major CSS-pipeline migration. Production-only dependency audit is clean.
- Representative tenant-scale query plans, load tests, multi-instance SignalR, backup retention/encryption and RTO/RPO remain unverified.
