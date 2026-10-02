# Warehouse Assistant — verified implementation status

Checkpoint: 2026-10-02 (Asia/Calcutta). Reference: local reference-repo, pinned main commit ab63ee73efeb537ca4e11afdccc160450c5356d6. This checkpoint supersedes historical target statuses in the inventory and gap matrix. Reference source was inspected; its Python/Flutter suite was not executed. PASS means the described target contract passed the stated checks, not certification of the entire reference ERP or production deployment. Prior verified Harisree branding/login/PWA changes were preserved in this phase.

Evidence labels: VERIFIED_CODE means inspected implementation; VERIFIED_TEST means executed passing checks; VERIFIED_RUNTIME means observed local API/browser/database behavior. DOCUMENTATION_CLAIM and ASSUMPTION are not proof; UNKNOWN/BLOCKED identifies absent contracts or unavailable environments. Preserved prior-phase PASS rows refer to their executed regression evidence, not reference runtime.

| Area | Status | Evidence | Remaining Issue |
|---|---|---|---|
| Branding | PASS | Existing Warehouse Assistant / Harisree Agency identity; browser regression | None in this phase |
| Assets | PASS | Canonical root Harisree PNGs match public icons; native brand WebP assets retained | None |
| Login | PASS | Responsive/validation checks and real Owner/Manager/Staff sign-in | None in supported sign-in flow |
| Signup | BLOCKED | Reference public registration disabled by default; target owner-help flow | Guarded source route exists; no approved public target flow |
| Password Reset | BLOCKED | Target safely unavailable, no false success | Reference token routes exist; target email-delivery/recovery contract unavailable |
| PWA | PASS | Built preview icons/MIME/manifest/worker/cache/offline tests | Offline business mutations unavailable |
| User Management | PASS | Manager creates/manages Staff; escalation and privileged demotion rejected | None in requested role matrix |
| Permission Editor | PASS | Known permissions, tenant scope, self-edit and privileged membership guards | None in requested role matrix |
| Menus | PASS | Existing shell; Operations and Settings checked for all three roles | No Export menu added |
| Damage | PASS | PurchaseDamageService sole owner; regression and quantity classification | None in supported workflow |
| Daily Operations | PASS | Checklist/notes/templates/team progress, task transitions, cumulative usage, snapshots/history and rule-based report; API/database/browser evidence | Historical snapshots read-only; materialization uses today's stock |
| Export | PARTIAL | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME: existing XLSX/PDF/JSON/ZIP/selected CSV retained; five backend-owned stock/low-stock/supplier-line/supplier-report/item-report CSV formats verified | All 37 CSV columns mapped; missing historical metadata, business-date filtering and native/source layout adaptations remain explicit |
| Settings | PARTIAL | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME: existing settings plus manual backup, latest-50 history, help links, real backup/AI-use telemetry and device backup preferences | WhatsApp delivery producer absent; Arabic help unported; nightly wall-clock trigger not observed |
| Financial Integrity | PASS | Existing backend totals/payment/valuation owners; owner-only financial fields | Exact reference decimal precision remains a historical gap |
| Stock Integrity | PASS | Only StockService changes CurrentStock; actual PostgreSQL correction/conflict/snapshot checks | None in supported mutations |
| AI | PASS | Draft-only/failover regression and controlled HTTP test of business-key resolution | External provider availability untested |
| OCR | BLOCKED | Reference decodes/parses text; no verified target image recognition | Real vision and correction learning deferred; supplied OCR PASS unsupported |
| Images | PASS | Existing protected logo handling and asset regression | Production storage depends on configuration |
| Realtime | PASS for tested stock scenario | VERIFIED_RUNTIME: two permitted clients receive the correct item once; foreign/no-permission clients receive none; reconnect refreshes actual stock | Purchase/notification event delivery and production scale not separately verified |
| Security | PASS | Auth, selected membership, tenant isolation, 403/404/409, privilege/secret protections | Deployment-specific validation still required |
| Desktop UI | PASS | Three roles at 1366x768, 1440x900, 1920x1080; actual API Owner/Manager smoke | Physical hardware untested |
| Mobile UI | PASS | Three roles at 390x844, 393x852, 412x915; actual API Staff smoke; reduced-height scrolling | Height simulation does not prove physical keyboard behavior |
| Performance | PARTIAL | Lazy pages, bounded task/history queries; no added polling | No production load/device latency benchmark |
| Backend Build | PASS | Full slnx build, zero warnings/errors | None |
| Backend Tests | PASS | VERIFIED_TEST: 294 unit/endpoint + 51 real PostgreSQL tests; zero skipped | External services controlled in tests |
| Frontend Build | PASS | TypeScript + production bundling | None |
| Frontend Tests | PASS | VERIFIED_TEST: 9 files, 79 tests | None |
| Browser Tests | PASS | VERIFIED_TEST: 144 ordinary tests + 11 separate real-API checks (eight retained plus three CSV-role checks) | Ordinary suite uses API fixtures; live suite uses local disposable data |
| Documentation | PASS | Current status, inventory and matrix updated with evidence limits | Initial audits remain labelled historical |

## Implemented contracts and owners

Daily Operations remains in OperationsService, IOperationsService, ApiOperationController, existing entities/DTOs/configuration and OperationsPage. No parallel operations service was introduced. Reference router operations.py and staff_tasks.py are the source for the workflow; inventory/matrix detail the API mapping.

Checklist notes are personal, templates are Owner/Admin/Manager editable, and business summary counts distinct slot/key completions. Owner/Admin assign work to active business members; Manager sees the team and Staff sees own tasks. Only assignees accept/complete/reject, with optimistic version checks and optional correction notes. Usage replaces the cumulative quantity for today: changing 2 to 3 consumes only one additional unit through StockService. Whole batches are validated before writes; relational transactions and stale item versions protect the ledger. Today-only snapshot materialization is idempotent and does not change stock. Date/item-filtered history remains readable. Rule-based reports display usage, stock age, last movement and monthly supplier frequency. No ML was added.

SettingsController retains business/logo and notification ownership. Own-name updates use the existing 150-character schema bound; they cannot alter another user, email, role or permissions. Business writes stay Owner/SuperAdmin only. Six notification kinds persist per business/user and existing NotificationService consumes them. Forms save/cancel drafts, preserve failed edits and disable while saving. Missing logo storage is visibly unavailable.

ProviderCredentialService owns encryption and internal resolution. Seven reference types are write-only to Owner/Admin/SuperAdmin, with tenant/type-specific Data Protection isolation, safe audit metadata and version checks. Responses expose status/time/version/suffix only; values at most four characters long expose no suffix. Tenant AI keys precede server fallback; controlled HTTP transport tests verify resolution without an external provider call. WhatsApp key storage does not claim a WhatsApp delivery pipeline. Owner overview now also reads the latest actual business backup status/time/bytes and today's UTC AI request count from persisted records. WhatsApp delivery telemetry remains visibly unavailable. Target stored purchase totals remain authoritative; reference line-only spend/decimal precision differences are not exact parity.

Manager user-management policy now matches the existing role defaults/service: Staff creation/management allowed; privileged creation/promotion/demotion denied. Permission grants cannot exceed the Manager's permissions or include privileged keys. Staff cannot gain user-management access by adding a forged users.manage permission. The damage service was not recreated; its quantity DTOs were correctly classified as operational values.

## Executed verification

| Command / runtime | Actual result |
|---|---|
| dotnet build backend/PurchaseAssistant.slnx --no-restore | PASS, zero warnings/errors |
| dotnet test backend/PurchaseAssistant.slnx --no-restore | VERIFIED_TEST: PASS, 294 unit/endpoint + 51 PostgreSQL; zero skipped |
| npm test -- --run (frontend) | VERIFIED_TEST: PASS, 79/79, 9 files |
| npm run build (frontend) | PASS: TypeScript + Vite |
| npm run test:e2e (frontend) | VERIFIED_TEST: PASS, 144/144 after final frontend edit; retained 125 checks + 19 CSV checks |
| npx playwright test --config playwright.live.config.ts | VERIFIED_RUNTIME / VERIFIED_TEST: PASS, 11/11 against local ASP.NET API/PostgreSQL; no route mocking |
| Runtime CSV file inspection | VERIFIED_RUNTIME: all five actual formats and seven UI downloads parsed; exact headers/widths, Unicode quoting, decimal/backend DTO comparisons, unknown blanks and Manager financial denial |
| git diff --check | PASS |
| EF database update against local development PostgreSQL | Previous-phase VERIFIED_RUNTIME: CompleteOperationsAndCredentials and AddBusinessBackupAndAiUsageHistory applied locally; no CSV migration |
| EF pending-model check | VERIFIED_TEST: no changes since the last migration |

The ordinary browser suite checks Owner/Manager/Staff at 390x844, 393x852, 412x915, 1366x768, 1440x900 and 1920x1080. CSV coverage adds all 18 role/size combinations and an actionable failure check. Owner downloads all five files; Manager downloads stock/low-stock only; Staff has no export controls. Date/filter parameters request fresh backend data, and the supplier table scrolls to its existing actions on mobile. Existing login, users/permissions, dashboard, catalog, suppliers/brokers, stock/physical stock, purchases/receiving/damage, operations/reports/notifications/AI, settings/backups/history/telemetry, realtime, branding and PWA regressions remain included. Ordinary tests use API fixtures, not production data. Owner 1440px report and 390px supplier captures were visually reviewed; CSV captures are preserved under ignored TestResults/Browser/Csv. Final frontend role-change-during-request rejection is VERIFIED_TEST, without claiming a live privilege-change scenario.

Live checks retain eight previous login/operations/settings/export/backup/telemetry/realtime/selected-CSV checks and add three actual CSV role checks. Owner/Manager XLSX/PDF/JSON/ZIP regressions passed; quantities and money retain backend authority. New CSV files use real decimal stock 1.2345, physical stock 1.1111, BAG purchase quantity 2, pack weight 50, per-kg rate 0.09 and stored line total 9.00. Reports yield 2 bags, 100 kg and amount 9, independently of purchase header totals. All five actual CSV files and seven UI downloads were parsed with standard CSV/decimal readers and compared with actual backend DTOs. Unknown opening/date/selling values remain blank. Foreign item access is 404; PostgreSQL tests additionally exercise every aggregate against two businesses, mismatched units and missing geometry. CSV queries perform no stock writes.

Retained live realtime checks use actual WebSockets: two permitted clients each receive one correct stock event; foreign/no-permission clients receive none, and reconnect refreshes actual stock. A real unconfigured AI request increments usage once without activating external providers. Backup/restore validation changes no stock; encrypted credential persistence remains covered. The temporary API was stopped and two disposable businesses/five users/dependent records/login audits were deleted transactionally. Zero businesses/users/purchases/catalog items/backup logs/AI logs/credentials/login audits remained for those fixture IDs. Ignored runtime artifacts remain as evidence; no production database or external provider was used.

Current logs under ignored TestResults: csv-backend-final-build.log, csv-backend-final-tests.log, csv-frontend-final-tests.log, csv-frontend-final-build.log, csv-browser-final-regression.log, csv-browser-focused.log, csv-live-final.log and csv-live-cleanup-counts.log. RuntimeCsv contains actual CSVs, UI downloads and authoritative-values.json. The 37-column preimplementation matrix is csv-column-audit.md; the post-verification matrix is in the project inventory document. Previous-phase RuntimeExports/PDF previews and prior logs remain historical evidence. Live fixtures needed an active item and fresh stock version; correcting the fixture preserved the application guards. Groups remain paced for the existing authentication rate limit.

Previous-phase historical checks included 283 unit/endpoint, 48 PostgreSQL, 73 frontend, 125 ordinary browser and eight live checks, plus 27 focused browser checks after a help-copy edit. Earlier transient browser networking and overlapping PostgreSQL readers were resolved and fully rerun in that phase. These counts do not describe the current final run.

Live replay requires a running local API, isolated disposable accounts and ignored frontend/live-session.local with businessId/password. Accounts use owner-{businessId}@example.test, manager-{businessId}@example.test and staff-{businessId}@example.test, with the tested membership permissions. The separate suite mutates only its disposable records. Deleted accounts are not usable; recreate isolated fixtures before replay. Physical devices/keyboards and production load were not tested.

## Migration and production placeholder audit

Migration 20261002051205_CompleteOperationsAndCredentials adds StaffTask and ProviderCredential persistence, tenant assignment membership FK, unique credential business/type constraint and concurrency tokens. EF also detected existing variant configuration width 512 versus prior snapshot 150. PostgreSQL blocks widening Name while generated NormalizedName depends on it; the migration rebuilds that derived column/index around widening the original column. Up succeeded locally without dropping original names. Production migration and rollback were not executed; rollback narrowing needs a check for names longer than 150.

Migration 20261002064557_AddBusinessBackupAndAiUsageHistory adds only source-supported BackupLogs and AiUsageLogs, business FKs with restricted deletion, bounded metadata, JSON row counts and business/time indexes. Application guards reject modifying/deleting these historical records; PostgreSQL tests verify persistence, FK rejection and stock integrity. Applied locally; EF reports no pending model changes. Neither latest migration has been deployed to production or rehearsed against a production copy. BACKUP_DIR, permissions, available disk space, retention, persistent Data Protection keys and database recovery need deployment verification.

The preceding phase searched active production source for unfinished/demo markers; retained matches were legitimate input/select hints and styling. Tests/fixtures, generated migrations and reference source are distinct from production behavior. StubAIProvider returns AI_NOT_CONFIGURED rather than fabricated content. Earlier phases added source-supported exports and backups; this phase adds only the five mapped CSV formats, with no new public signup/reset/ML/OCR/WhatsApp pipeline. PurchaseDamageService remains the only damage service; the final stock-write search still finds only the two existing StockService entity assignments (other matches are DTO projections).

## Export, backup, history, help and telemetry contracts

Reference evidence is VERIFIED_CODE only: backend/app/routers/exports.py is registered by main.py; backup_ops.py, owner_ops.py, llm_failover.py, Flutter backup_page.dart/settings_page.dart/help_guide_page.dart and CSV producers provide the contracts. The reference suite was not executed. The earlier NOT_SUPPORTED_BY_REFERENCE export claim is contradicted by active source. Existing target owners, paths, DTOs, permission policy and UI shell were extended; no Export sidebar menu or new financial authority was introduced.

All target export routes use /api/v1/exports, authenticated selected-business membership, RequireReportsView and Owner/Admin/Manager/SuperAdmin roles; Staff is denied. Source export_access defaults allow Owner/Admin/Manager and deny Staff. Financial values remain Owner/SuperAdmin only, including for Admin/Manager downloads. ReportService.ReportingPurchases supplies the same eligible business purchases to reports, exports and stored backups, excluding Draft/Cancelled. Native purchase CreatedAt and stock units/decimal precision are retained. PDF/ZIP/selected and supplier-line CSV format stored money to two decimals; source report CSV displays amount_inr at zero decimals; JSON preserves backend decimals; no financial engine rounding/schema change was made. Invalid ranges return 400; explicit paired ranges over ten years are rejected. Bounded downloads reject overflow instead of silently truncating purchases/catalog/suppliers. Every successful HTTP download has safe business/actor audit metadata, private no-store caching and nosniff; no secrets or local storage paths are returned.

| Reference contract | Target method/path | Supported scope, limits, format/name, errors and UI |
|---|---|---|
| GET stock-inventory.xlsx | GET stock.xlsx | Current active business catalog, max 5,000; real XLSX MIME, harisree_stock_YYYY-MM-DD.xlsx. Native nine columns retained plus subcategory/supplier/barcode. Empty native workbook remains valid (source returns 404); overflow 400. Settings > Export & Backup |
| GET purchases-month.pdf | GET purchases.pdf | Default UTC calendar month through today; optional existing start/end; max 2,000, 413 overflow, empty valid PDF. application/pdf; harisree_purchases_YYYY-MM.pdf by default, purchases.pdf for explicit dates. Stored totals/owner-only line money |
| GET backup/export | GET backup.json | Default inclusive last 90 UTC days; optional start/end. Native schemaVersion 1 preserved, catalog max 5,000, suppliers/purchases max 2,000, latest 500 filtered stock movements; 413 overflow. application/json; business-backup.json. No users/passwords/provider credentials |
| POST backup with range_preset | POST backup with rangePreset; existing GET backup.zip retained | month / quarter (last 90 UTC days) / all; max 400 purchases, 413 before archive generation, 404 empty. application/zip; purchase_assistant_backup_BUSINESS_YYYY-MM-DD.zip. Current stock XLSX, purchase summary/order PDFs, supplier ledgers, Summary.txt and README.txt; safe GUID entry names. Existing native archive paths retained |
| GET backup/logs | GET backup/logs | Latest 50 business run records, ordered by UTC creation/id; safe filename basename, counts/bytes/duration/status, normalized failure message. Loading/empty/populated/error/retry UI |
| POST backup/run | POST backup/run | Manual stored business JSON, actual immutable run history/audit; 200 success or safe 503 failure. No arbitrary stored-file read/download endpoint added |
| POST restore/dry-run with payload | POST restore/dry-run | Owner/SuperAdmin only; max 10 MiB; validates business ID/version/required arrays for native schemaVersion 1 and stored harisree-backup-v1. Existing validation response retained; valid/errors/counts, writesPerformed=false, restoreEnabled=false; malformed object 400, structural issues valid=false. No writes |
| POST restore/commit | POST restore/commit | Owner/SuperAdmin only; intentionally 501, as in source. No token/commit workflow invented |
| purchase_home.dart selected CSV | Purchases > Copy selected CSV; no invented HTTP CSV route | Owner/SuperAdmin only, selected visible rows; exact headers human_id,purchase_date,supplier,total_inr,remaining_inr,status. Existing server DTO totals and remaining balance only; two-decimal formatting, quoting/formula escaping. Clipboard errors visible; Manager/Staff denied |

BusinessBackupService streams all eligible purchase periods, active catalog and suppliers plus at most 2,000 native immutable stock audit IDs/timestamps in a repeatable-read snapshot. Stored source schema harisree-backup-v1 contains schema_version/business_id/exported_at/excludes and catalog/suppliers/purchases/stock_audits arrays. It includes purchase quantities but no financial values, accounts/password hashes/keys/provider credentials/settings/configuration. BACKUP_DIR or LocalApplicationData/HarisreeBackups owns storage; each business has a directory and exclusive timestamp+GUID file, avoiding source filename collisions. Latest 14 successful files are retained; historical run records remain. Retention accepts only generated basenames, rejects reparse directories/files and does not follow arbitrary persisted paths. PostgreSQL verification covers 16 runs, retained files/history and an unrelated sentinel. This is a business-data backup, distinct from the reference weekly pg_dump/GitHub-artifact database backup, which is operational deployment work.

BusinessBackupWorker is VERIFIED_CODE / VERIFIED_TEST for active-business scheduling at 02:00 Asia/Kolkata and scheduled service execution. Manual service/API execution is VERIFIED_RUNTIME. The actual nightly wall-clock trigger was not observed. Optional daily downloaded JSON backup on app open defaults off and is scoped per device/user/business, deduplicated while in flight and stamped only after success. Local timestamps are device preferences, not shared server history. The monthly owner reminder appears in Settings and dismisses for the scoped month. History means backup runs plus the existing Purchases shortcut; no new settings-change, credential-use or session-history API was invented.

Help is a static English adaptation of the reference role guide with keyboard-operable sections and working permitted links to existing purchase/stock/operations/settings actions. Source Arabic help remains unported. Camera scanning/label PDF actions are explicitly unavailable where target contracts do not exist. Settings integration telemetry records one real logical AI request, including failure, with safe provider/latency/escalation metadata; disabled requests do not create fake usage. No prompts, model override, credentials, raw error body, tokens or cost are stored. The model field remains null when trustworthy request metadata is unavailable. Owner overview reads today's UTC business request count and latest actual backup; other tenants cannot affect the count. WhatsApp delivery count/health is UNKNOWN/BLOCKED because no target delivery producer exists; credential presence does not imply delivery health.

## Authoritative CSV mapping and migration decisions

The complete 37-column, 16-field matrix is in [REFERENCE_FULL_FEATURE_INVENTORY.md](REFERENCE_FULL_FEATURE_INVENTORY.md#authoritative-csv-column-mapping--2026-10-02), including every reference meaning/header, current entity/field, transformation/state, precision, null behavior, tenant policy, permission, migration/backfill decision and evidence. It was preceded by a source/data-model audit before application edits. Existing StockService, ReportService, ExportsController and ExportFileBuilder own projections, server aggregates and generation; React requests fresh files only.

All routes below have prefix GET /api/v1/exports. S = existing export roles Owner/Admin/Manager/SuperAdmin with reports.view and stock.view; F = Owner/SuperAdmin with reports.view (supplier lines also require supplier.view and purchase.view). Staff is denied throughout. Financial-role and selected-user/business changes during download discard stale responses.

| Reference CSV contract | Native route / filename | Verified mapping and explicit limit |
|---|---|---|
| Low-stock: name,subcategory,unit,system_stock,physical_stock,reorder,purchased,status,supplier | low-stock.csv / harisree_low_stock.csv; S | CurrentStock, PhysicalStock and ReorderLevel from catalog; received native-unit purchase totals, exact status/source display rules, tenant last supplier. Unknown conversions blank; absent quick-purchase/source normalized snapshots remain a gap |
| Stock action labelled Excel: Item,Category,Subcategory,Unit,Current Stock,Opening Stock,Purchased,Reorder Level,Last Updated | stock.csv / harisree_stock_export.csv; S | Current catalog/threshold, received native-unit quantities and MAX immutable StockMovement.CreatedAt. Opening blank; ledger-derived timestamp is a documented native adaptation |
| Supplier lines: date,pur_id,item,qty,unit,landing_per_unit,selling,total_line | suppliers/{supplierId}/purchases.csv / harisree_supplier_GUID.csv; F | Tenant purchase lines, native order number/current catalog name, ordered quantity/unit, stored rate branch and LineTotal. Business date/selling blank; historical name snapshot unavailable |
| Supplier report: supplier,bag_qty,bag_kg,amount_inr | reports/suppliers.csv / harisree_report_suppliers.csv; F | Stable supplier identity; source BAG/SACK/BOX/TIN classifier and explicit KG-name pack fallback; decimal stored line-total aggregation; missing bag geometry yields blank weight |
| Item report: name,kg,bags,boxes,tins,amount_inr,purchase_count | reports/items.csv / harisree_report_items.csv; F | Stable item identity, source pack counts/weight, stored classified-line totals and distinct parent purchase count; no generic report schema |

Exact headers/order, comma delimiter, quote doubling, embedded newlines/Unicode, UTF-8 without BOM, LF, formula-safe text, safe names and text/csv; charset=utf-8 are verified. Empty native downloads are header-only (reports retain the source preamble), an explicit source empty/no-share adaptation. Bounds reject more than 5,000 catalog items, 2,000 purchases or 50,000 lines with safe 400, never truncate. Invalid filters/ranges return 400; foreign/inactive IDs 404; anonymous 401; denied roles/policies 403. All successful downloads retain audit/private-no-store/nosniff protections.

No new migration, snapshot change or backfill was made for CSV. Independent opening stock, business purchase date, selling rate, historical line-name, normalized stock-unit/weight snapshots and quick-purchase workflow need a separate capture/import design. Source opening/selling are nullable; source purchase_date/item_name are required, so absent facts remain an exact-parity blocker. No CreatedAt is substituted into the supplier date column; native range filters deliberately use existing CreatedAt. No historical balance/name/geometry is inferred from current data. Native physical stock is nonnullable, unlike the source optional physical value. No SKU/barcode/reserved/variance/valuation/supplier-contact/payment/delivery columns occur in these five source CSV formats. Missing XLSX metadata remains a separate gap.

Decimal authority is preserved: up to four native stock/supplier quantity decimals; source integer low-stock discrete display; up to two other low-stock/report quantity decimals; two money-rate/line-total decimals; zero report-money decimals. MidpointRounding.AwayFromZero (HALF_UP) is presentation only. No financial formula/storage precision, stock ownership or existing export contract changed. All five native mappings have VERIFIED_CODE/VERIFIED_TEST/VERIFIED_RUNTIME evidence; missing historical source values remain UNKNOWN/BLOCKED, with blank handling verified separately.


## Trusted historical-data contract and safe import design

This latest phase is design-only: the existing application and all CSV/export contracts remain unchanged. The full [historical field matrix and design](REFERENCE_FULL_FEATURE_INVENTORY.md#trusted-historical-data-contract--design-checkpoint-2026-10-02) distinguish inspected source (VERIFIED_CODE), proposed native mechanisms (DOCUMENTATION_CLAIM), missing original facts/identity maps (UNKNOWN/BLOCKED), and actual existing regression evidence. No application endpoint, importer UI, schema, migration, capture, correction or backfill was implemented.

| Area | Established contract / design decision | Current evidence / implementation gate |
|---|---|---|
| Opening stock | Reference catalog opening quantity plus absolute ledger event, recording actor/time/lock. Historical metadata import changes no current stock; new initialization/correction must use StockService and a separate explicit preview | VERIFIED_CODE. No independently effective-dated financial/opening table found; old original dates cannot be inferred |
| Business date | Required reference purchase_date calendar date, distinct from invoice/created/received/completed times. Proposed nullable legacy BusinessPurchaseDate; explicit date required for future source-compatible confirmation | VERIFIED_CODE / proposed design. Legacy timestamp substitution forbidden |
| Selling rate | Optional line INR rate per purchase quantity unit; validated new aliases agree. Proposed nullable SellingRate, without financial recalculation | VERIFIED_CODE / proposed design. Conflicting or unlabelled legacy aliases/basis require trusted clarification |
| Historical name | Required persisted reference line item_name. Proposed nullable ItemNameSnapshot; future confirmation captures server-reviewed name, never retroactive catalog-name copying | VERIFIED_CODE / proposed design. Old names UNKNOWN/BLOCKED |
| Historical units/weights | Native Unit/KgPerUnit already exist; legacy Unit migration defaults empty and geometry is nullable. Reference additionally saves normalized quantity and total weight | VERIFIED_CODE. First metadata importer only asserts existing Unit/KgPerUnit; correcting missing/conflicting financial/stock inputs is a separate gate |
| Provenance / audit | Reuse existing operation audit; proposed committed import batch and append-only per-field provenance, actor/time/business/source hash/row/original allowed scalar/revisions | DOCUMENTATION_CLAIM design, not persisted or runtime-tested |
| Null / backfill | KNOWN, UNKNOWN, NOT_APPLICABLE, NOT_CAPTURED are distinct. Known zero opening/rate remains zero; unknowns remain NULL/projection-unknown with correction requirement | No historical backfill. Current catalog/stock/timestamps, AI/OCR and source maintenance scripts cannot prove old facts |
| Import workflow | Validate → zero-write preview → review → explicit confirm → reauthorize/version checks → one atomic metadata/provenance/audit transaction; bounded deterministic matching/idempotency, no stock/totals writes | Design only. No trusted dataset/source-to-target identity map or confirm schema/implementation exists |
| Permissions | Proposed upload/preview/confirm/correction/history Owner/SuperAdmin only, selected business and relevant existing reports/catalog/purchase policies. Initial stock also stock.adjust | Design only; no Manager/Staff grants, new permission or global tenant bypass |

The implementation blocker is specific: no verified historical source file and stable item/parent/line mapping, no import/provenance persistence, and no reviewed correction mechanism for old blank Unit or missing/conflicting financial geometry. Legacy source rate basis and normalized quantity target-unit/scope need record-specific evidence. The contract-first task is documented; application/data implementation stops at these gates under the user's stop conditions. Acknowledgement cannot turn an unknown fact into truth.

Current design-phase regression reruns the unchanged application: full backend build/test, frontend test/build and the complete fixture browser suite. This does not validate a future importer. Logs are historical-backend-build.log, historical-backend-tests.log, historical-frontend-tests.log, historical-frontend-build.log and historical-browser-regression.log under ignored TestResults. Prior 11 local live checks/actual CSV artifacts remain prior-phase VERIFIED_RUNTIME, not rerun or importer evidence. Phase-start hashes are compared to confirm only the three required documentation files changed. Future focused validator/confirm/provenance/PostgreSQL/upload/preview/UI tests are an explicit acceptance plan in the inventory; none are claimed passing before implementation.

Final design-phase checks: VERIFIED_TEST — 294 unit/endpoint + 51 PostgreSQL, zero skipped; 79 frontend tests in nine files; 144 ordinary browser tests passed in 8.2 minutes, including the three-role/six-size matrix. Backend build passed with zero warnings/errors; frontend production build passed. EF reports no changes since the last migration. Final git diff --check passed. File hashes confirm only the three required documents changed among 378 existing project files. No importer persistence, preview/confirm screen or production/physical-device behavior is certified by these regression results.

## Remaining development

1. Exact next task: Implement only a read-only historical-metadata-v1 validator/preview with isolated synthetic fixtures, explicit field states, deterministic source/target identity and role/policy checks, zero-persistence tests, unsafe-source/backfill rejection and responsive owner review. Keep confirm unavailable and generate no migration until a trusted historical source manifest/identifier map and a separate correction contract for legacy Unit/KgPerUnit are reviewed. Additive persistence/confirm is a later verified slice.
2. Full export/Settings parity remains PARTIAL for missing historical data, native/source layout/schema and date/unit differences, Arabic help and absent WhatsApp delivery telemetry. Restore commit deliberately remains 501. Real OCR/correction learning remains UNKNOWN/BLOCKED; the source text parser is not image recognition.
3. Rehearse both latest migrations and backup storage/retention/recovery against a production copy; verify persistent encryption keys, external provider availability and device/load performance. Production deployment, database rollback/recovery, actual nightly trigger and untested realtime event families remain unverified.

## Working-tree file manifest

This manifest includes preserved branding/Operations/Settings/export/backup/CSV work and this historical contract documentation phase. Earlier changes were preserved; the listed paths are the current uncommitted work. No CSV or historical-contract schema migration was added.

- backend/PurchaseAssistant.Application/DTOs/Operations/OperationsDtos.cs
- backend/PurchaseAssistant.Application/DTOs/Purchases/DamageReportDto.cs
- backend/PurchaseAssistant.Application/DTOs/Reports/CsvExportDtos.cs
- backend/PurchaseAssistant.Application/DTOs/Settings/SettingsDtos.cs
- backend/PurchaseAssistant.Application/DTOs/Stock/StockDtos.cs
- backend/PurchaseAssistant.Application/Interfaces/AI/IAIProviderFactory.cs
- backend/PurchaseAssistant.Application/Interfaces/AI/IAIUsageRecorder.cs
- backend/PurchaseAssistant.Application/Interfaces/AI/IProviderCredentialResolver.cs
- backend/PurchaseAssistant.Application/Interfaces/IOperationsService.cs
- backend/PurchaseAssistant.Application/Interfaces/IReportService.cs
- backend/PurchaseAssistant.Application/Interfaces/IStockService.cs
- backend/PurchaseAssistant.Domain/Entities/IntegrationHistory.cs
- backend/PurchaseAssistant.Domain/Entities/Operations.cs
- backend/PurchaseAssistant.Domain/Entities/StockMovement.cs
- backend/PurchaseAssistant.Domain/Entities/UserSettings.cs
- backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs
- backend/PurchaseAssistant.Infrastructure/Data/Configurations/OperationsConfiguration.cs
- backend/PurchaseAssistant.Infrastructure/Data/Configurations/SettingsConfiguration.cs
- backend/PurchaseAssistant.Infrastructure/Migrations/20261002051205_CompleteOperationsAndCredentials.Designer.cs
- backend/PurchaseAssistant.Infrastructure/Migrations/20261002051205_CompleteOperationsAndCredentials.cs
- backend/PurchaseAssistant.Infrastructure/Migrations/20261002064557_AddBusinessBackupAndAiUsageHistory.Designer.cs
- backend/PurchaseAssistant.Infrastructure/Migrations/20261002064557_AddBusinessBackupAndAiUsageHistory.cs
- backend/PurchaseAssistant.Infrastructure/Migrations/AppDbContextModelSnapshot.cs
- backend/PurchaseAssistant.Infrastructure/Services/AI/AIProviderFactory.cs
- backend/PurchaseAssistant.Infrastructure/Services/AI/AIRoutingService.cs
- backend/PurchaseAssistant.Infrastructure/Services/AI/AiUsageRecorder.cs
- backend/PurchaseAssistant.Infrastructure/Services/AI/StubAIProvider.cs
- backend/PurchaseAssistant.Infrastructure/Services/BusinessBackupService.cs
- backend/PurchaseAssistant.Infrastructure/Services/ExportFileBuilder.cs
- backend/PurchaseAssistant.Infrastructure/Services/OperationsService.cs
- backend/PurchaseAssistant.Infrastructure/Services/ReportService.Csv.cs
- backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs
- backend/PurchaseAssistant.Infrastructure/Services/StockService.Csv.cs
- backend/PurchaseAssistant.Infrastructure/Services/StockService.cs
- backend/PurchaseAssistant.Infrastructure/Services/UserService.cs
- backend/PurchaseAssistant.IntegrationTests/Stock/BackupHistoryIntegrationTests.cs
- backend/PurchaseAssistant.IntegrationTests/Stock/CsvExportIntegrationTests.cs
- backend/PurchaseAssistant.IntegrationTests/Stock/OperationsIntegrationTests.cs
- backend/PurchaseAssistant.IntegrationTests/Stock/StockIntegrationTests.cs
- backend/PurchaseAssistant.UnitTests/AI/AIProviderFactoryTests.cs
- backend/PurchaseAssistant.UnitTests/AI/AIRoutingServiceFailoverTests.cs
- backend/PurchaseAssistant.UnitTests/AI/AIRoutingServiceTests.cs
- backend/PurchaseAssistant.UnitTests/AI/CsvExportEndpointTests.cs
- backend/PurchaseAssistant.UnitTests/AI/ExportBackupEndpointTests.cs
- backend/PurchaseAssistant.UnitTests/AI/FinalParityEndpointTests.cs
- backend/PurchaseAssistant.UnitTests/AI/OperationsSettingsEndpointTests.cs
- backend/PurchaseAssistant.UnitTests/AI/PurchaseIntentEndpointTests.cs
- backend/PurchaseAssistant.UnitTests/Services/ReferenceAuditSafetyTests.cs
- backend/PurchaseAssistant.UnitTests/Services/UserServiceRBACStaffTests.cs
- backend/PurchaseAssistant.Web/Controllers/ApiOperationController.cs
- backend/PurchaseAssistant.Web/Controllers/ExportsController.Csv.cs
- backend/PurchaseAssistant.Web/Controllers/ExportsController.cs
- backend/PurchaseAssistant.Web/Controllers/SettingsController.cs
- backend/PurchaseAssistant.Web/Program.cs
- backend/PurchaseAssistant.Web/Services/BusinessBackupWorker.cs
- backend/PurchaseAssistant.Web/Services/BusinessEvents.cs
- backend/PurchaseAssistant.Web/Services/ProviderCredentialService.cs
- docs/BRANDING_ASSET_AUDIT.md
- docs/REFERENCE_FULL_FEATURE_INVENTORY.md
- docs/REFERENCE_IMPLEMENTATION_STATUS.md
- docs/REFERENCE_VS_CURRENT_GAP_MATRIX.md
- frontend/e2e/branding.spec.ts
- frontend/e2e/csv-exports.spec.ts
- frontend/e2e/operations-settings.spec.ts
- frontend/e2e/phase3.spec.ts
- frontend/index.html
- frontend/live-e2e/operations-settings.spec.ts
- frontend/playwright.live.config.ts
- frontend/public/favicon.png
- frontend/public/icon-192.png
- frontend/public/icon-512.png
- frontend/public/manifest.webmanifest
- frontend/public/offline.html
- frontend/public/sw.js
- frontend/src/api/exportsApi.ts
- frontend/src/auth/AuthProvider.tsx
- frontend/src/components/AI/PurchaseAssistant.tsx
- frontend/src/components/BackupReminder.tsx
- frontend/src/components/BrandIdentity.tsx
- frontend/src/components/CsvExportButton.tsx
- frontend/src/components/ExportControls.tsx
- frontend/src/components/RouteErrorBoundary.tsx
- frontend/src/components/users/PermissionEditor.tsx
- frontend/src/layouts/AppShell.tsx
- frontend/src/pages/BackupPage.tsx
- frontend/src/pages/HelpGuidePage.tsx
- frontend/src/pages/OperationsPage.tsx
- frontend/src/pages/SettingsPage.tsx
- frontend/src/pages/auth/Login.tsx
- frontend/src/pages/purchases/PurchaseList.tsx
- frontend/src/pages/stock/StockList.tsx
- frontend/src/pages/suppliers/SupplierList.tsx
- frontend/src/pages/users/UsersPage.tsx
- frontend/src/router/index.tsx
- frontend/src/tests/BackupSettings.test.tsx
- frontend/src/tests/CsvExportButton.test.tsx
- frontend/src/tests/ExportContracts.test.ts
- frontend/src/tests/PurchaseAssistant.test.tsx
