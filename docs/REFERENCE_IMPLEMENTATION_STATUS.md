# Reference implementation status

Audit date: 2026-10-01, Asia/Calcutta. **Overall result: PARTIAL. Full reference parity and production signoff are not complete.** Passing builds and the scoped regression suite do not resolve the remaining issues below.

## Reference repository and evidence scope

- Repository: https://github.com/ANANDU-2000/PurchaseAssiastant.git
- Branch/commit: `main` / `ab63ee73efeb537ca4e11afdccc160450c5356d6`.
- Read-only clone: `C:\Users\SURAG\.codex\reference-repositories\PurchaseAssiastant-20261001`, outside the implementation workspace.
- Initial extraction and the first three audit documents preceded application edits. Existing uncommitted AI/frontend work was retained; no reset, commit, push or production deployment was performed.
- Source index: 1,353 tracked text files indexed/searched, 229 API handler definitions and 52 Python model classes (including the base class). Indexing is not proof that every line was manually reviewed or that every feature works.
- `VERIFIED_CODE`: the 229 handler contracts, located service/model implementations and frontend constructor references in the inventory. This count is API contracts, not 229 fully tested product features.
- `VERIFIED_TEST`: target tests listed below were executed. Reference tests were located and indexed, **not executed**; reference runtime has not been certified.
- `VERIFIED_RUNTIME`: target browser rendering/interactions under mocked API contracts, and actual local PostgreSQL operations in integration tests. No live reference server, production host, external AI, OCR, WhatsApp or physical-device runtime was verified.
- `DOCUMENTATION_CLAIM`: AGENTS/README deployment, OCR/cloud provider and offline claims are separated from located implementations. Reference `PLAN.md`, `docs/ai/` and linked `docs/TEST_RESULTS.md` are absent at the pinned commit. Migration index head 062 is stale relative to actual files through 070.

The inventory contains the 56 requested areas A–BD, all extracted route signatures, response schema names, dependency/role declarations, handler error branches, service calls, model tables, source frontend references and evidence limitations. The matrix preserves the initial baseline and adds final area status. No absent screen/contract is treated as verified runtime.

## Count definitions and final report

Counts are deliberately non-additive: verified means extracted reference API contracts; implemented means the nine bounded additions below; fixed means the 35 defect groups below; missing/blocked mean whole area statuses in the 56-area matrix. Partial areas can contain additional missing subfeatures.

| Requested report field | Result | Scope / reason |
|---|---|---|
| REFERENCE STATUS | analyzed source; runtime unverified | Read-only source extraction, pinned commit |
| FEATURES VERIFIED | 229 | `VERIFIED_CODE` API contracts; **0** reference runtime/test signoffs |
| FEATURES IMPLEMENTED | 9 | I01–I09 below; full financial/reference category parity remains partial |
| FEATURES FIXED | 35 | F01–F35 below |
| FEATURES STILL MISSING | 6 whole areas | AB damage, AC daily operations, AK export, AS settings, AX realtime, AY PWA; **45 areas remain PARTIAL** |
| FEATURES BLOCKED | 5 whole areas | AL restore, AO OCR/vision, AP OCR correction learning, AQ WhatsApp, AR voice; secure password recovery is also blocked inside A |
| SECURITY | FAIL full-project signoff | Critical reset takeover and scoped security defects fixed/tested; remaining R01–R05/R32 prevent blanket certification |
| TENANT ISOLATION | PASS scoped regression | Explicit tenant references, tracked foreign entities, memberships, AI and report paths tested; historical/DB/production qualification remains R03 |
| PURCHASE DATA INTEGRITY | FAIL full reference parity | Supported preview/weight/state/receipt/concurrency paths pass; full reference header money, precision, payment ledger and damage rules remain R01/R10–R13 |
| STOCK INTEGRITY | PASS scoped regression | StockService authority, append-only application ledger, FK RESTRICT, stale writes/receipt retry/transaction rollback pass on PostgreSQL |
| AI SAFETY | PASS scoped regression | Existing provider architecture preserved; candidates cannot confirm, write stock or set authoritative totals/permissions |
| BACKEND BUILD | PASS | 0 warnings, 0 errors |
| BACKEND TESTS | 207/207 | 184 unit + 23 PostgreSQL integration; no skips |
| FRONTEND BUILD | PASS | TypeScript + Vite production build |
| FRONTEND TESTS | 53/53 | 5 test files |
| REGRESSION | FAIL requested full feature coverage | Production-build browser suite **62/62 PASS**; prior development suite **61/62**, with one unresolved intermittent blank page at 390px (R35); missing/blocked features and complete live API/device flows cannot pass the requested full list |
| MOBILE UI | FAIL strict stability signoff | Production suite and new interactions pass at 320, 375, 390, 430; prior development suite had an intermittent blank page at 390px (R35). Physical keyboard/camera/touch/accessibility remain unverified |
| DESKTOP UI | PASS automated viewport scope | 768, 1024, 1280, 1440, 1920; tested rendering/navigation/review/receipt flows |
| DOCUMENTATION | PASS produced artifacts | Four requested documents plus source index; limitations explicitly retained |

No result in this table certifies production security, every API, every role, every device or full product parity.

## Current architecture and preserved functionality

The .NET 10 Domain/Application/Infrastructure/Contracts/Web layers, EF Core/Npgsql, JWT/refresh rotation, permission handlers, StockService, existing catalog/contact/report services and React/TypeScript/Tailwind/TanStack Query UI remain. No alternate purchase, stock, AI provider, controller, table or frontend page architecture was introduced.

Verified existing production stock assignments occur in StockService, not PurchaseService. DTO projections of CurrentStock are reads. The existing AI provider abstraction, routing/failover and read-only purchase intent matching remain in their original owners. Provider outputs populate candidates, quantities remain editable proposals, and AI financial/state/business/permission fields are discarded before purchase submission.

Catalog variants now have tenant-scoped list/create/update/delete, optional default weight, normalized database uniqueness, version checks and UI. The target retains its existing 150-character limit; the reference permits 512 (R06). Supplier item prices/links and broker supplier links still lack complete usable workflows. Reference CatalogVariant CRUD is real code (`catalog.py:3206` onward).

## Implemented bounded additions

| ID | Addition | Existing owners extended | Evidence |
|---|---|---|---|
| I01 | Nonmutating server purchase preview, user review step and expiring review token | IPurchaseService, PurchaseService, PurchaseController, purchaseApi, PurchaseForm | Reference `trade_preview_service.py`, trade router preview/validate; target service/HTTP/frontend/browser tests |
| I02 | Optional paired weight pricing with exact reference consistency fallback | PurchaseItem, existing DTO/config/calculator/form | Reference `line_totals_service.line_gross_base`; three parity and four invalid-weight unit cases |
| I03 | Explicit Arrived → Verified step, actor/time and verified prerequisite for stock receipts | PurchaseOrder/PurchaseService, existing status API and detail UI | Reference `_VERIFY_FROM`, `_COMMIT_FROM`, `verify_trade_purchase_delivery`; policy, no-stock verification and PostgreSQL lifecycle tests |
| I04 | Purchase activity API/UI using existing SecurityAuditLogs | Existing purchase service/controller/API/detail and audit table | Reference lifecycle/staff logs; target draft/status/receipt events and tenant-scoped latest-100 read tests |
| I05 | Unpriced-stock coverage alongside sourced inventory valuation | Existing ReportService, StockAnalyticsDto and API type | Target real/unpriced valuation tests; no fabricated fallback cost |
| I06 | Backend line discount and tax rates; computed tax/subtotal/total only | PurchaseInputLimits, existing purchase DTO/entity/config/service/form | Reference line_money; calculation parity/invalid rates/malicious totals, PostgreSQL and frontend tests |
| I07 | Variant list/create/update/delete and optional default weight | Existing CatalogVariant, ICatalogService, CatalogService/Controller/API/detail component | Reference catalog.py:3206–3343; HTTP permissions/conflicts, tenant tests, PostgreSQL duplicate/concurrency and nine viewport interactions |
| I08 | Explicit owner payment recording, cumulative paid/remaining, terms and due states | Existing PurchaseOrder/PaymentState/service/controller/API/detail | Reference patch_trade_purchase_payment/compute_status; owner, bounds, tenant, replay/concurrency, no-stock and frontend/browser tests |
| I09 | Server-confirmed business selector | Existing auth select-business API, AppShell and AuthProvider | Reference me/business membership; existing backend membership tests, nine browser switch/failure scenarios; cache isolation tests |

Review tokens are bound to business/user/all submitted supported inputs, last 20 minutes, and become unusable after a form edit. They authorize review of a draft submission; they do not authorize AI confirmation. Reusing an unchanged create request also meets the existing order-number uniqueness check/constraint. Line discount and tax rates now share the server calculator in preview/save. Reference two-decimal precision and header-charge parity are **not** claimed.

## Fixed defect groups

| ID | Defect fixed | Main file/owner | Test or source evidence |
|---|---|---|---|
| F01 | Anonymous reset endpoint ignored token and could take over an account | backend/PurchaseAssistant.Web/Controllers/AuthController.cs | HTTP empty/forged token tests prove unchanged password/session; recovery now unavailable, not falsely sent |
| F02 | Blocked accounts/deleted memberships remained authorized | backend/PurchaseAssistant.Web/Services/CurrentUserService.cs | Live-session and deleted-membership HTTP tests |
| F03 | Stale/forged JWT roles/permissions survived server changes | CurrentUserService / PermissionAuthorizationHandler | Membership overrides claim tests; malformed stored permissions fail closed |
| F04 | Random refresh cookie with victim user ID could revoke victim sessions | AuthController | Victim-session preservation HTTP test |
| F05 | Concurrent refresh rotation could produce multiple replacements | AuthConfigurations / AuthController | RevokedAt concurrency plus real PostgreSQL two-context atomic-rotation test |
| F06 | Refresh lost selected business | AuthController | Selected-business/rotation HTTP test; selected membership validated |
| F07 | Nonowners received money in API bodies | OwnerFinancialResultFilter / existing money displays | Owner/Manager/Staff HTTP serialization and recursive redaction tests |
| F08 | Foreign category/type IDs could be linked | CatalogService / CategoryTypeService | Foreign category and mismatched/foreign type tests |
| F09 | FindAsync returned tracked foreign catalog/category/contact rows | Existing catalog/category/supplier/broker services | Tracked foreign mutation/read/delete tests; filtered queries |
| F10 | Tenant user admin could escalate or change a shared global account | UserService | Role-escalation/shared-account tests; protected/self memberships |
| F11 | New/changed memberships lacked usable role defaults | Permissions / UserService / development seed | Staff/Manager defaults tested; startup no longer overwrites explicit permission sets |
| F12 | Lifecycle allowed arbitrary skips/reversals | PurchaseService/status authorization | Five invalid transitions, stale version and per-action HTTP permission cases |
| F13 | Invalid/duplicate/excess receipts could touch stock | PurchaseService | Entire batch prevalidated; invalid quantities/duplicate/over-receipt tests |
| F14 | Repeated/stale purchase receipts could add stock twice | Purchase xmin / receipt API/client | Partial receipt retry and separate-context PostgreSQL conflicts |
| F15 | Stock allowed invalid quantities and leaked transaction lifetimes | StockService | Negative/precision tests, existing stock tests and PostgreSQL rollback/release test |
| F16 | Mutable/deletable ledger and catalog deletion could erase history | AppDbContext / StockMovement FK / CatalogService | EF sync/async immutable tests; real PostgreSQL catalog delete restriction |
| F17 | Receipt movements lacked purchase provenance | PurchaseService / StockService / AdjustStockRequestDto | PostgreSQL asserts PurchaseOrder reference; internal fields JsonIgnore for API binding |
| F18 | Drafts counted as actual spend | DashboardService / ReportService | Unit/PostgreSQL analytics exclude drafts and cancelled orders |
| F19 | Inventory value used invented CurrentStock × 10 | ReportService | Confirmed effective line cost tests; unpriced stock contributes no invented value |
| F20 | Stock counts/labels confused available, physical and system quantities | DashboardService / ReportService / CatalogDetail | Available counts use CurrentStock − ReservedStock; CatalogDetail now labels its CurrentStock value as system stock |
| F21 | Unexpected errors could expose internal detail / conflicts misclassified | Program / CatalogController / StockController | Secret-marker HTTP test and explicit 400/403/404/409 mappings |
| F22 | Auth and AI endpoints lacked request rate limits | Program / existing endpoint attributes | Per-IP auth and per-user/IP AI sliding windows; no distributed/load qualification claimed |
| F23 | Search returned contact domains without their view permission | SearchController | Three HTTP permission combinations, using current server membership |
| F24 | Credential CORS trusted arbitrary localhost in production | Program | Explicit Cors:AllowedOrigins; HTTP arbitrary-localhost rejection test |
| F25 | Date-only report bounds caused PostgreSQL UTC errors; today excluded; render bounds unstable | ReportService / ReportsDashboard | PostgreSQL unspecified-DateTime query; invalid period unit tests; stable memoized UTC UI bounds |
| F26 | Catalog/stock/purchase/notification page sizes were unbounded | Existing services | Capped pages/sizes; no claim contact lists are now paginated |
| F27 | Category/type/broker counts and purchase catalog checks caused N+1 queries | Existing services | Aggregate projections and batch tenant catalog dictionary; final service regression passes |
| F28 | Duplicate review changed outer-loop item during ID reordering | CatalogService | Three distinct-pair regression test |
| F29 | Mutations left affected caches stale or invalidated the same purchase twice | PurchaseForm / PurchaseDetail | Scoped keys; duplicate-submit frontend test asserts purchase/dashboard invalidation |
| F30 | Redacted financial fields could be saved back as zero by nonowners | PurchaseService / PurchaseForm | Server rejects changes to existing monetary values by nonowners; UI handles absent prices safely |
| F31 | Client entered TaxTotal was trusted | PurchaseInputLimits / PurchaseService / PurchaseForm | Removed writable totals; preview/save compute line discount then tax; malicious JSON total test and PostgreSQL persistence |
| F32 | Archive button permanently deleted the catalog row | CatalogService / CatalogController / catalogApi / CatalogDetail | Explicit archive endpoint with version/tenant checks; preserves item/stock/purchase history; nine browser archive tests |
| F33 | Role attribute and refreshed server role used different claim types | Program / CurrentUserService | Configure role claim type and remove old mapped role claims; owner/manager variant/payment HTTP tests |
| F34 | Cache/forms survived identity/tenant/role changes; Strict Mode could duplicate startup refresh | AuthProvider | Synchronous cache clear and form remount on scope change; four tests including same-scope token preservation and single startup request |
| F35 | UI sign-out omitted server logout; logout did not recognize new three-part cookie | AppShell / AuthController / Login | Authenticated-user-bound revocation for both cookie formats and foreign-cookie protection; nine login/switch/network-failure/logout browser scenarios; linked login labels |

These rows include defense-in-depth code changes with different evidence depths. F20/F22/F26/F27 are source-reviewed/regression-tested, not standalone production load or penetration-test signoffs.

## Exact financial behavior and remaining integrity gap

Reference `line_gross_base` uses `qty × kg_per_unit × landing_cost_per_kg` only when both weight fields are positive and `abs(kg_per_unit × landing_cost_per_kg − unit landing rate) <= 0.05`; otherwise it uses `qty × unit landing rate`. Target preview/save share this branch and bounded decimal calculation.

Reference `line_money` applies discount then tax. Target preview/create/update now compute gross → line discount → line tax through PurchaseInputLimits; TaxTotal/Subtotal/GrandTotal are not writable request fields. Malicious extra totals are ignored and tests verify server-derived results. Percentages outside reference limits are rejected instead of silently clamped. Target preserves four-decimal supported rates/totals; reference DecimalModel/decimal_precision quantizes money/rates/totals to two decimals and quantity/weight to three. Full freight/delivered/billty, header discount, commission and unit-mode parity remains R01, P0; full purchase financial parity is FAIL. Existing stored totals are not repriced by migrations.

Payment is a manual cumulative total, capped by the backend at GrandTotal as in the reference payment patch. Pending/Partial/Paid and due-soon/overdue are derived independently of delivery/stock. Due terms use the target creation date in UTC; the reference supports a separate purchase_date. Four-decimal storage remains for historical compatibility. Monetary amounts are owner-only, only an owner may write payments, and the activity event records states without leaking amounts in embedded JSON. Existing rows receive PaidAmount=0, not a fabricated payment inferred from an old enum; historical paid states need reconciliation before production (R10). Payments never confirm an order or move stock. A cancelled purchase with a recorded payment cannot be deleted.

Valuation now uses latest confirmed noncancelled effective line cost `LineTotal / OrderedQuantity`. This is an estimate, not a claim of FIFO/weighted-average/fully allocated accounting valuation. Stock with no confirmed cost is counted as unpriced. Spend uses confirmed/noncancelled data, with existing created-date grouping; reference business-date methodology still requires qualification.

## Test and build evidence

| Check | Before application changes | Final executed result |
|---|---|---|
| Backend build | Baseline test compilation succeeded | `dotnet build backend/PurchaseAssistant.slnx --no-restore --verbosity minimal`: exit 0, 0 warnings/errors |
| Backend unit | 63/63 | 184/184, 0 failed/skipped |
| PostgreSQL integration | 13/13 | 23/23, 0 failed/skipped; actual local `warehouse_erp_dev` |
| Frontend build | PASS | `npm run build`: exit 0 |
| Frontend tests | 29/29 | `npm test`: 53/53, 5 files |
| Browser | 14/14 baseline | `npm run test:e2e`: latest production build 62/62, 4.2 minutes, after the final catalog system-stock label correction; prior development 61/62; isolated 390px retest 1/1 |

Final backend result files: `backend/PurchaseAssistant.UnitTests/TestResults/reference-final.trx` and `backend/PurchaseAssistant.IntegrationTests/TestResults/reference-final.trx`. Browser configuration now builds and serves the production assets on a separate local port, with failure traces and fresh server ownership; it does not reuse a stale development server. Test: `frontend/playwright.config.ts`, `frontend/e2e/phase3.spec.ts`. Screenshots remain under ignored `frontend/test-results/`; purchase review, 320px payment and 1920px variant images were visually inspected. Dependencies used: SDK 10.0.203, Node 24.18, local PostgreSQL. No live provider API or reference test execution was required or claimed.

The 62 browser cases cover nine purchase review/error/reduced-height scenarios, eight existing-page checks, nine login/business-switch/error/server-logout scenarios, nine payment scenarios, nine variant/archive scenarios, nine explicit arrival/verification/partial/full receipt scenarios and nine matrices over 22 routes (198 route/width checks). They check heading/main rendering, page errors and document/main horizontal overflow. Purchase review never saves; explicit submission displays server errors; AI never writes stock. A reduced 400px viewport checks scrolling access, not a real software keyboard. Mocked browser order state does not substitute for PostgreSQL API semantics, which have separate tests.

Failures encountered and resolved are not hidden: an expanded catalog fixture initially omitted CurrentStock and failed rendering; it was corrected. A frontend invalidation assertion expected one call and was changed to verify affected keys when dashboard invalidation was added. Empty reset tokens correctly produced framework 400 instead of the test's expected 503; the test now verifies both 400/503 rejection paths and unchanged state. Additional resolved failures included an old manually-entered-tax test, owner role claim mapping, and TypeScript test/ref typing. The development browser suite failed once with a blank page at 390px, then the isolated case passed. Its root cause remains UNKNOWN (R35); it is not downgraded to a warning. Failed screenshot/context was retained outside the workspace at C:/Users/SURAG/.codex/reference-repositories/regression-failures-20261001. Current TRX files describe the latest executed backend run.

## Full regression coverage boundary

| Required flow | Final evidence / remaining limit |
|---|---|
| Login/logout/refresh/business selection | Server cookie revocation for both formats, selected-business refresh, membership/permission HTTP checks, cache/form isolation and nine browser login/switch/failure/logout interactions; access-JWT revocation remains R04 |
| Dashboard/users/catalog/categories/types/suppliers/brokers/search | Existing service/unit/integration tests and browser rendering; search HTTP permission checks; not exhaustive live API CRUD for every role |
| Variants | CRUD/default weight/duplicates/owner-delete/tenant/version HTTP, unit, PostgreSQL and nine browser interactions pass; name-length/reference legacy migration scope remains R06 |
| Barcode | Manual tenant lookup/index/assignment exists; camera scanner/device/cache performance not verified |
| Stock/physical/system/low/movements | Existing stock tests + immutable/history/concurrency/receipt integration checks; browser route checks; reservation/opening-stock breadth remains partial |
| Purchase draft/create/preview/confirm | Server service/HTTP/frontend/browser evidence; review token, weights, duplicate submission/error cases; full financial parity remains FAIL |
| Receiving/verification/delivery | Real PostgreSQL partial/full/stale/batch rollback and browser explicit steps; detailed short/damaged/returned verification and transport metadata absent |
| Payment/damage | Payment cumulative total, balance, due-soon/overdue, owner permissions, stale/replay/concurrent behavior tested; full payment ledger/method/date parity and damage remain partial/missing |
| Notifications/reports/period/valuation/analytics | Existing plus corrected unit/integration tests and browser routes; no scheduler/export/saved views/drilldown qualification |
| AI parse/review/apply | Mocked routing/provider transport/timeout/malformed/ambiguity/auth/tenant/frontend tests; explicit backend draft submission required |
| OCR/WhatsApp/voice/restore commit | BLOCKED/DEFERRED, not activated or tested as implemented |
| Settings/realtime/PWA | FAIL: active target implementations absent |
| Responsive | PASS stated viewport tests; full physical-device/accessibility/camera/touch/dialog/sticky-header coverage remains R31 |

## Deployment impact

Six migrations were reviewed and applied **only to the existing local development database**:

1. `20261001082605_ReferencePurchaseSafety`: nullable purchase line weight fields. Purchase Version maps PostgreSQL's existing system `xmin`; no fake application xmin column is created.
2. `20261001083902_ReferenceStockLedgerProtection`: changes StockMovements → CatalogItems delete behavior from CASCADE to RESTRICT; no row deletion or financial rewrite. Its Down restores CASCADE, so rollback restores the older history-deletion risk.
3. `20261001085317_ReferencePurchaseVerification`: nullable VerifiedAt/VerifiedById; no legacy verification is invented. Existing uncompleted orders must follow explicit verification before future receipts. Previously completed orders are not reopened/recounted.

4. `20261001092811_ReferenceCalculatedLineTax`: line DiscountPercent/TaxPercent default zero; existing totals are preserved. Removed client TaxTotal request input; API/UI must release together.
5. `20261001094650_ReferenceCatalogVariants`: optional kg/unit, GUID concurrency metadata, computed normalized name and tenant/item/name unique index. Existing duplicate normalized names cause migration failure for deliberate reconciliation; no automatic deletion/merge.
6. `20261001095600_ReferencePurchasePayments`: zero/default PaidAmount, nullable PaidAt/PaymentDays; no inferred legacy payment or change to existing invoice totals.

API/UI must deploy together: writes now need a reviewed PreviewToken, update/status/receive/payment need ExpectedVersion; variant edits/deletes and item archive need RowVersion, and receipts require Verified. Secure shared Data Protection key persistence is required for restart/multiple-instance review-token validity. Production must configure `Cors:AllowedOrigins` explicitly. Auth/AI rate limits are process-local. No external secrets, upload providers, WhatsApp actions, restore commits or production migrations were enabled. Password recovery intentionally returns unavailable until a complete safe issuer/delivery/single-use flow exists. No production deployment, financial repricing or row deletion was performed. New variant version/normalized-name metadata is populated by the additive database defaults/computed column.

## Every known remaining issue and recommended phase

Paths below are relative to this target root unless prefixed `reference:`. A missing feature is pointed at its existing owner/model/UI placeholder rather than inventing a new file. Evidence labels describe what is actually known; production UNKNOWN does not become PASS.

| ID | File / existing owner | Feature and remaining problem | Severity | Evidence | Next phase |
|---|---|---|---|---|---|
| R01 | backend/PurchaseAssistant.Application/DTOs/Purchase/PurchaseInputLimits.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseForm.tsx | Full financial parity: line discount/tax calculation is implemented and client totals removed; freight/billty/delivered charges, header discount/commission, unit-mode semantics and reference two-decimal precision remain unmapped | P0 | VERIFIED_CODE line_totals_service/compute_totals/decimal_precision vs tested supported subset | A financial correctness, then C |
| R02 | backend/PurchaseAssistant.Web/Authorization/OwnerFinancialResultFilter.cs; backend/PurchaseAssistant.Web/Controllers | Financial serialization uses a known-key list; full per-response/role and future export/schema coverage not certified; financial input permissions need comprehensive qualification | P0 | VERIFIED_TEST covers Owner/Manager/Staff purchase + recursive known keys; other responses UNKNOWN runtime | A |
| R03 | backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs; frontend/src/auth/AuthProvider.tsx | Service FK checks and browser context cache/form isolation fixed, but composite tenant FKs and historical invalid-link/production data audit not performed; direct DB writers outside service validation | P0 signoff gap | VERIFIED_CODE global filters/explicit checks; production data UNKNOWN/BLOCKED | A/BB with production data access |
| R04 | backend/PurchaseAssistant.Web/Controllers/AuthController.cs; backend/PurchaseAssistant.Web/Services/CurrentUserService.cs | Logout-all revokes refresh rows; JWT access validity until expiry/session revoke policy not comprehensively tested; session families/bounded refresh hash scans not mapped | P0 signoff gap | VERIFIED_TEST refresh rotation; access-session revocation runtime UNKNOWN | A |
| R05 | backend/PurchaseAssistant.Web/Controllers/AuthController.cs; frontend/src/pages/auth/Login.tsx | Secure password recovery unavailable; no safe token issuer, single-use store, verified delivery or recovery browser flow; registration/Google auth absent | P1 / BLOCKED recovery | VERIFIED_CODE unsafe path removed; reference PasswordResetToken exists; external delivery contract missing | A then B after configuration |
| R06 | backend/PurchaseAssistant.Domain/Entities/MasterDataRelations.cs; backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs; frontend/src/pages/catalog/CatalogVariants.tsx | Variant CRUD/default weight/UI implemented; target retains existing 150-character name limit versus reference 512. Legacy archived-entry variant deletion guard has no equivalent target legacy data; migration/legacy import scope requires qualification | P1 | VERIFIED_TEST CRUD/tenant/owner/conflicts/DB; VERIFIED_CODE catalog.py schemas and archived-entry guard | B/BB for remaining parity |
| R07 | backend/PurchaseAssistant.Infrastructure/Services/SupplierService.cs; backend/PurchaseAssistant.Infrastructure/Services/BrokerService.cs; backend/PurchaseAssistant.Domain/Entities/MasterDataRelations.cs; frontend/src/pages/suppliers/SupplierList.tsx | Item/supplier/broker linking, defaults/rates/ledgers/history incomplete; update normalization/uniqueness/error and field validation breadth not finished | P1 | VERIFIED_CODE contacts/default service/model contracts vs target CRUD only | B |
| R08 | backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs; backend/PurchaseAssistant.Infrastructure/Services/CategoryService.cs; backend/PurchaseAssistant.Infrastructure/Services/CategoryTypeService.cs; frontend/src/pages/catalog/CatalogForm.tsx | Catalog metadata, bulk archive/create, taxonomy summaries, validation lengths/scales, opening stock/unit conversion and reference defaults incomplete | P1 | VERIFIED_CODE reference catalog schemas/handlers; target supported subset | B/D |
| R09 | frontend/src/pages/purchases/PurchaseForm.tsx; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; backend/PurchaseAssistant.Web/Controllers/PurchaseController.cs | Per-user wizard autosave/resume missing; legacy draft confirmation has no durable preview/review stamp; draft delete API does not carry external expected version | P1 | VERIFIED_CODE reference TradePurchaseDraft/local WIP; supported target preview tokens tested | C |
| R10 | backend/PurchaseAssistant.Domain/Entities/PurchaseOrder.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchasePayment.tsx | Payment amount/balance/due states/API/UI implemented. Full payment ledger, separate purchase date, mark-paid increment convenience, reference precision and historical paid enum reconciliation remain incomplete | P1 | VERIFIED_TEST owner/tenant/amount/state/PostgreSQL concurrency and browser; VERIFIED_CODE reference richer schema/status/payment helpers | C/E, historical reconciliation before production |
| R11 | backend/PurchaseAssistant.Domain/Entities/PurchaseOrder.cs; backend/PurchaseAssistant.Domain/Entities/PurchaseItem.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Damage/short/missing/returned reports, resolution/status/notes/notification and stock/finance effects missing | P1 | VERIFIED_CODE reference purchase_damage_service/damage routers/model | C |
| R12 | backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Stage verification works, but per-line received/damaged/return verification, short shipment resolution and reference unit setup preflight missing | P1 | VERIFIED_TEST explicit stage gate; VERIFIED_CODE richer reference verification | C |
| R13 | backend/PurchaseAssistant.Domain/Entities/PurchaseOrder.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Dispatch note/carrier/delivery dates/transport metadata/pipeline details not mapped; legacy verified rows without actor/time require explicit operational resolution | P1 | VERIFIED_CODE reference dispatch/arrive/delivery schema vs target state/timestamps | C |
| R14 | backend/PurchaseAssistant.Infrastructure/Services/StockService.cs; backend/PurchaseAssistant.Domain/Entities/StockMovement.cs; frontend/src/pages/stock/StockDetail.tsx | Reservation operations/opening stock/audit/dispute sessions, mandatory adjustment reasons and physical-count idempotency breadth incomplete | P1 | VERIFIED_CODE richer reference stock routers/models; target subset tested | D |
| R15 | frontend/src/pages/catalog/BarcodeManager.tsx; backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs | Camera scanner state machine, permission handling, scan history/label printing/unknown barcode quick-create/cache timing not mapped or physically tested | P2 | VERIFIED_CODE reference barcode controllers/cache; no target camera decoder found | D/I |
| R16 | backend/PurchaseAssistant.Infrastructure/Services/DashboardService.cs; frontend/src/pages/Dashboard.tsx | Owner command center, goals/staff task summaries/delivery pipeline breadth missing | P2 | VERIFIED_CODE owner dashboard/business goals/staff tasks | E |
| R17 | backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs; backend/PurchaseAssistant.Infrastructure/Services/DashboardService.cs | Daily usage/checklists/snapshots/staff task operational workflows missing | P2 | VERIFIED_CODE reference operations/owner_ops and Flutter pages | E |
| R18 | backend/PurchaseAssistant.Infrastructure/Services/NotificationService.cs; backend/PurchaseAssistant.Web/Program.cs; frontend/src/pages/NotificationsPage.tsx | Scheduled emitters/BackgroundService, dedupe unique constraint/concurrent notification creation and operational role subscriptions incomplete | P2 | VERIFIED_CODE target list/read/create helper; no active scheduled target producer found | E |
| R19 | backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs; frontend/src/pages/reports/ReportsDashboard.tsx | Saved views/BI/item drilldowns/supplier ledger analytics and complete created-vs-business-date comparison methodology remain incomplete | P2 | VERIFIED_CODE reference reports_trade/report_views/contact ledgers | E |
| R20 | backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs; frontend/src/pages/reports/ReportsDashboard.tsx; frontend/src/api/reportApi.ts | Valuation is latest confirmed effective line cost; allocation/accounting methodology parity and unpriced coverage/value UI absent | P1 | VERIFIED_TEST real cost/coverage; reference deeper financial inputs not mapped | A/E |
| R21 | backend/PurchaseAssistant.Web/Controllers/ReportsController.cs; backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs | Authorized tenant stock XLSX/monthly PDF/ZIP/JSON export absent | P2 | VERIFIED_CODE reference exports/export_files | E |
| R22 | backend/PurchaseAssistant.Web/Program.cs; reference:backend/app/routers/exports.py | Backup export/dry-run runner absent; restore commit intentionally 501 in reference pending production-copy approval | P2 / BLOCKED commit | VERIFIED_CODE restore_commit; no target runner | E, retain commit deferred |
| R23 | backend/PurchaseAssistant.Web/Program.cs; frontend/src/router/index.tsx; frontend/src/layouts/AppShell.tsx | Business profile/settings/provider credential editor absent; encrypted external credential store/rotation not configured | P2 | VERIFIED_CODE reference provider_credentials/settings; target menu has no working settings route | E |
| R24 | backend/PurchaseAssistant.Infrastructure/Services/AI; reference:backend/app/routers/media.py; reference:TASKS.md | OCR/vision blocked: reference is pasted/plain-text extraction, not actual image vision; no target provider/upload contract | P3 / BLOCKED | VERIFIED_CODE UTF-8 base64 decode and fixed confidence; deferred TASKS | G only after supported contract |
| R25 | backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs; reference:backend/app/models/unit_intelligence.py | OCR alias ORM/bootstrap exists; active correction-learning service/API/UI not located; correction-events SQL alone is not a learning feature | P3 / BLOCKED | VERIFIED_CODE ORM/DDL, UNKNOWN active learning | G, retain deferred |
| R26 | backend/PurchaseAssistant.Web/Program.cs; reference:backend/app/services/whatsapp_po_delivery.py | WhatsApp credentials/infrastructure missing; reference webhook/signature/status callback not found; auto-send behavior conflicts with explicit action requirement | P3 / BLOCKED | VERIFIED_CODE Graph v19 PDF/3 retry/60s/manual fallback; TASKS deferred | H explicit action only after configuration |
| R27 | backend/PurchaseAssistant.Infrastructure/Services/AI; reference:backend/app/config.py | Voice processing route/provider/UI absent; flag does not verify feature | P3 / BLOCKED | DOCUMENTATION_CLAIM/config flag only | G/H optional contract first |
| R28 | backend/PurchaseAssistant.Web/Program.cs; frontend/src/api/apiClient.ts; frontend/src/lib/queryKeys.ts | No target SignalR hub/event client despite reference business SSE feed and README claim | P2 | VERIFIED_CODE reference realtime.py; target source absence | J after event/tenant contract |
| R29 | frontend/package.json; frontend/public; frontend/src/main.tsx | No active target PWA manifest/service worker/plugin; offline mutation replay safety unverified | P2 | DOCUMENTATION_CLAIM target PWA; reference web assets exist | I/J, no automatic offline writes |
| R30 | backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs; backend/PurchaseAssistant.Infrastructure/Services/SupplierService.cs; backend/PurchaseAssistant.Infrastructure/Services/BrokerService.cs | Duplicate review still O(n²); contact lists unbounded; no representative dataset timings/DB plans; backend ETags/cache absent | P2 | VERIFIED_CODE algorithms/list contracts; production load UNKNOWN | J |
| R31 | frontend/e2e/phase3.spec.ts; frontend/src/layouts/AppShell.tsx; frontend/src/auth/AuthProvider.tsx | Mocked browser tests do not cover all live CRUD/role states, dialogs/nav/sticky headers/accessibility/touch 48px/physical keyboard/camera; missing flows cannot pass full regression | P1 signoff gap | VERIFIED_RUNTIME nine viewport scopes; physical/live flows UNKNOWN | I/L |
| R32 | backend/PurchaseAssistant.Web/Program.cs; backend/PurchaseAssistant.Infrastructure/Migrations; backend/PurchaseAssistant.Web/appsettings.json | Production key ring/CORS/secrets/migration rehearsal/health readiness/distributed limits and encrypted external credentials unverified; no production deployment performed | P0 signoff gap | VERIFIED_CODE local settings/migrations; production UNKNOWN/BLOCKED | A/AZ/L before deployment |
| R33 | backend/PurchaseAssistant.Domain/Entities/SecurityAuditLog.cs; backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs; frontend/src/pages/purchases/PurchaseDetail.tsx | Activity reuses generic audit table and latest 100; complete retention/filter/global staff activity UI, verifier display name and DB audit append-only controls incomplete | P2 | VERIFIED_TEST purchase events; reference lifecycle/activity breadth richer | C/E/J |
| R34 | backend/PurchaseAssistant.UnitTests; backend/PurchaseAssistant.IntegrationTests; frontend/src/tests; reference:backend/tests | Reference tests not run; not every new hardening branch has dedicated HTTP/load/network coverage (e.g. auth rate limits and complete financial schema coverage); no full contract parity suite | P1 signoff gap | Actual target counts above; reference execution UNKNOWN | L |
| R35 | frontend/e2e/phase3.spec.ts; frontend/src/auth/AuthProvider.tsx; frontend/playwright.config.ts | Prior development suite rendered a blank page once at 390px; isolated case passed with trace, root cause remains unknown. Production suite subsequently passed all 62 cases; development failure is still unexplained | P1 unresolved regression | VERIFIED_TEST development 61/62 failure, isolated 1/1 pass; failed screenshot/context retained externally; no invented root cause | L reproduce and diagnose |

## Deferred items and known limitations

OCR/vision, OCR correction learning, WhatsApp, voice and restore commit remain inactive. Their blocked reasons come from located source, deferred TASKS entries, missing credentials/contracts or explicit production-copy signoff. No external integration is invented. The reference itself auto-commits verified delivery and auto-sends WhatsApp from some lifecycle paths; the target keeps stock commit as an explicit user operation and does not reproduce auto-send.

Framework-specific Flutter sheets/providers are not copied. Source frontend constructor references are evidence of usage in code, not live route/device success. The target stock append-only guard covers tracked EF changes, and FK restriction prevents catalog cascade deletion; an administrator using raw SQL can still alter ledger data. Existing permission arrays remain explicit overrides; no bulk role/permission/data migration silently rewrites production records.

The work completed here is the source inventory/gap documentation, safety fixes and bounded core/purchase additions. **The requested full feature implementation remains unfinished at the issues above.** Do not deploy or label the ERP/reference parity complete using only these build/test results.
