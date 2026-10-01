# Reference parity remaining work

Reference `main` / `ab63ee73efeb537ca4e11afdccc160450c5356d6`. Completion phase started 2026-10-01. Original certification: PARTIAL; 6 MISSING, 45 PARTIAL, 5 BLOCKED whole areas; 207 backend, 53 frontend and 62 production browser tests passed.

This worklist is extracted from the four existing audit documents, not a repeated repository audit. Issue descriptions, original severity and evidence below are preserved verbatim. Group priority follows the completion request; R01 retains its original P0 severity while being scheduled in P1 financial. Mixed blocked issues retain their feasible subwork. No issue is completed until code/API/security/tenant/validation/tests/regression evidence applies. Existing changes are preserved.

## Original FAIL and scoped PASS gates

| SECURITY | FAIL full-project signoff | Critical reset takeover and scoped security defects fixed/tested; remaining R01–R05/R32 prevent blanket certification |
| TENANT ISOLATION | PASS scoped regression | Explicit tenant references, tracked foreign entities, memberships, AI and report paths tested; historical/DB/production qualification remains R03 |
| PURCHASE DATA INTEGRITY | FAIL full reference parity | Supported preview/weight/state/receipt/concurrency paths pass; full reference header money, precision, payment ledger and damage rules remain R01/R10–R13 |
| STOCK INTEGRITY | PASS scoped regression | StockService authority, append-only application ledger, FK RESTRICT, stale writes/receipt retry/transaction rollback pass on PostgreSQL |
| AI SAFETY | PASS scoped regression | Existing provider architecture preserved; candidates cannot confirm, write stock or set authoritative totals/permissions |
| REGRESSION | FAIL requested full feature coverage | Production-build browser suite **62/62 PASS**; prior development suite **61/62**, with one unresolved intermittent blank page at 390px (R35); missing/blocked features and complete live API/device flows cannot pass the requested full list |
| MOBILE UI | FAIL strict stability signoff | Production suite and new interactions pass at 320, 375, 390, 430; prior development suite had an intermittent blank page at 390px (R35). Physical keyboard/camera/touch/accessibility remain unverified |
| DESKTOP UI | PASS automated viewport scope | 768, 1024, 1280, 1440, 1920; tested rendering/navigation/review/receipt flows |

## P0 — security/data correctness

| ID | File / existing owner | Exact recorded issue | Original severity | Exact recorded evidence | Original next phase |
|---|---|---|---|---|---|
| R02 | backend/PurchaseAssistant.Web/Authorization/OwnerFinancialResultFilter.cs; backend/PurchaseAssistant.Web/Controllers | Financial serialization uses a known-key list; full per-response/role and future export/schema coverage not certified; financial input permissions need comprehensive qualification | P0 | VERIFIED_TEST covers Owner/Manager/Staff purchase + recursive known keys; other responses UNKNOWN runtime | A |
| R03 | backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs; frontend/src/auth/AuthProvider.tsx | Service FK checks and browser context cache/form isolation fixed, but composite tenant FKs and historical invalid-link/production data audit not performed; direct DB writers outside service validation | P0 signoff gap | VERIFIED_CODE global filters/explicit checks; production data UNKNOWN/BLOCKED | A/BB with production data access |
| R04 | backend/PurchaseAssistant.Web/Controllers/AuthController.cs; backend/PurchaseAssistant.Web/Services/CurrentUserService.cs | Logout-all revokes refresh rows; JWT access validity until expiry/session revoke policy not comprehensively tested; session families/bounded refresh hash scans not mapped | P0 signoff gap | VERIFIED_TEST refresh rotation; access-session revocation runtime UNKNOWN | A |
| R32 | backend/PurchaseAssistant.Web/Program.cs; backend/PurchaseAssistant.Infrastructure/Migrations; backend/PurchaseAssistant.Web/appsettings.json | Production key ring/CORS/secrets/migration rehearsal/health readiness/distributed limits and encrypted external credentials unverified; no production deployment performed | P0 signoff gap | VERIFIED_CODE local settings/migrations; production UNKNOWN/BLOCKED | A/AZ/L before deployment |

## P1 — financial/purchase correctness

| ID | File / existing owner | Exact recorded issue | Original severity | Exact recorded evidence | Original next phase |
|---|---|---|---|---|---|
| R01 | backend/PurchaseAssistant.Application/DTOs/Purchase/PurchaseInputLimits.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseForm.tsx | Full financial parity: line discount/tax calculation is implemented and client totals removed; freight/billty/delivered charges, header discount/commission, unit-mode semantics and reference two-decimal precision remain unmapped | P0 | VERIFIED_CODE line_totals_service/compute_totals/decimal_precision vs tested supported subset | A financial correctness, then C |
| R09 | frontend/src/pages/purchases/PurchaseForm.tsx; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; backend/PurchaseAssistant.Web/Controllers/PurchaseController.cs | Per-user wizard autosave/resume missing; legacy draft confirmation has no durable preview/review stamp; draft delete API does not carry external expected version | P1 | VERIFIED_CODE reference TradePurchaseDraft/local WIP; supported target preview tokens tested | C |
| R10 | backend/PurchaseAssistant.Domain/Entities/PurchaseOrder.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchasePayment.tsx | Payment amount/balance/due states/API/UI implemented. Full payment ledger, separate purchase date, mark-paid increment convenience, reference precision and historical paid enum reconciliation remain incomplete | P1 | VERIFIED_TEST owner/tenant/amount/state/PostgreSQL concurrency and browser; VERIFIED_CODE reference richer schema/status/payment helpers | C/E, historical reconciliation before production |
| R11 | backend/PurchaseAssistant.Domain/Entities/PurchaseOrder.cs; backend/PurchaseAssistant.Domain/Entities/PurchaseItem.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Damage/short/missing/returned reports, resolution/status/notes/notification and stock/finance effects missing | P1 | VERIFIED_CODE reference purchase_damage_service/damage routers/model | C |
| R12 | backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Stage verification works, but per-line received/damaged/return verification, short shipment resolution and reference unit setup preflight missing | P1 | VERIFIED_TEST explicit stage gate; VERIFIED_CODE richer reference verification | C |
| R13 | backend/PurchaseAssistant.Domain/Entities/PurchaseOrder.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Dispatch note/carrier/delivery dates/transport metadata/pipeline details not mapped; legacy verified rows without actor/time require explicit operational resolution | P1 | VERIFIED_CODE reference dispatch/arrive/delivery schema vs target state/timestamps | C |
| R20 | backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs; frontend/src/pages/reports/ReportsDashboard.tsx; frontend/src/api/reportApi.ts | Valuation is latest confirmed effective line cost; allocation/accounting methodology parity and unpriced coverage/value UI absent | P1 | VERIFIED_TEST real cost/coverage; reference deeper financial inputs not mapped | A/E |

## P2 — core operational functionality

| ID | File / existing owner | Exact recorded issue | Original severity | Exact recorded evidence | Original next phase |
|---|---|---|---|---|---|
| R06 | backend/PurchaseAssistant.Domain/Entities/MasterDataRelations.cs; backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs; frontend/src/pages/catalog/CatalogVariants.tsx | Variant CRUD/default weight/UI implemented; target retains existing 150-character name limit versus reference 512. Legacy archived-entry variant deletion guard has no equivalent target legacy data; migration/legacy import scope requires qualification | P1 | VERIFIED_TEST CRUD/tenant/owner/conflicts/DB; VERIFIED_CODE catalog.py schemas and archived-entry guard | B/BB for remaining parity |
| R07 | backend/PurchaseAssistant.Infrastructure/Services/SupplierService.cs; backend/PurchaseAssistant.Infrastructure/Services/BrokerService.cs; backend/PurchaseAssistant.Domain/Entities/MasterDataRelations.cs; frontend/src/pages/suppliers/SupplierList.tsx | Item/supplier/broker linking, defaults/rates/ledgers/history incomplete; update normalization/uniqueness/error and field validation breadth not finished | P1 | VERIFIED_CODE contacts/default service/model contracts vs target CRUD only | B |
| R08 | backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs; backend/PurchaseAssistant.Infrastructure/Services/CategoryService.cs; backend/PurchaseAssistant.Infrastructure/Services/CategoryTypeService.cs; frontend/src/pages/catalog/CatalogForm.tsx | Catalog metadata, bulk archive/create, taxonomy summaries, validation lengths/scales, opening stock/unit conversion and reference defaults incomplete | P1 | VERIFIED_CODE reference catalog schemas/handlers; target supported subset | B/D |
| R14 | backend/PurchaseAssistant.Infrastructure/Services/StockService.cs; backend/PurchaseAssistant.Domain/Entities/StockMovement.cs; frontend/src/pages/stock/StockDetail.tsx | Reservation operations/opening stock/audit/dispute sessions, mandatory adjustment reasons and physical-count idempotency breadth incomplete | P1 | VERIFIED_CODE richer reference stock routers/models; target subset tested | D |
| R16 | backend/PurchaseAssistant.Infrastructure/Services/DashboardService.cs; frontend/src/pages/Dashboard.tsx | Owner command center, goals/staff task summaries/delivery pipeline breadth missing | P2 | VERIFIED_CODE owner dashboard/business goals/staff tasks | E |
| R17 | backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs; backend/PurchaseAssistant.Infrastructure/Services/DashboardService.cs | Daily usage/checklists/snapshots/staff task operational workflows missing | P2 | VERIFIED_CODE reference operations/owner_ops and Flutter pages | E |
| R18 | backend/PurchaseAssistant.Infrastructure/Services/NotificationService.cs; backend/PurchaseAssistant.Web/Program.cs; frontend/src/pages/NotificationsPage.tsx | Scheduled emitters/BackgroundService, dedupe unique constraint/concurrent notification creation and operational role subscriptions incomplete | P2 | VERIFIED_CODE target list/read/create helper; no active scheduled target producer found | E |
| R19 | backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs; frontend/src/pages/reports/ReportsDashboard.tsx | Saved views/BI/item drilldowns/supplier ledger analytics and complete created-vs-business-date comparison methodology remain incomplete | P2 | VERIFIED_CODE reference reports_trade/report_views/contact ledgers | E |
| R21 | backend/PurchaseAssistant.Web/Controllers/ReportsController.cs; backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs | Authorized tenant stock XLSX/monthly PDF/ZIP/JSON export absent | P2 | VERIFIED_CODE reference exports/export_files | E |
| R23 | backend/PurchaseAssistant.Web/Program.cs; frontend/src/router/index.tsx; frontend/src/layouts/AppShell.tsx | Business profile/settings/provider credential editor absent; encrypted external credential store/rotation not configured | P2 | VERIFIED_CODE reference provider_credentials/settings; target menu has no working settings route | E |
| R33 | backend/PurchaseAssistant.Domain/Entities/SecurityAuditLog.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Activity reuses generic audit table and latest 100; complete retention/filter/global staff activity UI, verifier display name and DB audit append-only controls incomplete | P2 | VERIFIED_TEST purchase events; reference lifecycle/activity breadth richer | C/E/J |
| R34 | backend/PurchaseAssistant.UnitTests; backend/PurchaseAssistant.IntegrationTests; frontend/src/tests; reference:backend/tests | Reference tests not run; not every new hardening branch has dedicated HTTP/load/network coverage (e.g. auth rate limits and complete financial schema coverage); no full contract parity suite | P1 signoff gap | Actual target counts above; reference execution UNKNOWN | L |
| R35 | frontend/e2e/phase3.spec.ts; frontend/src/auth/AuthProvider.tsx; frontend/playwright.config.ts | Prior development suite rendered a blank page once at 390px; isolated case passed with trace, root cause remains unknown. Production suite subsequently passed all 62 cases; development failure is still unexplained | P1 unresolved regression | VERIFIED_TEST development 61/62 failure, isolated 1/1 pass; failed screenshot/context retained externally; no invented root cause | L reproduce and diagnose |

## P3 — UX/performance/enhancement

| ID | File / existing owner | Exact recorded issue | Original severity | Exact recorded evidence | Original next phase |
|---|---|---|---|---|---|
| R15 | frontend/src/pages/catalog/BarcodeManager.tsx; backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs | Camera scanner state machine, permission handling, scan history/label printing/unknown barcode quick-create/cache timing not mapped or physically tested | P2 | VERIFIED_CODE reference barcode controllers/cache; no target camera decoder found | D/I |
| R28 | backend/PurchaseAssistant.Web/Program.cs; frontend/src/api/apiClient.ts; frontend/src/lib/queryKeys.ts | No target SignalR hub/event client despite reference business SSE feed and README claim | P2 | VERIFIED_CODE reference realtime.py; target source absence | J after event/tenant contract |
| R29 | frontend/package.json; frontend/public; frontend/src/main.tsx | No active target PWA manifest/service worker/plugin; offline mutation replay safety unverified | P2 | DOCUMENTATION_CLAIM target PWA; reference web assets exist | I/J, no automatic offline writes |
| R30 | backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs; backend/PurchaseAssistant.Infrastructure/Services/SupplierService.cs; backend/PurchaseAssistant.Infrastructure/Services/BrokerService.cs | Duplicate review still O(n²); contact lists unbounded; no representative dataset timings/DB plans; backend ETags/cache absent | P2 | VERIFIED_CODE algorithms/list contracts; production load UNKNOWN | J |
| R31 | frontend/e2e/phase3.spec.ts; frontend/src/layouts/AppShell.tsx; frontend/src/auth/AuthProvider.tsx | Mocked browser tests do not cover all live CRUD/role states, dialogs/nav/sticky headers/accessibility/touch 48px/physical keyboard/camera; missing flows cannot pass full regression | P1 signoff gap | VERIFIED_RUNTIME nine viewport scopes; physical/live flows UNKNOWN | I/L |

## BLOCKED — external credentials/infrastructure/approval or unverified source contract

| ID | File / existing owner | Exact recorded issue | Original severity | Exact recorded evidence | Original next phase |
|---|---|---|---|---|---|
| R05 | backend/PurchaseAssistant.Web/Controllers/AuthController.cs; frontend/src/pages/auth/Login.tsx | Secure password recovery unavailable; no safe token issuer, single-use store, verified delivery or recovery browser flow; registration/Google auth absent | P1 / BLOCKED recovery | VERIFIED_CODE unsafe path removed; reference PasswordResetToken exists; external delivery contract missing | A then B after configuration |
| R22 | backend/PurchaseAssistant.Web/Program.cs; reference:backend/app/routers/exports.py | Backup export/dry-run runner absent; restore commit intentionally 501 in reference pending production-copy approval | P2 / BLOCKED commit | VERIFIED_CODE restore_commit; no target runner | E, retain commit deferred |
| R24 | backend/PurchaseAssistant.Infrastructure/Services/AI; reference:backend/app/routers/media.py; reference:TASKS.md | OCR/vision blocked: reference is pasted/plain-text extraction, not actual image vision; no target provider/upload contract | P3 / BLOCKED | VERIFIED_CODE UTF-8 base64 decode and fixed confidence; deferred TASKS | G only after supported contract |
| R25 | backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs; reference:backend/app/models/unit_intelligence.py | OCR alias ORM/bootstrap exists; active correction-learning service/API/UI not located; correction-events SQL alone is not a learning feature | P3 / BLOCKED | VERIFIED_CODE ORM/DDL, UNKNOWN active learning | G, retain deferred |
| R26 | backend/PurchaseAssistant.Web/Program.cs; reference:backend/app/services/whatsapp_po_delivery.py | WhatsApp credentials/infrastructure missing; reference webhook/signature/status callback not found; auto-send behavior conflicts with explicit action requirement | P3 / BLOCKED | VERIFIED_CODE Graph v19 PDF/3 retry/60s/manual fallback; TASKS deferred | H explicit action only after configuration |
| R27 | backend/PurchaseAssistant.Infrastructure/Services/AI; reference:backend/app/config.py | Voice processing route/provider/UI absent; flag does not verify feature | P3 / BLOCKED | DOCUMENTATION_CLAIM/config flag only | G/H optional contract first |

## Whole-area coverage carried forward

The following exact final area rows remain active. Original pre-edit BROKEN rows are historical baseline findings already linked to F01–F35; they are not duplicate new issues. This phase verifies existing fixes before changing their owners.

| Area | Original final status | Exact recorded gap |
|---|---|---|
| A. Authentication | PARTIAL | Live account/membership checks, atomic refresh metadata, selected-business preservation, invalid-cookie safety and server logout/cookie-format revocation fixed. Secure recovery, registration/Google and access-session family revocation remain. |
| B. Authorization / RBAC | PARTIAL | Server claims rehydrated, owner monetary serialization, per-domain search and lifecycle permissions fixed. Full endpoint/role and credential-backed runtime coverage remains open. |
| C. Business / Tenant Management | PARTIAL | Server-confirmed selector and cache/form isolation added; tenant FK validation/shared-account protections fixed. Business profile and historical composite-FK/data audit remain. |
| D. Dashboard | PARTIAL | Operational/financial bundles exist; no owner command center, goals or staff task summary. |
| E. Catalog | PARTIAL | Tenant references, tracked access, concurrency errors, history deletion, pagination and duplicate pair correctness fixed. Broader reference metadata/bulk flows remain. |
| F. Categories | PARTIAL | Tenant validation and aggregate count query fixed; full taxonomy summary/bulk flows remain. |
| G. Types | PARTIAL | Tenant category linkage and aggregate count query fixed; summary/bulk behaviors remain. |
| H. Variants | PARTIAL | Tenant list/create/update/delete/default kg/unit, owner delete, normalized unique index, version conflicts and responsive UI now work with tests. Existing 150-character name limit differs from reference 512; legacy import/dependency scope remains R06. |
| I. Suppliers | PARTIAL | Tracked foreign supplier access fixed. Links/default prices/history/ledger remain incomplete. |
| J. Brokers | PARTIAL | Tracked foreign broker access and count N+1 fixed. Broker links/defaults/history remain incomplete. |
| K. Search | PARTIAL | Catalog policy and supplier/broker permission filtering enforced. Ranking/performance/full role runtime coverage remains open. |
| L. Barcode | PARTIAL | Tenant unique index and manual lookup/assignment exist; no camera decoder or reference scanner state machine. |
| M. Stock | PARTIAL | Quantity bounds/transaction disposal/409 rules and receipt tests pass. No production data/load test or reservation operation implementation. |
| N. Physical Stock | PARTIAL | Negative/overprecision counts rejected. Physical-device UI and explicit count idempotency keys remain unverified. |
| O. System Stock | PARTIAL | Existing StockService authority preserved. Opening-stock reference workflows/reservation management are not fully mapped. |
| P. Stock Movements | PARTIAL | Application append-only guard, purchase provenance and catalog FK RESTRICT implemented. DB administrator/raw-SQL writes are outside this guard. |
| Q. Stock Adjustments | PARTIAL | Nonzero/bounded adjustments and transaction lifetime fixed. Mandatory reasons and full audit filter UX remain. |
| R. Low Stock | PARTIAL | Available-stock calculation aligned; urgency/reorder/supplier enrichment remains. |
| S. Purchases | PARTIAL | Preview, exact weight fallback, computed line discount/tax, review token, xmin, Verified gate and payment now implemented. Full header money/precision parity, detailed verification and damage remain. |
| T. Purchase Draft | PARTIAL | Draft create/edit now requires reviewed preview externally. Per-user wizard autosave/resume remains absent. |
| U. Purchase Preview | PARTIAL | Current contract calculates on server, requires fresh user-bound review token, rejects expired/changed snapshots, and passes tests. Full reference financial inputs remain missing. |
| V. Purchase Confirmation | PARTIAL | Explicit confirmation/state whitelist retained; fresh draft saves require reviewed preview. Legacy draft confirmation has no persisted review stamp; detailed reference validation remains incomplete. |
| W. Purchase Lifecycle | PARTIAL | Transitions, Arrived/Verified UI, actor/time and existing audit log reuse implemented. Full per-line verification/damage semantics remain partial. |
| X. Purchase Receiving | PARTIAL | Verified prerequisite, bounded unique receipt batch, no over-receipt, xmin retry protection, transactional StockService and partial balance tested. Short/damaged/returned quantity workflow absent. |
| Y. Purchase Verification | PARTIAL | Explicit permission-gated verification records actor/time before stock commit. Detailed per-line received/damaged/returned verification remains missing. |
| Z. Payment Lifecycle | PARTIAL | Owner cumulative payment API/UI, paid/balance, terms, due-soon/overdue and versioned writes now work without stock/state side effects. Payment ledger, separate purchase date/precision and historical reconciliation remain R10. |
| AA. Delivery Lifecycle | PARTIAL | Explicit dispatch/arrive/verify/receive UI works. Carrier/delivery notes/date/pipeline reference workflows remain missing. |
| AB. Damage Management | MISSING | Reference damage reports/resolution are implemented; target has no damage entity/service/UI. |
| AC. Daily Operations | MISSING | Reference checklists, daily usage, snapshots and staff tasks have no target equivalents. |
| AD. Notifications | PARTIAL | Selected business required and pagination capped. Scheduled notification emission/dedupe uniqueness/role subscription breadth remains incomplete. |
| AE. Reports | PARTIAL | Draft spend excluded, owner money stripped, UTC/date validation and stable frontend bounds fixed. Saved views/export/drilldowns remain incomplete. |
| AF. Analytics | PARTIAL | Summaries exist; saved report views/item drilldowns/BI breadth absent. |
| AG. Period Comparison | PARTIAL | Server date validation and PostgreSQL UTC compatibility fixed. Complete boundary and historical comparative data coverage remains open. |
| AH. Inventory Valuation | PARTIAL | Invented rate removed; confirmed effective line cost and unpriced-stock coverage returned. Allocation/methodology parity and coverage UI remain partial. |
| AI. Spend Analytics | PARTIAL | Draft/cancelled purchases excluded consistently. Full reference financial charge/date methodology parity remains partial. |
| AJ. Supplier Analytics | PARTIAL | Supplier summary exists; ledgers/item details/discount/commission semantics missing. |
| AK. Export | MISSING | Reference stock XLSX, monthly PDF, ZIP/JSON downloads implemented; no target export endpoints. |
| AL. Backup / Restore | BLOCKED | Export and dry-run are reference code; restore commit explicitly returns 501 pending production-copy approval. No target backup runner. |
| AM. AI | PARTIAL | Target provider abstraction/failover is verified and preserved; reference llm_failover code remains but intent router is absent. |
| AN. Purchase Intent Parsing | PARTIAL | Target supports read-only catalog/supplier matching and user review; reference active natural-language intent UI/API not located. |
| AO. OCR / Vision | BLOCKED | Reference media/ocr parses pasted/plain text; image bytes are decoded as UTF-8, no actual Google vision call. TASKS defers OCR. |
| AP. OCR Learning / Corrections | BLOCKED | OcrItemAlias ORM/bootstrap and correction-events supplemental SQL exist; active correction service/API/frontend not located. Do not fabricate learning feature. |
| AQ. WhatsApp | BLOCKED | Graph v19 media+document send/resend and delivery logs exist; webhook/signature/status callback not found. TASKS defers expansion; credentials missing. |
| AR. Voice | BLOCKED | enable_voice flag exists; no voice processing route/provider/UI located. Documentation/config claim only. |
| AS. Settings | MISSING | Reference business profile, encrypted provider credentials, backups/settings screens exist; target menu lacks working settings route. |
| AT. User Management | PARTIAL | Privilege escalation/shared account/self role changes guarded; new default permissions fixed. Field validation, explicit per-user permission editor/session admin UI remain incomplete. |
| AU. Audit / Activity | PARTIAL | Existing security audit log now records draft/status/receipt actions with an activity API/UI. Global activity UI/immutable security-log DB enforcement and retention remain open. |
| AV. Performance | PARTIAL | Category/type/broker count N+1 and per-line purchase catalog lookup fixed; bounded catalog/stock/purchase/notification pages. Duplicate search remains quadratic; contact lists unbounded. |
| AW. Caching | PARTIAL | Scoped session cache clearing/form reset and same-scope token preservation tested; backend Redis/ETag generation absent. |
| AX. Realtime / SignalR-equivalent behavior | MISSING | Reference business SSE queues/recent feed exist; target has no hub registration or SignalR client despite README claims. |
| AY. PWA / Offline-related behavior | MISSING | Reference manifest/service worker exists; target has no manifest, worker or PWA plugin. Offline mutations not proven safe in either project. |
| AZ. Deployment | PARTIAL | Target local build exists; production hosting/health readiness/secret and migration operation contract incomplete. |
| BA. Security | PARTIAL | Critical reset takeover closed; live permissions/redaction/rate limits/CORS/safe exception responses fixed. Complete production security signoff has not been performed. |
| BB. Database / migrations | PARTIAL | Six reviewed local additive/restrictive migrations applied; xmin uses PostgreSQL system column. Composite tenant FK/historical data/production migration validation remains open. |
| BC. Testing | PARTIAL | 184 unit +23 real PostgreSQL integration +53 frontend tests pass. Production browser suite 62/62 PASS; prior development suite 61/62, isolated failed case then passed. Reference/full live API/device qualification remains. |
| BD. UX / responsive behavior | PARTIAL | New review/receipt/variant/archive/payment/auth interactions pass at nine widths. One prior intermittent development blank-page regression at 390px remains R35; production suite 62/62 PASS; device/a11y coverage incomplete. |

## Blocked activation requirements

| Area | Exact reason | Required dependency | Already prepared | Activation remaining |
|---|---|---|---|---|
| AL backup/restore | Reference restore commit returns 501 pending production-copy approval | Explicit approval and a validated isolated production-copy rehearsal | Located export/dry-run/restore source, no active target runner | Implement/test safe export and dry-run separately; keep commit inactive |
| AO OCR/vision | Plain-text/base64 UTF-8 heuristics do not establish an image provider contract; reference TASKS defers OCR | Supported provider/upload contract and credentials | Existing IAIProvider/IAIRoutingService candidate safety boundaries | Do not activate vision until extraction/security/review contract is verified |
| AP OCR correction learning | Alias ORM/DDL exists but active learning API/service/UI not located | Verified correction workflow/source contract | Located alias ORM and supplemental SQL evidence | Keep learning inactive; do not invent model updates |
| AQ WhatsApp | Credentials/infrastructure absent; webhook/signature/status callbacks not found; expansion deferred | Provider configuration/credentials and explicit delivery action | Located Graph v19 PDF delivery/retry/manual-fallback source | Do not activate automatic PO sends or invented webhooks |
| AR voice | Config flag only; actual route/provider/UI absent | Verified behavior/provider contract | Source absence documented | Keep inactive |

Password recovery (R05) additionally requires a secure issuer, single-use persistence and verified delivery configuration. Production portions of R03/R32 require production access/rehearsal; local code hardening remains actionable.

## Reference dependency and cleanup register

Reference checkout: `C:\Users\SURAG\.codex\reference-repositories\PurchaseAssiastant-20261001` — RETAINED during implementation because exact financial/damage/security source evidence is still required. No GitHub repository deletion is authorized. Before any local removal, exact paths, runtime imports, scripts/config, documentation, CI and project/package references will be checked. Audit documents, source index, tests, migrations and retained regression evidence are protected.

Current application must run independently; reference source is for development/audit only. Production database: NOT TOUCHED. Production deployment: NOT PERFORMED.

## Completion evidence

No issues removed yet. P0 inspection in progress. Original results remain historical until this phase executes its checks.
