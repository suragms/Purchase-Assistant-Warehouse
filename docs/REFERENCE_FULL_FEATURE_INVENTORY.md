# Reference full feature inventory

Reference: https://github.com/ANANDU-2000/PurchaseAssiastant.git

Branch: main; commit: `ab63ee73efeb537ca4e11afdccc160450c5356d6`; audit date: 2026-10-01 (Asia/Calcutta).

Initial extraction was read-only. The fresh clone is outside the target. 1353 tracked text files were indexed and source-searched; 229 route definitions and 52 model classes were extracted. Indexing is not execution or proof of correctness. The source index preserves path, line count and SHA-256 for reproducibility.

Evidence labels: VERIFIED_CODE = located implementation; VERIFIED_TEST = executed passing test; VERIFIED_RUNTIME = observed running API/browser; DOCUMENTATION_CLAIM = unproven docs; ASSUMPTION = unverified; UNKNOWN/BLOCKED = no active implementation or unavailable environment. Reference tests were located but not executed; no reference behavior is labelled VERIFIED_TEST or VERIFIED_RUNTIME. Target baseline tests were executed.

Status COMPLETE requires all requested API/security/tenant/UI/regression evidence. Source presence alone does not meet that threshold. All category statuses below are the initial audit; final changes are in REFERENCE_IMPLEMENTATION_STATUS.md.

## Required source coverage

### Current target checkpoint — 2026-10-02

The source inventory below remains the initial evidence record. Current target results and limitations are authoritative in [REFERENCE_IMPLEMENTATION_STATUS.md](REFERENCE_IMPLEMENTATION_STATUS.md). Daily Operations, Settings, export/backup and the subsequent CSV mapping phase extend existing owners and preserve verified Harisree branding, login and PWA behavior. Reference source is VERIFIED_CODE only; target test/runtime evidence is labelled separately.

| Reference-supported capability | Current target owner/evidence | Current scope |
|---|---|---|
| AC: personal checklist, notes, templates, team summary | OperationsService / ApiOperationController / OperationsPage; endpoint and real-API browser checks | Implemented; Owner/Admin/Manager template editing, own completion, tenant-bound summary |
| AC: staff tasks and performance | Existing operations owner extended; StaffTask EF entity with tenant membership FK/version | Assignment Owner/Admin; Manager team list; Staff own list/actions; accept/complete/reject and notes tested |
| AC: daily usage and snapshot history | OperationsService calls StockService; actual PostgreSQL integration and live browser | Cumulative corrections, versions/transactions, validated batches, history filters; idempotent today-only materialization |
| AC: operational report | Existing report method/page | Real stock age/movement, 7/30-day usage and monthly supplier frequency; rule-based, no ML |
| AS: personal/business profile and logo | SettingsController / SettingsPage | Own-name only; owner business writes; existing authenticated logo storage controls retained |
| AS: notifications | Existing UserSettings + NotificationService | Six per-business/user kinds; explicit save/cancel; persisted and consumed by notifications |
| AS: provider/integration credentials | ProviderCredentialService + settings controller/entity; controlled provider transport and persistence tests | Seven write-only encrypted types; metadata only; business AI key before environment fallback |
| AS: operational command center | OperationsService + SettingsPage; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Real stock/damage/task exceptions and owner-only target stored spend; latest actual backup status/time/bytes and UTC AI request count. WhatsApp delivery telemetry UNKNOWN/BLOCKED |
| AK/AL: downloads | Existing ExportsController, shared ReportService purchase query, Settings > Export & Backup; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Stock XLSX, monthly PDF, 90-day native JSON and month/quarter/all ZIP. Source export router is active; earlier NOT_SUPPORTED_BY_REFERENCE claim corrected. No Export sidebar menu |
| Historical capture/import contract | Source and native model/write-path audit; full nine-row field matrix and controlled workflow below | Design documented; no importer/model/migration/backfill/UI changes. Unit/KgPerUnit already persisted; missing original facts and stable source identifier mapping gate future writes |
| AK: five additional CSV formats | Existing StockService / ReportService / ExportsController / CsvExportButton; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Low-stock, stock, supplier-line, supplier-report and item-report; exact 37 headers/columns mapped. Fresh backend files; financial-only role guards; missing history explicit, no migration/backfill |
| AK: selected purchase CSV | PurchaseList + exportsApi; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Exact source six headers; server DTO total/balance, two-decimal formatting and clipboard; Owner/SuperAdmin only. The five additional formats are mapped below; historical-data parity remains PARTIAL |
| AL/AS: stored business backup and history | BusinessBackupService + BackupLog; VERIFIED_CODE / VERIFIED_TEST / manual VERIFIED_RUNTIME | Source harisree-backup-v1 JSON, all eligible purchase periods, no financial values/accounts/credentials; 14 successful files retained, latest 50 history records. Business data, not database/config backup |
| AL/AS: scheduler/device preferences | BusinessBackupWorker and BackupReminder; VERIFIED_CODE / VERIFIED_TEST | 02:00 Asia/Kolkata active-business scheduler; nightly wall-clock runtime UNKNOWN. Optional daily app-open download defaults off, scoped device/user/business; monthly owner reminder in Settings |
| AL: restore validation | Existing ExportsController + BackupPage; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Owner-only native/source schema validation, 10 MiB limit, no writes; commit deliberately 501. No restore workflow invented |
| AS: help/history shortcut | HelpGuidePage + SettingsPage; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Static English role guide with working permitted links; existing Purchases shortcut. Source Arabic guide unported, unavailable camera/label-PDF actions identified. No new settings/session history API |
| AM/AS: real usage metadata | Existing AIRoutingService + AiUsageRecorder; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | One persisted logical request including failure, safe provider/latency/escalation, model null; no prompts/keys/raw errors/tokens/cost; no record for disabled AI |
| AX: realtime stock runtime | Existing BusinessEvents + RealtimeUpdates retained; VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME | Two permitted clients receive correct item once; foreign/no-permission clients receive none; 401/403 and reconnect refresh verified. Other event families/production scale unverified |
| AO/AP: OCR and learning | Reference media.py decodes base64 as UTF-8 and parses text; TASKS defers real OCR | No verified vision pipeline; do not inherit an unsupported OCR PASS or invent learning |

Target regression executed: full backend build (zero warnings/errors), 294 unit/endpoint tests, 51 PostgreSQL integration tests, zero skipped, 79 frontend tests in nine files, 144 ordinary browser tests after the final frontend edit and 11 actual-API checks (eight retained plus three CSV roles). Browser role matrix covers Owner/Manager/Staff at 390x844, 393x852, 412x915, 1366x768, 1440x900 and 1920x1080. Ordinary tests use API fixtures; live checks use disposable local accounts/database without request mocking. All disposable records were removed and verified absent; API stopped. Actual files from all five CSV formats and seven UI downloads were parsed and compared with backend DTOs; Owner desktop report and mobile supplier captures were visually reviewed. Source runtime, external delivery, nightly trigger and physical-device/load behavior remain unverified.

Safety adaptations: historical snapshot materialization cannot fabricate past stock from current quantities; past records remain readable. Personal names retain the target 150-character bound. Target stored purchase totals remain authoritative; overview/export formatting does not claim full reference line-spend/storage-precision parity. Signup/reset delivery remain unavailable. Full AK/AL and AS remain PARTIAL for documented missing historical data and native/source date/unit/schema-layout differences, Arabic help and absent WhatsApp delivery telemetry; production migrations/performance remain unverified.

### Export/Settings source audit resolved in this phase

The detailed target route/role/filter/limit/MIME/filename/error mapping is in the status document. Source authentication uses the business path and export_access; its Owner/Admin/Manager defaults allow export and Staff defaults deny it. Target preserves selected-business authentication, RequireReportsView and its existing role/DTO contracts, with stricter Owner/SuperAdmin-only financial disclosure. No new permission or generic export-all endpoint was introduced.

| Source evidence (VERIFIED_CODE) | Exact source scope | Verified target action / unresolved gap |
|---|---|---|
| backend/app/routers/exports.py and main.py | Active stock XLSX, monthly PDF, range ZIP, JSON export, run/logs and owner dry-run; restore commit 501 | Extended existing controller and Settings page; native route/schema/archive paths retained. Empty native XLSX remains valid unlike source 404; omitted metadata never filled with fake defaults |
| backend/app/services/backup_ops.py; backend/app/models/owner_ops.py | Stored harisree-backup-v1 business JSON, latest-50 logs, retain 14 files; source backup excludes provider credentials | Streaming repeatable-read target snapshot, safe basename/error metadata, unique timestamp+GUID names, immutable application history; native stock ledger maps audit IDs/timestamps. No arbitrary file-download route |
| backend/app/main.py | Nightly 02:00 Asia/Kolkata business backup | Worker/NextRun and scheduled service tested; actual nightly runtime not observed |
| .github/workflows/db-backup.yml | Weekly pg_dump with GitHub artifacts retained 90 days | Distinct operational database backup; no application database-backup endpoint invented |
| flutter_app/lib/features/settings/presentation/backup_page.dart and settings_page.dart | Download presets, device timestamps, opt-in daily app-open JSON, monthly reminder, run/logs/dry-run and Purchases history shortcut | Native Settings hub and scoped browser preferences; tested success/empty/failure/retry/duplicate suppression; no settings-change history invented |
| flutter_app/lib/features/settings/presentation/help_guide_page.dart | Static English/Arabic role guide with feature links | English adaptation and working role links tested; Arabic and unavailable scan/label actions remain explicit gaps |
| backend/app/routers/owner_ops.py and services/llm_failover.py | Actual backup/AI-use/delivery records drive overview; one AI log per logical request | Actual backup and UTC AI count implemented/tested/live; no target WhatsApp producer, so delivery metric unavailable |
| Flutter purchase_home_page.dart selected CSV | human_id,purchase_date,supplier,total_inr,remaining_inr,status; selected rows, two-decimal display | Owner clipboard export uses existing backend total/balance DTOs; formatting only, no client calculation or new CSV HTTP endpoint |
| stock/presentation/widgets/low_stock_bulk_export.dart; stock_page.dart; supplier_detail_page.dart; reports_shell_page.dart | Low-stock nine-column CSV, stock action labelled Excel emits CSV, supplier line CSV, supplier/item report CSV | All 37 CSV columns mapped and five native backend-owned files tested/live-inspected; PARTIAL for missing opening/business date/selling/historical snapshots and documented native date/unit/ledger timestamp adaptations. No migration/backfill; rack is not in these CSV headers |

Stored business backup is not a settings/configuration snapshot. It excludes users/password hashes/keys/credentials and financial values. HTTP JSON preserves existing schemaVersion 1 and owner-only monetary fields; stored JSON preserves source schema_version harisree-backup-v1. Restore validation recognizes both and changes nothing. Help/history/telemetry source presence is not evidence for fabricated settings-change history, AI token/cost counters or WhatsApp delivery health. OCR remains UNKNOWN/BLOCKED; production/device performance, production migration/recovery and other realtime event families remain unverified.

## Authoritative CSV column mapping — 2026-10-02

This is the post-verification mapping. The initial 37-column audit was written before application edits in ignored TestResults/csv-column-audit.md. Reference source remains VERIFIED_CODE, not reference runtime. Evidence here concerns the target native mappings and observed blank handling; it does not certify unavailable historical source data. T = authenticated selected business with explicit parent/child/catalog/supplier predicates. S = existing export roles Owner/Admin/Manager/SuperAdmin, reports.view and stock.view; Staff denied. F = Owner/SuperAdmin with reports.view, plus supplier.view and purchase.view for supplier-line exports. No new permission.

Source provenance at the pinned reference commit:

| Contract | Source file / entry |
|---|---|
| Low-stock | flutter_app/lib/features/stock/presentation/widgets/low_stock_bulk_export.dart:25; core/utils/unit_utils.dart for display |
| Stock CSV | flutter_app/lib/features/stock/presentation/stock_page.dart:559; header at 570 |
| Supplier lines | flutter_app/lib/features/contacts/presentation/supplier_detail_page.dart:196; shared/widgets/trade_purchase_ledger_cards.dart:29 for canonical line amount |
| Supplier / item reports | flutter_app/lib/features/reports/shell/reports_shell_page.dart:360; core/reporting/trade_report_aggregate.dart for pack classification, weights and stored line totals |
| Received / historical metadata | backend/app/services/stock_helpers.py:_period_purchased_map; models/catalog.py and models/trade_purchase.py for persisted fields/nullability |

Target source owners: backend/PurchaseAssistant.Infrastructure/Services/StockService.Csv.cs, ReportService.Csv.cs and ExportFileBuilder.cs; backend/PurchaseAssistant.Web/Controllers/ExportsController.Csv.cs. Evidence tests: UnitTests/AI/CsvExportEndpointTests.cs and IntegrationTests/Stock/CsvExportIntegrationTests.cs under backend/PurchaseAssistant.*Tests; frontend/e2e/csv-exports.spec.ts and live-e2e/operations-settings.spec.ts. Live files and DTO comparisons are retained under ignored TestResults/RuntimeCsv.

| Export | Column | Reference header | Reference meaning | Current header | Current backend source | Authoritative entity | Authoritative field | Transformation / state | Precision | Null behavior | Tenant | Permission | Migration decision | Backfill decision | Evidence status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Low-stock | 1 | name | Catalog display name | name | StockService.GetCsvRowsAsync | CatalogItem | Name | EXACT_MATCH | text | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 2 | subcategory | Type name, category fallback | subcategory | StockService.GetCsvRowsAsync | CatalogItem / CategoryType / Category | Type.Name / Category.Name | TRANSFORM_REQUIRED, fallback | text | fallback category/empty | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 3 | unit | Stock unit | unit | StockService.GetCsvRowsAsync | CatalogItem | DefaultUnit | EXACT_MATCH native stock unit | text | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 4 | system_stock | Current system quantity | system_stock | StockService.GetCsvRowsAsync | CatalogItem | CurrentStock | TRANSFORM_REQUIRED, source unit display formatting | discrete integer; other up to 2 decimals | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 5 | physical_stock | Recorded physical quantity | physical_stock | StockService.GetCsvRowsAsync | CatalogItem | PhysicalStock | TRANSFORM_REQUIRED, native persisted physical source | source unit display | native nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 6 | reorder | Positive threshold only | reorder | StockService.GetCsvRowsAsync | CatalogItem | ReorderLevel | TRANSFORM_REQUIRED, nonpositive empty | source unit display | empty if <=0 | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 7 | purchased | Received purchase quantity in selected period/stock units, including reference quick purchases | purchased | StockService.GetCsvRowsAsync | PurchaseItem / PurchaseOrder / CatalogItem | ReceivedQuantity / Unit / CreatedAt / status / DefaultUnit | AGGREGATION_REQUIRED for matching native units; reference purchase_date/normalized snapshot/quick-purchase data unavailable | source unit display | blank if unknown unit conversion; no matching receipts means 0 then blank | T | S | Deferred source snapshots/workflow | Historical source-only imports unavailable; no reconstruction | VERIFIED_TEST positive receipts/conversion isolation; VERIFIED_RUNTIME zero receipts |
| Low-stock | 8 | status | out/critical/low/healthy from system stock/reorder | status | StockService.GetCsvRowsAsync | CatalogItem | CurrentStock / ReorderLevel | AGGREGATION_REQUIRED exact source threshold rule; native view filters use available stock | text | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Low-stock | 9 | supplier | Last catalog supplier name | supplier | StockService.GetCsvRowsAsync | CatalogItem / Supplier | LastSupplier.Name | EXACT_MATCH | text | missing/foreign supplier empty | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 1 | Item | Catalog name | Item | StockService.GetCsvRowsAsync | CatalogItem | Name | EXACT_MATCH | text | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 2 | Category | Category name | Category | StockService.GetCsvRowsAsync | CatalogItem / Category | Category.Name | EXACT_MATCH | text | missing/foreign empty | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 3 | Subcategory | Type name | Subcategory | StockService.GetCsvRowsAsync | CatalogItem / CategoryType | Type.Name | EXACT_MATCH | text | missing/foreign empty | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 4 | Unit | Catalog stock unit | Unit | StockService.GetCsvRowsAsync | CatalogItem | DefaultUnit | EXACT_MATCH native units | text | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 5 | Current Stock | Current system stock | Current Stock | StockService.GetCsvRowsAsync | CatalogItem | CurrentStock | TRANSFORM_REQUIRED, invariant decimal | target authoritative up to 4 decimals; source Dart raw double | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 6 | Opening Stock | Independently recorded opening balance | Opening Stock | StockService.GetCsvRowsAsync | None in target | Missing independently recorded opening balance | MIGRATION_REQUIRED if workflow/import is approved; leave blank | source stored 3 decimals, raw-double CSV; target unavailable | source schema nullable but CSV coerces null to zero; target preserves unknown blank | T | S | Deferred; no unused column added | No deterministic historical backfill; trusted import/manual decision required | UNKNOWN/BLOCKED source value; blank VERIFIED_TEST / VERIFIED_RUNTIME |
| Stock | 7 | Purchased | Period received quantity | Purchased | StockService.GetCsvRowsAsync | PurchaseItem / PurchaseOrder / CatalogItem | ReceivedQuantity / Unit / CreatedAt / status / DefaultUnit | AGGREGATION_REQUIRED; source/native date and unit differences explicit | target up to 4 decimals | unknown conversion blank | T | S | Same decision as purchased above | No fabricated normalization history/quick purchases | VERIFIED_TEST positive receipts/conversion isolation; VERIFIED_RUNTIME zero receipts |
| Stock | 8 | Reorder Level | Reorder threshold including 0 | Reorder Level | StockService.GetCsvRowsAsync | CatalogItem | ReorderLevel | EXACT_MATCH | target up to 4 decimals | nonnull | T | S | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Stock | 9 | Last Updated | Source persisted last stock-update time | Last Updated | StockService.GetCsvRowsAsync | StockMovement | CreatedAt (MAX, selected business/item) | TRANSFORM_REQUIRED, latest native ledger event; explicit native adaptation, never entity.CreatedAt fallback | UTC ISO 8601 | no ledger means blank | T | S | Persisted source metadata deferred | Ledger-derived only; no invented historic timestamp | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier lines | 1 | date | Business purchase date, distinct from creation timestamp | date | ReportService.GetCsvPurchaseLinesAsync | None in target | Missing business purchase_date; not CreatedAt | MIGRATION_REQUIRED; export blank, native CreatedAt filters explicitly separate | yyyy-MM-dd when source data exists | unknown blank; source date is required, exact parity BLOCKED | T | F | Deferred nullable capture/import plan; no field created now | No deterministic backfill; manual/import decision required | UNKNOWN/BLOCKED source value; blank VERIFIED_TEST / VERIFIED_RUNTIME |
| Supplier lines | 2 | pur_id | Purchase human identifier | pur_id | ReportService.GetCsvPurchaseLinesAsync | PurchaseOrder | OrderNumber | EXACT_MATCH native identifier | text | nonnull | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier lines | 3 | item | Line item name | item | ReportService.GetCsvPurchaseLinesAsync | CatalogItem | Name (current native name; historical snapshot missing) | TRANSFORM_REQUIRED native current catalog name; source persisted name snapshot missing | text | tenant item required | T | F | Snapshot workflow deferred | Cannot reconstruct historical renamed names | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier lines | 4 | qty | Ordered line quantity | qty | ReportService.GetCsvPurchaseLinesAsync | PurchaseItem | OrderedQuantity | EXACT_MATCH | target up to 4 decimals | nonnull | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier lines | 5 | unit | Purchase line unit | unit | ReportService.GetCsvPurchaseLinesAsync | PurchaseItem | Unit | EXACT_MATCH | text | nonnull | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier lines | 6 | landing_per_unit | Per-kg rate when paired positive geometry/rate exists, otherwise unit landing rate | landing_per_unit | ReportService.GetCsvPurchaseLinesAsync | PurchaseItem | KgPerUnit / LandingCostPerKg / UnitPrice | TRANSFORM_REQUIRED exact source branch; no multiplication | invariant 2 decimal HALF_UP display | paired nullable rate; unit rate fallback | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier lines | 7 | selling | Optional line selling cost/rate | selling | ReportService.GetCsvPurchaseLinesAsync | None in target | Missing optional line selling rate | MIGRATION_REQUIRED if explicit purchase workflow added; leave blank | source 2 decimals | reference nullable; unknown blank | T | F | Deferred; no unused financial column | No safe historical backfill | UNKNOWN/BLOCKED source value; blank VERIFIED_TEST / VERIFIED_RUNTIME |
| Supplier lines | 8 | total_line | Canonical persisted line total; source helper prefers lineTotal | total_line | ReportService.GetCsvPurchaseLinesAsync | PurchaseItem | LineTotal | EXACT_MATCH authoritative stored total; format only | 2 decimals HALF_UP display | nonnull | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier report | 1 | supplier | Supplier display name grouped by stable identity | supplier | ReportService.GetCsvPackReportsAsync | PurchaseOrder / Supplier | SupplierId / Name | AGGREGATION_REQUIRED, identity grouping | text | blank name becomes '-' | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier report | 2 | bag_qty | Classified BAG/SACK count; KG name-size fallback only explicitly defined by source | bag_qty | ReportService.GetCsvPackReportsAsync | PurchaseItem / CatalogItem | Unit / OrderedQuantity / Name (explicit source fallback) | AGGREGATION_REQUIRED exact source classifier/fallback in backend | up to 2 decimals source display | nonnull classified counts | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Supplier report | 3 | bag_kg | Weight of bag-family lines only | bag_kg | ReportService.GetCsvPackReportsAsync | PurchaseItem | OrderedQuantity / KgPerUnit; missing total_weight stays unknown | AGGREGATION_REQUIRED; missing positive geometry/weight snapshot means unknown | up to 2 decimals source display | any unknown contributor makes aggregate blank | T | F | Source total_weight snapshot deferred | Cannot backfill missing geometry from today's catalog | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping); missing geometry blank VERIFIED_TEST |
| Supplier report | 4 | amount_inr | Sum canonical line totals for classified BAG/BOX/TIN lines | amount_inr | ReportService.GetCsvPackReportsAsync | PurchaseItem | LineTotal (classified lines only) | AGGREGATION_REQUIRED in ReportService; not header GrandTotal | 0 decimals source display; HALF_UP decimal; no currency symbol | nonnull | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Item report | 1 | name | Catalog identity/name per classified item | name | ReportService.GetCsvPackReportsAsync | PurchaseItem / CatalogItem | CatalogItemId / Name (current native name) | AGGREGATION_REQUIRED stable IDs, native current-name adaptation | text | nonnull tenant catalog | T | F | Source line-name snapshot deferred | Historical names not reconstructed | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Item report | 2 | kg | Classified bag weight; BOX/TIN count-only are 0 by source rule | kg | ReportService.GetCsvPackReportsAsync | PurchaseItem | OrderedQuantity / KgPerUnit / Unit; missing weight unknown | AGGREGATION_REQUIRED source pack/weight semantics | up to 2 decimals source display | unknown bag geometry makes total blank | T | F | Source total_weight snapshot deferred | No historical geometry guesses | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping); missing geometry blank VERIFIED_TEST |
| Item report | 3 | bags | Bag-family pack count | bags | ReportService.GetCsvPackReportsAsync | PurchaseItem / CatalogItem | OrderedQuantity / Unit / Name (explicit source fallback) | AGGREGATION_REQUIRED source classifier | up to 2 decimals | 0 when no bag contributor | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Item report | 4 | boxes | Box-family count | boxes | ReportService.GetCsvPackReportsAsync | PurchaseItem | OrderedQuantity / Unit | AGGREGATION_REQUIRED source classifier | up to 2 decimals | 0 when no box contributor | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Item report | 5 | tins | Tin-family count | tins | ReportService.GetCsvPackReportsAsync | PurchaseItem | OrderedQuantity / Unit | AGGREGATION_REQUIRED source classifier | up to 2 decimals | 0 when no tin contributor | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Item report | 6 | amount_inr | Canonical totals for classified lines only | amount_inr | ReportService.GetCsvPackReportsAsync | PurchaseItem | LineTotal (classified lines only) | AGGREGATION_REQUIRED, stored values only | 0 decimals HALF_UP display | nonnull | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |
| Item report | 7 | purchase_count | Distinct purchases contributing classified lines | purchase_count | ReportService.GetCsvPackReportsAsync | PurchaseItem | PurchaseOrderId (DISTINCT) | AGGREGATION_REQUIRED DISTINCT, not line count | integer | 0 for empty source, no row | T | F | No | No | VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME (native mapping) |

Source CSV headers deliberately exclude item codes/barcodes/reservations/variance/valuation and supplier contacts/payment/delivery metrics; those are NOT_SUPPORTED in these five formats. Existing XLSX/PDF/JSON/ZIP/selected-purchase CSV remain separate contracts. No generic report schema or client financial aggregation was added.

Migration decision: no new migration or database field in this phase. Missing opening stock, business purchase date, selling rate, historical line-name snapshot, normalized stock-unit/weight snapshots and the reference quick-purchase workflow need explicit capture/import design. Reference opening/selling fields are nullable. Reference purchase_date and line item_name are required; target lacks the historical facts, so blank/current-name native adaptations remain PARTIAL rather than fabricated backfills. Source purchase_date is never substituted with CreatedAt in the supplier date column. Native date filters use existing CreatedAt and are explicitly not exact source purchase-date filtering. A ledger-derived Last Updated value is not claimed to be the reference persisted metadata field. Missing bag geometry/weight produces a blank aggregate, never a guessed zero/current catalog weight.

Backfill decision: none executed. No deterministic evidence exists for those missing historical values or renamed line-item snapshots. Trusted import/manual decisions are required. Source explicit KG-name pack-size fallback is used only for classified report pack quantities, as written in trade_report_aggregate.dart; it never reconstructs stock or financial values. BOX/TIN weights are zero by the explicit source count-only rule.

Precision: all queries, rates, quantities, sums and conversions use decimal. Stock/supplier raw quantities retain up to four native decimals; low-stock discrete quantities use source integer display, other low-stock/report quantities use up to two decimals with source near-integer formatting. Monetary rates/line totals are formatted to two decimals; source report amount_inr is formatted to zero decimals. Rounding uses MidpointRounding.AwayFromZero (HALF_UP) for presentation only. Stored line totals remain authoritative; no money formula/storage precision change, binary float conversion, currency symbol or percentage column is introduced.

CSV quality/security: deterministic exact header order; comma delimiter; RFC-style quote doubling and quoted CR/LF/commas; original Unicode/newlines preserved; UTF-8 without BOM and LF record endings. Text formula prefixes are protected. Empty native HTTP exports return headers (report preamble retained), differing from source no-share/disabled empty UI. Catalog max 5,000, purchase max 2,000, lines max 50,000; overflow rejected with safe 400, never truncated. Sources use active catalog for stock, native available-stock view filters and shared Draft/Cancelled exclusion for purchases. Unknown unit conversion stays blank. Names and optional relations are tenant-scoped; successful downloads have business/actor/static-format audit, private no-store and nosniff.

Native backend-owned routes: GET /api/v1/exports/stock.csv (filter/search/start/end/repeated ids); GET low-stock.csv (search/start/end/repeated ids); GET suppliers/{supplierId}/purchases.csv (start/end); GET reports/suppliers.csv and reports/items.csv (start/end). These port reference client CSV contracts under the existing native controller; they are not claims of matching reference HTTP routes. Financial files are Owner/SuperAdmin only. Native filenames: harisree_stock_export.csv and harisree_low_stock.csv match explicit source names; supplier/report files use safe native GUID/static names where source shares text without filename. MIME text/csv; charset=utf-8. Malformed filters/IDs/ranges return 400; missing/foreign/inactive selected IDs return 404; anonymous 401 and unauthorized 403.

Verification: 294 unit/endpoint + 51 PostgreSQL tests, zero skipped; 79 frontend tests; 144 fixture browser tests; 11 actual local API/browser checks (eight retained plus three CSV roles). New 19 fixture checks cover Owner/Manager/Staff across all six required dimensions and actionable failure. PostgreSQL validates exact decimal projections, two-business isolation, actual receipt totals, unknown conversions/weights and ledger timestamps without CSV stock writes. Runtime checks inspect all five real CSV formats and seven UI downloads against actual backend DTOs; Manager receives only the two stock files, Staff denied. Files under ignored TestResults/RuntimeCsv were parsed with a standard CSV reader and decimal comparisons. A final frontend role-change-during-request rejection has VERIFIED_TEST evidence; it does not claim a live privilege-change scenario. Source/framework tests were not executed.

## Trusted historical-data contract — design checkpoint, 2026-10-02

This section fulfills the contract-first phase. It is a reviewed source/model comparison and a proposed native capture/import design, not an implemented importer. No application source, endpoint, UI, model, migration, historical value or stock balance changes in this phase. VERIFIED_CODE below applies to inspected existing source. Proposed mechanisms are DOCUMENTATION_CLAIM (design requirements), not VERIFIED_TEST or VERIFIED_RUNTIME. Source tests were inspected, not executed. No actual trusted historical file or source-to-target identifier map was supplied. Commit, correction and stock initialization remain implementation gates; they must not be inferred from the passing existing regression suite.

### Source evidence and distinctions

All reference paths below are relative to the pinned reference-repo at ab63ee73efeb537ca4e11afdccc160450c5356d6.

| Evidence | Located source | Established meaning / limitation |
|---|---|---|
| VERIFIED_CODE | backend/app/models/catalog.py:112–118; backend/sql/035_opening_stock.sql | Nullable numeric(12,3) opening_stock_qty with opening_stock_set_at/by and locked flag on the catalog item. No separate financial opening-balance entity or independently effective-dated opening table was located |
| VERIFIED_CODE | backend/app/routers/stock/stock_detail.py:671–745; schemas/stock.py:363; tests/test_opening_stock_setup.py | Owner/SuperAdmin set nonnegative opening stock through apply_stock_movement, mode=absolute, source_type=opening_stock_setup; then persist catalog opening quantity, actual UTC recording time, actor display and lock. Changing a recorded quantity requires a reason. Located tests assert ledger/current quantity and correction reason, but were not run |
| VERIFIED_CODE | backend/app/models/stock_movement.py:17–58; services/stock_movement_service.py | Immutable event has before/delta/after, stock unit, source type/id, actor/time, metadata and business/idempotency uniqueness. This proves stock provenance, not a generic historical-file import protocol |
| VERIFIED_CODE | backend/app/routers/stock/stock_detail.py:1184–1225 | Existing recent-adjustment undo can append an absolute correcting event with privileged opening-stock handling. It does not define bulk historical-import undo or prove opening metadata is restored; no bulk reversal is designed as available |
| VERIFIED_CODE | backend/app/models/trade_purchase.py:41,109–145; schemas/trade_purchases.py:73–168 | Required purchase_date is a calendar Date distinct from created_at, delivery/paid timestamps and invoice_number. Required item_name/unit are persisted on each transaction line; nullable per-unit/total weights and selling fields are distinct |
| VERIFIED_CODE | backend/app/services/trade_purchase_service.py:1352–1426,1523–1606; schemas/trade_purchases.py:115–145 | Creation stores supplied business date, line name, line unit, normalized quantity, rate and weight snapshots. Aliases selling_cost/selling_rate and kg_per_unit/weight_per_unit are synchronized on new validated input. Reference update may replace lines and stock-adjust confirmed edits; native target only permits draft line updates, so source snapshots are persisted transaction values, not a claim of absolute source immutability |
| VERIFIED_CODE | backend/app/services/aggregate_totals_service.py:17–32; line_totals_service.py:98–117 | Active canonical selling totals/profit use ordered quantity × selling_rate (INR per purchase quantity unit). The weight-multiplying line_selling_gross_in helper has no caller in the inspected source and is not selected as authority. Old conflicting aliases or an undocumented legacy rate basis require source-specific clarification; no financial conversion is inferred |
| VERIFIED_CODE | backend/app/services/trade_purchase_service.py:645–679; services/trade_unit_type.py; services/unit_normalization.py | Default wholesale BOX/TIN are count-only and strip weight fields. Persisted unit label is distinct from derived bag/box/tin/kg/litre/pcs/other classification. SACK is a historical bag alias, not a new native capture option. qty_in_stock_unit is a saved normalized quantity; its historical target-unit label is not independently stored on the reference line |
| VERIFIED_CODE | backend/scripts/backfill_line_stock_unit_qty.py:25–37; services/unit_normalization.py:37–74 | Maintenance script recomputes from today's catalog/profile/name fallbacks. It is unsafe proof of historical conversion; not executed or ported. Current catalog-last-trade refresh and stock-commit repairs are not evidence for old opening/name/date/rate/weight facts |
| VERIFIED_CODE | backend/scripts/seed_suppliers_from_csv.py:80–130; routers/exports.py restore dry-run/commit | Supplier CSV seeding exists as a maintenance script with name/phone matching, not an authenticated historical preview/confirm workflow. Restore is validation-only with commit 501. No active historical-metadata file importer/provenance contract was found in the searched reference or target routes/services |
| VERIFIED_CODE | Target Domain/Entities/PurchaseItem.cs, PurchaseOrder.cs, CatalogItem.cs, StockMovement.cs; Infrastructure/Data/Configurations/PurchaseConfiguration.cs | Unit and nullable KgPerUnit already exist on native purchase lines; native ordered/received quantities and LineTotal remain authority. No business purchase date, line selling rate/name snapshot or recorded catalog opening quantity exists. DailyUsageLog.OpeningQty is an operational day calculation, not this opening-stock fact |
| VERIFIED_CODE | Target Migrations/20261001143945_FinalPurchaseChargesAndNotificationDedupe.cs:112–118; 20261001082605_ReferencePurchaseSafety.cs:15–30 | Historical Unit migration fills an empty string; KgPerUnit/landing rate were added nullable. Empty/defaulted unit and absent weight are not known historical snapshots. Do not infer a unit from today's catalog or the entity's PCS initializer |
| VERIFIED_CODE | Target Services/PurchaseService.cs:325–342,448–488,498–567; StockService.cs:155–209; AppDbContext.cs:44–50,112 onward | Draft-only line edits, purchase xmin version, stock RowVersion, existing transaction/stock owner, immutable ledger and composite business foreign keys are available building blocks. CurrentStock still has only the two existing StockService assignments |
| VERIFIED_CODE | Target Domain/Entities/SecurityAuditLog.cs; Services/PurchaseService.cs:299–321; Web/Program.cs:172–195 | Existing tenant audit stores actor/time/event/entity description and restricted metadata. Role plus known permission policies are reusable; audit itself is not an immutable per-field provenance store |

### Historical field contract matrix

P in the provenance column means the proposed field revision below: source kind/operation, actor and actual UTC capture/import time, source file hash/import ID/logical row and field when imported, original bounded scalar, and prior revision/reason on correction. No historical defaults are assigned. Proposed names here are design names only; none exist in the current model unless explicitly listed as current.

| Field | Reference requirement | Current field | Authoritative source | Persisted / derived | Historical meaning | Capture method | Import method | Provenance | Nullable? | Default? | Backfillable? | Safe backfill rule | Migration required? | Evidence |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Opening stock quantity | Catalog opening_stock_qty; nonnegative numeric(12,3), owner set through ledger | Missing; CurrentStock/PhysicalStock/DailyUsageLog.OpeningQty are different | Attested original opening setup quantity and its stock unit, scoped item/business | Persisted catalog metadata plus linked immutable event for a new stock initialization | Independently recorded stock quantity, not purchase/current/physical or monetary opening balance | Future explicit StockService.SetOpeningStockAsync; absolute initial setup and separate current correction flow | Historical metadata-only import; never set CurrentStock from the old quantity | P + item/unit + optional original event/time; stock-changing capture links movement ID | Yes for old unknown records; known zero valid | NULL, never current stock or first ledger before | MANUAL IMPORT REQUIRED; current reconstruction UNSAFE | Only exact trusted original value and verified unit; no ledger replay from incomplete history | Proposed CatalogItem.OpeningStockQuantity numeric(18,4) nullable and recording/unit metadata; preserve native 4 decimals, source adapter accepts 3 | Source/absence VERIFIED_CODE; design DOCUMENTATION_CLAIM; actual value UNKNOWN/BLOCKED |
| Opening recording time / effective date | opening_stock_set_at is actual UTC recording time; no independent effective date | Missing; StockMovement.CreatedAt is native event time | Original setup timestamp when supplied; actual new capture/import time separately | Persisted source timestamp, not a derived business date | Original record time; an imported-at time does not prove original effective date | New native setup records server UTC now; it does not claim a past date | Original offset timestamp only; missing original time stays NULL, with import time in P | P distinguishes sourceOccurredAt from recordedAt/importedAt | Yes; original effective date remains UNKNOWN when absent | NULL; no purchase/entity timestamp fallback | MANUAL IMPORT REQUIRED | Exact offset-bearing source timestamp; do not convert a calendar date into a guessed midnight instant | Proposed nullable OpeningStockSourceRecordedAtUtc; original actor ref in P, not a forged local UserId | VERIFIED_CODE; independent historical effective date UNKNOWN/BLOCKED |
| Purchase business date | Required TradePurchase.purchase_date Date; date filters/due date use it | Missing; CreatedAt, ConfirmedAt, PaidAt, DispatchedAt, ArrivedAt, VerifiedAt, CompletedAt remain distinct | Original purchase's explicitly identified purchase_date | Persisted DateOnly/PostgreSQL date | Calendar purchase date; not invoice/receipt/completion/report day | Explicit date in existing purchase wizard/server preview; required on future confirm after rollout gate | Exact matched parent purchase; no value inferred from lifecycle timestamps | P at parent field | Nullable for legacy target; required for new source-compatible confirmation | NULL; no today/CreatedAt default | MANUAL IMPORT REQUIRED; timestamp inference UNSAFE | Trusted exact date and parent identity; preserve date without timezone conversion | Proposed PurchaseOrder.BusinessPurchaseDate nullable date, no timestamp replacement | Source/absence VERIFIED_CODE; legacy date UNKNOWN/BLOCKED |
| Line selling rate | Nullable selling_rate/cost numeric(12,2); canonical new-input aliases | Missing; UnitPrice is purchase/landing cost; catalog selling defaults are not transaction history | Original line selling_rate and verified INR/per-line-unit semantics | Persisted transaction snapshot, not current catalog price | Rate attached to that purchase line/business date; not realized sale or profit | Explicit owner entry through existing purchase preview/confirm; known 0 preserved | Canonical source rate; legacy selling_cost accepted only if source version/basis proven; conflicting aliases reject | P + currency INR and rate basis; unresolved source basis blocks KNOWN | Yes, optionally not captured | NULL, never cost/current price/0 | MANUAL IMPORT REQUIRED | Copy exact trusted 2-decimal scalar only; no revenue/profit/tax/stock recalculation | Proposed PurchaseItem.SellingRate numeric(18,2) nullable; no duplicate alias column | Active basis VERIFIED_CODE; unlabelled/conflicting legacy basis UNKNOWN/BLOCKED |
| Historical line item name | Required item_name String(512) on line | Missing; CatalogItem.Name is current display | Transaction's persisted original line item_name | Persisted line snapshot | Name at transaction/corrected source revision, tied to stable item ID | At future confirm capture the server-reviewed line display name from that candidate/version; subsequent catalog rename does not mutate it | Exact matched line/source text; no name-based identity matching | P with original Unicode text | Nullable for legacy; required on new confirmation | NULL; no current-name historical backfill | MANUAL IMPORT REQUIRED | Exact original name only; current catalog can be a labelled current display fallback, never KNOWN history | Proposed PurchaseItem.ItemNameSnapshot nullable varchar(512) | Source/absence VERIFIED_CODE; old name UNKNOWN/BLOCKED |
| Purchase quantity unit | Required persisted line.unit String(32); derived unit_type is separate | PurchaseItem.Unit exists; old migration may be empty | Existing validated native transaction unit, or verified original source line.unit | Persisted label; classification derived separately | Unit of OrderedQuantity on this transaction, not catalog/stock unit or kg geometry | Preserve explicit unit already captured in existing purchase path; never default it to complete missing history | First import slice ASSERT_ONLY: exact known unit must agree; blank/mismatch blocks the row for correction | P records validation/source assertion without replacing existing authority | Source required; native column nonnullable but blank projects UNKNOWN, not known PCS | No new default | NOT APPLICABLE for known native value; missing/changed historical unit MANUAL IMPORT REQUIRED | Do not infer from current DefaultUnit, quantities, labels or migration initializer | No duplicate unit column; dedicated financially/stock-safe correction contract needed before modifying old blank/mismatched Unit | Existing field/migration VERIFIED_CODE; blank units UNKNOWN/BLOCKED |
| Normalized stock quantity and unit | Nullable qty_in_stock_unit snapshot at confirm; reference line does not independently store its target unit label | Missing pair; ReceivedQuantity is a different native quantity | Trusted original normalized snapshot AND proven target stock-unit context at that event | Persisted snapshot pair; conversion is not retroactively inferred | Quantity expressed in event-time stock units; ordered vs received scope remains explicit | Future capture only with validated matching units or an explicit event-time conversion fact; unsupported conversion stays NULL | Exact normalized quantity + original target unit + source quantity scope; an unlabelled number is rejected | P records original quantity scope/units; no current catalog conversion | Both nullable as a pair | NULL, never zero on conversion failure | MANUAL IMPORT REQUIRED | Copy source snapshot only when target-unit/ordered scope provenance is known; don't scale received amounts from ordered snapshots | Proposed PurchaseItem.StockQuantitySnapshot / StockUnitSnapshot nullable, pair constraint; not an SSOT stock write | Snapshot presence VERIFIED_CODE; absent original target-unit/received context UNKNOWN/BLOCKED |
| Historical kg per purchase unit | Nullable line.kg_per_unit/weight_per_unit numeric(12,3); positive, paired weight-price rules | PurchaseItem.KgPerUnit nullable numeric(18,4) exists, coupled to LandingCostPerKg validation | Existing transaction snapshot or original source line geometry, never today's catalog geometry | Existing persisted scalar | Kilograms per that purchase unit, not total kg/stock quantity | Keep existing explicit validated draft capture; freeze at confirm under PurchaseService | First slice ASSERT_ONLY: exact existing positive snapshot must agree. Missing/conflicting geometry cannot be filled by generic metadata import | P + kg/purchase-unit basis; BOX/TIN count-only distinguished | Yes; source BOX/TIN default is NOT_APPLICABLE, not unknown zero | NULL | NOT APPLICABLE if known; missing geometry MANUAL IMPORT REQUIRED | Do not parse current item name or copy CatalogItem.KgPerUnit; correction of this financial input needs its own reviewed contract | No duplicate kg-per-unit column; retain native precision and pricing pairing; correction gate remains blocked | Existing field/source rules VERIFIED_CODE; absent geometry UNKNOWN/BLOCKED |
| Historical total line weight | Nullable total_weight numeric(14,3); ordered line kg; BOX/TIN count-only store no weight | No persisted total-weight snapshot; current reports derive from known line geometry | Trusted stored source total_weight with kg/ordered-line scope; alternatively deterministic validated transaction operands in a future capture | Persisted source snapshot; native safe derived read remains labelled derived | Total kg of the ordered line, not per-pack weight, received kg, physical stock or catalog pack size | Future server confirmation may capture from its exact validated quantity/unit/geometry; no new financial formula | Copy trusted total kg only after unit/quantity match; discrepancy or unknown scope blocks row | P + source scope + original scalar; derived capture lists original line operands/rule version | Yes; count-only NOT_APPLICABLE | NULL; never unknown-to-zero | MANUAL IMPORT REQUIRED for original snapshot; existing valid native operands permit derived read only | Native Qty × KgPerUnit is a derived value, not proof of original stored total_weight; no automated historical fill | Proposed PurchaseItem.TotalWeightKgSnapshot numeric(18,4) nullable, no duplication of existing KgPerUnit | Source/persisted gap VERIFIED_CODE; historical original total UNKNOWN/BLOCKED |

### Opening stock safety contract

Opening in this work means reference catalog opening stock. It is not a financial balance or the existing daily operational opening calculation. The reference stores metadata on CatalogItem and also appends an immutable absolute stock event; it has no separate opening-balance table. Native design reuses the catalog, immutable ledger and StockService rather than creating a second stock engine.

The catalog value is the latest recorded opening setting: the reference overwrites quantity/time/actor on a reasoned correction. A catalog export therefore does not establish the very first opening event. Import must label the source revision it actually proves; recovering an original first event requires that immutable event and its unit/source evidence. No historical effective date is inferred from the latest setting time.

Two intents must remain distinct. HISTORICAL_METADATA imports a trusted old opening fact for reporting/provenance and has writesToCurrentStock=false permanently; it does not replay an old initial-stock event into a live ledger. INITIAL_STOCK_SETUP is a separate future owner action that explicitly previews before/after stock and calls StockService in one transaction. A historical file cannot select this second intent or provide movement before/after/delta fields. No guessed effective date or backdated CreatedAt is used.

The first native initialization requires an active tenant item, valid exact unit, expected RowVersion, no prior opening record, no stock movements and zero current/reserved stock. Nonzero existing stock, reservations or an established ledger require a separate reconciliation decision; they must not be reset to a historical opening. Known opening zero is allowed and must append an explicit zero-delta opening event in a dedicated StockService method; existing AdjustStockAsync rejects zero and must not be weakened. Source correction sets an absolute quantity; target proposes a stricter current correction action with reason, current versions, full before/after preview and a new ledger event, separate from historical metadata correction. This is a documented native safety adaptation.

Duplicate initial setup must return the existing operation or conflict without a second event. Same value with a new source is a reviewable assertion, not a second stock initialization. Concurrent setup competes on catalog version and a database-enforced opening-operation uniqueness key. Correction never updates/deletes the original movement or provenance revision. Bulk import undo is unsupported; a metadata correction or separately previewed StockService reconciliation is required. Historical opening timestamp and actor may stay unknown even when a trusted quantity is known; import time/actor are always actual. A missing original timestamp must not mark an imported quantity as pending new stock initialization.

### Null, unknown and backfill policy

| State | Value / interpretation | Handling |
|---|---|---|
| KNOWN | Valid scalar with traceable native capture or trusted source | Preserve exactly, including a known 0 opening quantity or selling rate; no silent rounding |
| UNKNOWN | Fact should exist but is lost, conflicting or cannot be proved | Historical projection NULL and requiresTrustedCorrection=true; do not silently pick one alias/source or current value |
| NOT_APPLICABLE | Explicit source semantics say the concept does not apply | NULL with a reason code; e.g. per-unit/total kg for reference default count-only BOX/TIN. Existing report display 0 is a count-only aggregation convention, not a weight fact |
| NOT_CAPTURED | Optional field/workflow did not capture a value | NULL with that distinct status. No provenance record can be treated as evidence of a known value |

This is the future read/capture contract, not a change to existing nonnullable columns. Legacy empty Unit remains unchanged in storage; its historical projection/status is UNKNOWN. Field-state provenance records only actual capture/import/correction decisions; existing required native transaction fields with validated persisted values may be labelled NativePersisted, not retroactively claimed as original reference facts. Required source purchase_date/item_name/unit cannot become complete through defaults; legacy records with absent facts keep a visible correction requirement. An import may fill eligible known fields while leaving other legacy facts NULL only when the preview explicitly shows the remaining correction requirement; an attempted KNOWN field without evidence rejects that row.

No automatic historical backfill is approved. CurrentStock → opening, CreatedAt → purchase_date, CatalogItem.Name → old item_name, DefaultUnit → old unit, catalog weight → old geometry and catalog selling defaults → old line rate are UNSAFE BACKFILL. Trusted exact source values are MANUAL IMPORT REQUIRED, not calculated backfills. Existing line Unit/KgPerUnit need no duplicate backfill when genuinely known; import validation may assert equality. Required-but-unknown old values remain NULL/unknown until corrected with trusted provenance. Source maintenance backfill scripts, name parsing, aggregate CSVs, AI/OCR output, or native exports with known gaps are not trusted historical evidence. No script in this phase changes them.

SAFE BACKFILL: none established for the absent original historical facts. NOT APPLICABLE: already validated native transaction Unit/KgPerUnit need no population; a deterministic derived report read does not populate an original snapshot. The matrix's safe-rule column describes prerequisites for trusted manual import, not permission to execute an automatic repair.

### Minimal provenance and database proposal

Reference provides opening actor/time, movement source/id/unit/idempotency and transaction snapshots, but no reusable per-field file-import lineage. The following is the smallest proposed native extension needed to answer where a value came from. It does not claim reference-equivalent provenance tables.

Proposed allowlisted source kinds are NativeCapture, ReferenceDatabaseExport and OwnerAttestedDocument; correction is an operation linked to its earlier revision and new attested source. A checksum identifies bytes and protects replay, but does not establish truth or source authenticity. The authenticated owner must attest the source instance/schema and identifier map; source facts conflicting with native transaction anchors are rejected, even if the file is attested. AI/OCR inference, a current catalog dump and an aggregate report cannot be relabelled as a trusted transaction source.

1. Reuse the semantically correct parent/item/catalog fields named in the matrix; add only missing nullable snapshot/metadata fields later. Do not introduce a SupplierLine, historical catalog copy, duplicate Unit/KgPerUnit or an independent stock table. BusinessPurchaseDate is a PostgreSQL date; source recording timestamps are UTC timestamptz. Never replace lifecycle timestamps or narrow existing numeric(18,4) columns. Source-format imports validate rates at 2 decimal places and quantity/weight at 3; native already-valid 4-decimal values remain unchanged. New native snapshot storage preserves 4 decimals. No default values or data UPDATE in an additive migration.
2. Proposed HistoricalImportBatch contains ID, BusinessId, source kind, template version, file SHA-256, identifier-map digest, validated-payload digest, actual importer ID/time, committed row/field/duplicate/null/warning counts and outcome. File name/full path/raw file are unnecessary; use an import ID and optional bounded non-sensitive source label. Preview creates no batch. A unique (BusinessId, source kind, template version, file hash, mapping digest) protects committed replay; same key with inconsistent payload is a conflict.
3. Proposed HistoricalFieldProvenance is append-only: BusinessId, exactly one typed catalog/purchase/purchase-item FK, allowlisted field, state, bounded original scalar, canonical scalar/type/unit/scope, source kind, optional batch/logical-row/source-field ID, actual capturing actor/time, optional original sourceOccurredAt, prior revision and required correction reason. For file values, original scalar is only the imported allowed cell, not the complete source row. Financial scalars remain owner-only. Direct native capture records the reviewed transaction/source version; it does not claim a file/row. Existing historical actor names are opaque original references, never forged local membership IDs. Superseding revisions preserve original values and source links; a supplied earlier event time never overwrites actual audit time.
4. Reuse SecurityAuditLog for operation-level actor/business/entity/import ID, result, validation summary and request ID. Do not duplicate full field values there, and never store credentials, secrets, tokens, full source rows or unrelated personal data. Stock-changing actions additionally link the immutable movement. Existing SecurityAuditLog is not sufficient alone for immutable original-field lineage; new provenance must reject updates/deletes in application and PostgreSQL controls, with scoped administrative retention handled separately.
5. All new FKs must include business and target ID, using existing composite alternate keys. A provenance row has exactly one target and any prior revision belongs to the same business/target/field; batch and actor-membership relationships are business-scoped. Use restricted deletion to preserve lineage. A unique current-revision/concurrency contract is required, not last-write-wins. Justified indexes are business/import replay, business/import time for bounded history, and business/target/field/revision for provenance lookup; no speculative index on raw values. Parent purchase xmin protects child metadata updates; catalog RowVersion protects opening metadata. Provenance append and parent version change occur together.

No database change, generated SQL, upgrade or migration is executed here. Before the future persistence slice: generate a normal additive EF migration, inspect SQL/defaults/checks/FKs/indexes, test upgrade from the current PostgreSQL schema using isolated fixtures, verify existing fields remain NULL and existing ledger/totals remain byte-for-byte equivalent, and test rollback. Existing migration history is preserved. Those are future acceptance gates, not current verified results.

Any future DTO selling-rate number must use the existing FinancialField classification; operational quantities/weights require explicit OperationalNumeric classification and the existing DTO audit. A new business-date or historical snapshot must not silently switch existing CreatedAt-based report/export/backup ranges or recompute payment due dates. Any later source-date read/filter rollout needs an explicit reviewed contract with legacy NULL behavior, independently of import persistence.

### Controlled file, preview and confirm design

The first format is proposed UTF-8 JSON with a versioned historical-metadata-v1 envelope, explicit source kind, source instance/reference, schema version and rows. Decimal values are invariant strings, dates yyyy-MM-dd and original instants RFC3339 with an offset. A future source adapter can transform an attested reference export into this format, but the adapter must retain field provenance and cannot repair gaps. Existing report/CSV exports are not a reversible historical file format; do not import an aggregate, a current catalog listing or backup schema as if it contained original snapshots. No new source HTTP contract is asserted.

Each row provides a logical source row ID/number, explicit field states/values, target entity ID, parent purchase/item IDs when applicable, expected native version and expected prior field revision; original source IDs and a reviewed source-to-target mapping are retained in its manifest. Source and target UUIDs must not be assumed equal. Direct target IDs are checked against tenant, parent and catalog membership; otherwise a reviewed mapping must resolve them uniquely. OrderNumber is only a tenant-unique parent aid; names, row ordinal, supplier name, price similarity and fuzzy matching cannot identify a line. Repeated item lines need stable line identity. Mixed-business or foreign/inactive targets reject without disclosing foreign values. Source quantity/unit/rate scope assertions must agree with the target transaction before any metadata fill.

Proposed limits: 1 MiB UTF-8 input, at most 1,000 logical rows and bounded field lengths/depth. These are native design limits, not reference requirements. Reject invalid encoding, empty input, malformed/duplicate JSON properties, unknown template/headers/fields/source kinds, unknown IDs, unsupported scope, extra secrets/financial/current-stock write fields, duplicate conflicting targets and unsafe numeric syntax. No archives, remote URLs, workbook formulas, path reads or AI extraction in this first format. All parser/error responses use safe row/field codes and short explanations, not original source payloads. Unicode names are preserved as text, subject to the 512-character source snapshot bound; rendering remains escaped and export formula protection remains in ExportFileBuilder.

Flow: authenticated upload → strict server validation → in-memory preview → owner reviews warnings and proposed changes → explicit confirm referencing that exact preview → reauthorize/revalidate versions → one database transaction → snapshots/provenance/batch/audit → committed summary. These operations are PROPOSED, not callable endpoints in this phase. Extend the existing export validation controller with a narrow coordinator that delegates purchase/catalog/stock ownership; it must not contain new financial formulas or stock setters. Restore dry-run/commit remains its separate existing contract.

Preview performs zero database writes, including no import/provenance/audit records, and retains no original file. A bounded, expiring (proposed 15-minute) server-memory entry holds only the validated allowed scalars/plan, bound to current business, actor, file/mapping/payload digests, target versions and prior revisions. An opaque token is not authorization; it is never logged. Process restart/expiry requires a new preview. Client edits to values, identity map or plan require new validation; the browser cannot supply an executable plan. Commit audits the validation outcome associated with the reviewed preview. Routine preview diagnostics can expose safe counts/request ID only, with no raw field values.

| Preview response element | Required content |
|---|---|
| Summary | totalRows and mutually exclusive validRows/warningRows/rejectedRows/duplicateRows; their sum equals totalRows. Additional fieldsRemainingNull and correction-required counts are separate |
| Rows / proposedChanges | Logical source row/field, verified tenant target IDs, current historical state/value, proposed state/value, source reference, known native version/prior revision and whether stock/totals change (always false in historical metadata import) |
| Errors | Safe row/field code, e.g. UNKNOWN_TARGET, FOREIGN_OR_MISSING_TARGET, REQUIRED_SOURCE_FACT_MISSING, UNIT_CONFLICT, WEIGHT_CONFLICT, UNSAFE_SOURCE, INVALID_DECIMAL, ALIAS_CONFLICT, DUPLICATE_CONFLICT |
| Warnings | Existing missing facts remain NULL; source original time absent; eligible attested manual source needs acknowledgement. Unknown basis/identity/conflicting units are rejection errors, not acknowledgeable warnings |
| Review | Explicit list of warning acknowledgements and fields remaining NULL/correction-required; authenticated reviewer confirms the complete eligible plan, not an unreviewed subset |
| Control | previewToken, digest, expiry, canConfirm, writesPerformed=false; canConfirm=false for any rejected row or unacknowledged required warning. Duplicate-only plan is a no-op summary |

No silent partial success: rejected rows require a corrected/new file or map and another preview. Identical duplicate rows are deterministic no-ops, conflicting duplicates reject. Same file replay returns the existing committed summary, regardless of token/request retries. Commit acquires locks in deterministic target-ID order, verifies selected membership/roles/permissions and native/prior-revision versions again, then inserts one committed batch, fills eligible NULL metadata and appends provenance/audit atomically. A nonnull differing field requires a separate correction preview with reason and expected previous revision; imported values never silently replace current authoritative fields.

For the first mutating slice, Unit, KgPerUnit, OrderedQuantity, ReceivedQuantity, UnitPrice, LandingCostPerKg, LineTotal, Subtotal, GrandTotal, payment and lifecycle/stock fields are ASSERT_ONLY or forbidden. Existing blank Unit/missing KgPerUnit cannot be repaired by this generic importer: those can affect receipt/financial interpretation and need a separately reviewed correction contract. Allowed future fills are new nullable opening/date/name/selling/normalized-stock/total-weight metadata, with exactly proven semantics and identity. Stock-snapshot ordered quantities cannot retroactively normalize received quantities without their own event facts. No money/profit, delivery status, valuation or current stock is recalculated. New capture through existing purchase preview/confirm can store its actual transaction unit/geometry with provenance; that is not a historical backfill.

Any validation/version/unique-key failure or exception rolls back all business data, batch, provenance and success audit writes. A safe failed-confirm audit may be recorded separately after rollback, with only actor/business/request/import-attempt ID, outcome and codes; that is intentional audit-only persistence, not partial import. If audit/provenance cannot persist, no successful commit is reported. Concurrent same-file commit is serialized by the replay uniqueness key; same-field competing imports conflict on native version/prior revision and require new preview. No production confirm path is enabled before these gates and a trusted dataset/identity map are verified.

### Validation and authorization contract

Opening quantity must be finite, nonnegative, in supported range and exactly expressed in a proven unit; known zero is valid. Initial setup validates current stock/reservations/ledger separately. Original opening timestamp requires an explicit offset; unknown source effective date is not manufactured. Business date must be a valid Gregorian yyyy-MM-dd date with declared purchase-date semantics; timezone conversion is not applied. Invoice/receipt dates cannot masquerade as purchase_date. Unknown date stays NULL and correction-required; no default or assumption based on completion date.

Selling rate must be finite, nonnegative, invariant and exactly representable at two decimal places in INR, with purchase-line quantity-unit basis proven. Zero is valid. Conflicting canonical/legacy aliases, currency or rate basis reject; no FX/financial reconstruction. Historical name must be nonblank for KNOWN, Unicode-safe, at most 512 source characters and exactly associated with a stable tenant line. Unit label is at most 32 characters; preserve the original label and separately classify with the verified reference unit rules. Historical SACK aliases may be asserted, not introduced as a new capture option. Unsupported conversions do not return zero.

Per-unit geometry must be positive kg per proven purchase unit and agree with any existing native geometry/rate pair. Total weight must be positive kg for the ordered line with that scope proven; count-only BOX/TIN weight is NULL/NOT_APPLICABLE. Native valid four-decimal authority is not rounded to match a three-decimal source file. Source-format quantities/per-unit weight accept the source numeric(12,3) range, rates numeric(12,2), and total weight numeric(14,3); existing native PurchaseInputLimits also cap any newly stored value. Overprecision is rejected rather than silently quantized; calculations, if future native capture requires them, remain decimal inside existing services with checked bounds. Import never uses binary floating point.

An existing native BOX/TIN geometry value is not deleted to enforce source count-only semantics; it is an explicit native/source conflict requiring review. Future TotalWeightKgSnapshot is reporting metadata and must not change how historical LineTotal was calculated. No supplied kg value is silently converted between per-unit, ordered-total, received-total or stock units.

| Capability (PROPOSED) | Owner / scoped SuperAdmin | Manager | Staff | Existing policy reuse |
|---|---|---|---|---|
| Historical upload/preview/confirm | Allowed only for selected active business with all affected-field permissions | Denied, even with forged/granted import-adjacent permissions | Denied | reports.view plus catalog.edit for opening metadata; purchase.edit + purchase.view for purchase metadata |
| New initial stock setup/current correction | Separate explicit action and preview | Denied for this opening workflow | Denied | stock.adjust + catalog.edit + stock.view; mutation only through StockService |
| Historical metadata correction | Reason, expected revision/native version and separate preview required | Denied | Denied | Same affected-field policies; financial fields Owner/SuperAdmin only |
| Provenance/import history | Tenant-bound, bounded, owner-only including source scalar/rate visibility | Denied | Denied | reports.view plus relevant entity-view policy; no global SuperAdmin bypass of selected membership |
| Bulk import undo/reversal | Unavailable; no verified import-reversal source contract | Unavailable | Unavailable | No new broad permission or fake undo endpoint |

Native future purchase capture must continue existing actor permissions and financial disclosure; it cannot expand Staff/Manager financial authority. Missing roles/policies return safe 403, anonymous 401, missing/foreign targets 404, malformed preview 400, stale versions/duplicate conflicts 409, expired token 410 and field validation errors a safe 422 with row codes. Every source/target/parent/provenance/batch relationship is checked server-side plus composite business FKs. Neither hidden UI controls nor a token proves authorization.

### UI and acceptance gates — not implemented

Reuse existing Settings/Export & Backup and its design components for owner access, with no new sidebar architecture or page redesign. Future upload/preview uses stacked row cards on mobile and a bounded desktop list, with loading/empty/validation/warning/success/error states, labelled controls, keyboard focus/error summary, disabled pending confirmation, 48px targets and a separate clear confirm action. Source/old/new values and NULL states remain readable without horizontal page overflow; long Unicode text wraps. Business/user changes cancel/discard the preview. A stock-initialization confirmation must visibly show current-stock impact; historical metadata import always shows no stock/totals change. This is a UI requirement, not a screen verified in this phase.

| Future test group | Required assertions before implementation is considered available |
|---|---|
| Backend validator / preview | Valid/invalid dates/decimals/quantities/rates/Unicode, empty/malformed/oversize files, state/value consistency, aliases/unit/scope conflicts, secret/unsafe-source rejection, deterministic target matching; preview creates zero business/audit/provenance rows |
| Confirm / provenance | Only reviewed digest and explicit confirm can persist; owner/Manager/Staff and missing-policy/anonymous/foreign negatives; original cell/actor/time/row/field retained, correct NULL state, repeat file no-op, reasoned revisions and no raw secrets |
| PostgreSQL | Additive upgrade NULL preservation, composite-FK tenant rejection, decimal exactness, append-only provenance/ledger, all-or-nothing rollback including simulated audit failure, two-business aggregates unchanged, replay uniqueness and concurrent same-file/same-field conflicts |
| Opening stock | Known zero/positive initial events, duplicate/correction/version/reservation/established-ledger gates, append-only movements, no import bypass and no historical metadata import stock write; unsupported bulk undo remains unavailable |
| Snapshots / existing authority | Catalog rename/unit/weight edits do not rewrite saved name/unit/weight facts; old unknown facts stay unknown; conflicting Unit/KgPerUnit cannot change through metadata import; stored purchase totals/payment/received/current stock remain unchanged |
| Frontend / browser | Upload/preview/error/warning/review/confirm/success/failure/empty/malformed states, no false success, duplicate clicks, cancellation/stale scope; Owner/Manager/Staff across 390x844,393x852,412x915,1366x768,1440x900,1920x1080; keyboard and no overflow |

No new focused importer/database/UI tests were added because those mechanisms do not yet exist; this table is an acceptance plan, not executed evidence. The current phase reruns the complete unchanged backend/frontend/browser regression and builds, and checks that only the three requested documentation files differ from phase-start hashes. The prior 11 live checks and parsed actual CSV artifacts remain prior-phase VERIFIED_RUNTIME; they do not prove import persistence or provenance.

Exact implementation blocker: no verified trusted historical dataset and stable source-to-target line/item/parent map, no persisted import/provenance schema or confirm mechanism, and no correction contract for legacy blank Unit/missing or conflicting financial geometry. Unlabelled legacy selling aliases/rate basis and normalized snapshot target-unit/scope must be resolved per source record; an acknowledgement cannot manufacture them. Application/data implementation stops at these gates, as requested. No existing value is guessed or migrated.

Exact next task: implement only the read-only historical-metadata-v1 validator/preview using isolated synthetic fixtures, the explicit field states and deterministic identity/permission checks above. Include zero-persistence tests, source-conflict/unsafe-backfill rejection and responsive owner preview tests. Keep confirm unavailable and generate no migration until a trusted source manifest/identifier map and the separate Unit/KgPerUnit correction decision are reviewed. An additive persistence migration and explicit confirm are a later, independently verified slice.

- `README.md`: present and read/indexed.
- `AGENTS.md`: present and read/indexed.
- `PLAN.md`: UNKNOWN/BLOCKED: absent at pinned commit.
- `TASKS.md`: present and read/indexed.
- `ARCHITECTURE.md`: present and read/indexed.
- `DESIGN.md`: present and read/indexed.
- `DEPLOYMENT.md`: present and read/indexed.
- `CODE_HYGIENE.md`: present and read/indexed.
- `.cursorrules`: present and read/indexed.
- `.env.example`: present and read/indexed.
- `package.json`: present and read/indexed.
- `docker-compose.yml`: present and read/indexed.
- `backend/sql/MIGRATION_INDEX.md`: present and read/indexed.
- `backend/`, `backend/sql/`, `backend/alembic/`, `backend/app/`: present; tracked sources are included in the source index, with router/model/service/SQL/migration findings below.
- `docs/ai`: UNKNOWN/BLOCKED: absent at pinned commit.
- `docs/TEST_RESULTS.md`: UNKNOWN/BLOCKED: absent at pinned commit.
- `backend/scripts`: present and read/indexed.
- `flutter_app/web`: present and read/indexed.
- `flutter_app/`, `flutter_app/lib/`: present; tracked Dart sources are indexed, with screen/provider/API usage references below.
- `docs/`, `docs/ux/`: present; tracked documentation is indexed. Written claims remain DOCUMENTATION_CLAIM until matched to implementation.
- `uiux context`: present and read/indexed.

`PLAN.md`, `docs/ai/`, `docs/TEST_RESULTS.md` are absent despite README links. No invented roadmap or test sign-off is used. SQL migration index says head 062, but actual revision files reach 070; index is stale. Deployment docs conflict between Render and Windows host; production environment is not verified.

## Financial source finding

`backend/app/services/line_totals_service.py::line_gross_base` uses qty × kg_per_unit × landing_cost_per_kg only when both weight fields are positive and their derived unit cost agrees with purchase_rate/landing_cost within ₹0.05; otherwise qty × purchase_rate/landing_cost. `line_money` applies line discount then tax. `trade_purchase_service.compute_totals` adds header charge/commission rules; `aggregate_totals_service.py` is separate landing/selling/profit roll-up. `trade_query.py` and `test_trade_query_vs_line_totals_parity.py` provide report parity evidence (test source, not run). Target UnitPrice currently acts as manual unit landing cost; no automatic conversion or AI-derived financial value is authorized. Full weight/discount/tax/header parity remains a gap until implemented/tested.

## Categories A–BD

### A. Authentication

- Feature scope / gap: Refresh rotation is not atomic; active membership is reset to first business on refresh. Registration/Google auth have no target equivalent. Target recovery endpoints existed, but deeper target review found reset-password ignored its token; this unsafe path is now disabled. This is a correction to the original target baseline finding.
- Reference implementation: `backend/app/routers/auth.py`, `backend/app/routers/me.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/auth/register`; `POST /v1/auth/forgot-password`; `POST /v1/auth/reset-password`; `POST /v1/auth/login`; `POST /v1/auth/google`; `POST /v1/auth/refresh`; `GET /v1/me/profile`; `PATCH /v1/me/profile`; `POST /v1/me/bootstrap-workspace`; `GET /v1/me/businesses`; `PATCH /v1/me/businesses/{business_id}/branding`; `POST /v1/me/businesses/{business_id}/branding/logo`.
- Database entities/tables: User (`users`; `backend/app/models/user.py:12`); UserSession (`user_sessions`; `backend/app/models/user_session.py:16`); PasswordResetToken (`password_reset_tokens`; `backend/app/models/password_reset.py:21`); Membership (`memberships`; `backend/app/models/membership.py:12`)
- Frontend screen/component: `flutter_app/lib/features/auth/presentation/login_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:335`); `flutter_app/lib/features/auth/presentation/forgot_password_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:342`); `flutter_app/lib/features/auth/presentation/reset_password_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:349`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Controllers/AuthController.cs`; frontend `frontend/src/pages/auth/Login.tsx` (module/path search; exact component absent).
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### B. Authorization / RBAC

- Feature scope / gap: Permission handler trusts JWT claims without checking current membership/account state; monetary payloads are not owner-protected.
- Reference implementation: `backend/app/routers/owner_ops.py`, `backend/app/routers/users.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/settings/credentials`; `PUT /v1/businesses/{business_id}/settings/credentials/{credential_type}`; `GET /v1/businesses/{business_id}/staff/tasks`; `GET /v1/businesses/{business_id}/staff/{staff_id}/tasks`; `POST /v1/businesses/{business_id}/staff/tasks`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete`; `GET /v1/businesses/{business_id}/staff/performance-summary`; `GET /v1/businesses/{business_id}/owner/dashboard`; `GET /v1/businesses/{business_id}/owner/whatsapp-deliveries`; `POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend`; `POST /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users/active-sessions`; `POST /v1/businesses/{business_id}/users/bulk`; `GET /v1/businesses/{business_id}/users/{user_id}`; `PATCH /v1/businesses/{business_id}/users/{user_id}`; `DELETE /v1/businesses/{business_id}/users/{user_id}`; `POST /v1/businesses/{business_id}/users/{user_id}/reset-password`; `GET /v1/businesses/{business_id}/users/{user_id}/credentials`; `GET /v1/businesses/{business_id}/users/{user_id}/created-items`; `GET /v1/businesses/{business_id}/users/{user_id}/stock-adjustments`; `GET /v1/businesses/{business_id}/users/{user_id}/purchases`; `GET /v1/businesses/{business_id}/users/{user_id}/ledger`; `GET /v1/businesses/{business_id}/users/{user_id}/permissions`; `PATCH /v1/businesses/{business_id}/users/{user_id}/permissions`; `POST /v1/businesses/{business_id}/activity-log`; `GET /v1/businesses/{business_id}/activity-log`.
- Database entities/tables: User (`users`; `backend/app/models/user.py:12`); Membership (`memberships`; `backend/app/models/membership.py:12`); UserSession (`user_sessions`; `backend/app/models/user_session.py:16`); StaffActivityLog (`staff_activity_log`; `backend/app/models/user_session.py:32`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/user_management_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:870`); `flutter_app/lib/features/staff/presentation/staff_shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1329`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Authorization/PermissionAuthorizationHandler.cs`, `backend/PurchaseAssistant.UnitTests/Auth/PermissionAuthorizationHandlerTests.cs`; frontend `frontend/src/auth/Guards.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### C. Business / Tenant Management

- Feature scope / gap: Selection exists; tenant-owned FK validation and global user mutations need hardening. No business profile editor.
- Reference implementation: `backend/app/routers/me.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/me/profile`; `PATCH /v1/me/profile`; `POST /v1/me/bootstrap-workspace`; `GET /v1/me/businesses`; `PATCH /v1/me/businesses/{business_id}/branding`; `POST /v1/me/businesses/{business_id}/branding/logo`.
- Database entities/tables: Business (`businesses`; `backend/app/models/business.py:10`); Membership (`memberships`; `backend/app/models/membership.py:12`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/business_profile_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:822`); `flutter_app/lib/features/settings/presentation/user_profile_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:883`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_business_profile.py`, `backend/tests/test_users_management.py`, `flutter_app/test/user_management_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Controllers/AuthController.cs`; frontend `frontend/src/auth/AuthProvider.tsx`.
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### D. Dashboard

- Feature scope / gap: Operational/financial bundles exist; no owner command center, goals or staff task summary.
- Reference implementation: `backend/app/routers/owner_ops.py`, `backend/app/routers/reports_trade.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/settings/credentials`; `PUT /v1/businesses/{business_id}/settings/credentials/{credential_type}`; `GET /v1/businesses/{business_id}/staff/tasks`; `GET /v1/businesses/{business_id}/staff/{staff_id}/tasks`; `POST /v1/businesses/{business_id}/staff/tasks`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete`; `GET /v1/businesses/{business_id}/staff/performance-summary`; `GET /v1/businesses/{business_id}/owner/dashboard`; `GET /v1/businesses/{business_id}/owner/whatsapp-deliveries`; `POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend`; `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`.
- Database entities/tables: BusinessGoal (`business_goals`; `backend/app/models/business_goal.py:16`); TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); StaffTask (`staff_tasks`; `backend/app/models/owner_ops.py:70`)
- Frontend screen/component: `flutter_app/lib/features/home/presentation/home_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1234`); `flutter_app/lib/features/settings/presentation/owner_command_center_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:846`); `flutter_app/lib/features/staff/presentation/staff_home_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1342`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_owner_dashboard_dup_b002.py`, `flutter_app/test/home_desktop_dashboard_grid_test.dart`, `flutter_app/test/home_owner_dashboard_body_smoke_test.dart`, `flutter_app/test/low_stock_dashboard_load_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/DashboardService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IDashboardService.cs`; frontend `frontend/src/pages/Dashboard.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### E. Catalog

- Feature scope / gap: Category/type foreign keys can reference another tenant. Full reference metadata/bulk operations not mapped.
- Reference implementation: `backend/app/routers/catalog.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); CatalogItemDefaultSupplier (`catalog_item_default_suppliers`; `backend/app/models/catalog.py:153`); CatalogItemDefaultBroker (`catalog_item_default_brokers`; `backend/app/models/catalog.py:172`)
- Frontend screen/component: `flutter_app/lib/features/catalog/presentation/catalog_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:373`); `flutter_app/lib/features/catalog/presentation/catalog_item_create_page.dart` (constructor referenced in `flutter_app/lib/features/catalog/presentation/catalog_add_item_page.dart:22`); `flutter_app/lib/features/catalog/presentation/item_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:568`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_catalog.py`, `backend/tests/test_catalog_insights.py`, `backend/tests/test_catalog_item_code_auto.py`, `backend/tests/test_catalog_item_unit_simplify.py`, `flutter_app/test/catalog_categories_load_error_test.dart`, `flutter_app/test/catalog_category_detail_empty_test.dart`, `flutter_app/test/catalog_category_trade_summary_error_test.dart`, `flutter_app/test/catalog_category_types_load_error_test.dart`, `flutter_app/test/catalog_duplicates_empty_test.dart`, `flutter_app/test/catalog_duplicates_load_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ICatalogService.cs`; frontend `frontend/src/pages/catalog/CatalogList.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### F. Categories

- Feature scope / gap: CRUD exists; category summaries and bulk behavior differ.
- Reference implementation: `backend/app/routers/catalog.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`.
- Database entities/tables: ItemCategory (`item_categories`; `backend/app/models/catalog.py:15`)
- Frontend screen/component: `flutter_app/lib/features/catalog/presentation/catalog_add_category_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:517`); `flutter_app/lib/features/catalog/presentation/catalog_taxonomy_hub_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:510`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `flutter_app/test/catalog_categories_load_error_test.dart`, `flutter_app/test/quick_catalog_taxonomy_categories_error_test.dart`, `flutter_app/test/supplier_wizard_categories_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/CategoryService.cs`; frontend `frontend/src/pages/catalog/CategoryList.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### G. Types

- Feature scope / gap: CRUD exists; reference grouping/summary requires mapping.
- Reference implementation: `backend/app/routers/catalog.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`.
- Database entities/tables: CategoryType (`category_types`; `backend/app/models/catalog.py:30`)
- Frontend screen/component: `flutter_app/lib/features/catalog/presentation/catalog_add_subcategory_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:529`); `flutter_app/lib/features/catalog/presentation/catalog_type_items_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:716`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_staff_activity_action_types_stock.py`, `flutter_app/test/catalog_category_types_load_error_test.dart`, `flutter_app/test/supplier_wizard_types_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/CategoryTypeService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ICategoryTypeService.cs`; frontend `frontend/src/pages/catalog/TypeList.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### H. Variants

- Feature scope / gap: Target model exists but no variant CRUD service/router/UI verified; reference catalog.py implements CatalogVariant CRUD; target has only model/DTO/placeholder UI.
- Reference implementation: `backend/app/routers/catalog.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`.
- Database entities/tables: CatalogVariant (`catalog_variants`; `backend/app/models/catalog.py:191`)
- Frontend screen/component: `flutter_app/lib/features/catalog/presentation/item_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:568`); `flutter_app/lib/features/catalog/presentation/item_edit_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:583`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ICatalogService.cs`; frontend `frontend/src/pages/catalog/CatalogDetail.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### I. Suppliers

- Feature scope / gap: CRUD exists; linked items/default rates/history/ledger missing.
- Reference implementation: `backend/app/routers/contacts.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/suppliers`; `POST /v1/businesses/{business_id}/suppliers`; `PATCH /v1/businesses/{business_id}/suppliers/{supplier_id}`; `DELETE /v1/businesses/{business_id}/suppliers/{supplier_id}`; `GET /v1/businesses/{business_id}/brokers`; `POST /v1/businesses/{business_id}/brokers`; `PATCH /v1/businesses/{business_id}/brokers/{broker_id}`; `DELETE /v1/businesses/{business_id}/brokers/{broker_id}`; `GET /v1/businesses/{business_id}/brokers/{broker_id}`; `GET /v1/businesses/{business_id}/brokers/{broker_id}/linked-suppliers`; `GET /v1/businesses/{business_id}/suppliers/{supplier_id}`; `GET /v1/businesses/{business_id}/suppliers/{supplier_id}/metrics`; `GET /v1/businesses/{business_id}/brokers/{broker_id}/metrics`; `GET /v1/businesses/{business_id}/contacts/search`; `GET /v1/businesses/{business_id}/contacts/category-items`.
- Database entities/tables: Supplier (`suppliers`; `backend/app/models/contacts.py:38`); SupplierItemDefault (`supplier_item_defaults`; `backend/app/models/supplier_item_default.py:17`); CatalogItemDefaultSupplier (`catalog_item_default_suppliers`; `backend/app/models/catalog.py:153`)
- Frontend screen/component: `flutter_app/lib/features/contacts/presentation/contacts_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:362`); `flutter_app/lib/features/contacts/presentation/supplier_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:729`); `flutter_app/lib/features/contacts/presentation/supplier_create_wizard_page.dart` (constructor referenced in `flutter_app/lib/features/contacts/presentation/contacts_page.dart:683`); `flutter_app/lib/features/supplier/presentation/supplier_ledger_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:742`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_broker_linked_suppliers.py`, `backend/tests/test_suppliers_list_compact.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/SupplierService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ISupplierService.cs`; frontend `frontend/src/pages/suppliers/SupplierList.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### J. Brokers

- Feature scope / gap: CRUD exists; linked suppliers/deal defaults/history missing.
- Reference implementation: `backend/app/routers/contacts.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/suppliers`; `POST /v1/businesses/{business_id}/suppliers`; `PATCH /v1/businesses/{business_id}/suppliers/{supplier_id}`; `DELETE /v1/businesses/{business_id}/suppliers/{supplier_id}`; `GET /v1/businesses/{business_id}/brokers`; `POST /v1/businesses/{business_id}/brokers`; `PATCH /v1/businesses/{business_id}/brokers/{broker_id}`; `DELETE /v1/businesses/{business_id}/brokers/{broker_id}`; `GET /v1/businesses/{business_id}/brokers/{broker_id}`; `GET /v1/businesses/{business_id}/brokers/{broker_id}/linked-suppliers`; `GET /v1/businesses/{business_id}/suppliers/{supplier_id}`; `GET /v1/businesses/{business_id}/suppliers/{supplier_id}/metrics`; `GET /v1/businesses/{business_id}/brokers/{broker_id}/metrics`; `GET /v1/businesses/{business_id}/contacts/search`; `GET /v1/businesses/{business_id}/contacts/category-items`.
- Database entities/tables: Broker (`brokers`; `backend/app/models/contacts.py:15`); BrokerSupplierLink (`broker_supplier_m2m`; `backend/app/models/trade_purchase.py:18`); CatalogItemDefaultBroker (`catalog_item_default_brokers`; `backend/app/models/catalog.py:172`)
- Frontend screen/component: `flutter_app/lib/features/contacts/presentation/broker_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:768`); `flutter_app/lib/features/contacts/presentation/broker_wizard_page.dart` (constructor referenced in `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart:956`); `flutter_app/lib/features/broker/presentation/broker_history_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:781`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/BrokerService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IBrokerService.cs`; frontend `frontend/src/pages/brokers/BrokerList.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### K. Search

- Feature scope / gap: Tenant-scoped search exists; full reference filter/role semantics not runtime-proven.
- Reference implementation: `backend/app/routers/search.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/search`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); Supplier (`suppliers`; `backend/app/models/contacts.py:38`); Broker (`brokers`; `backend/app/models/contacts.py:15`); ItemCategory (`item_categories`; `backend/app/models/catalog.py:15`); CategoryType (`category_types`; `backend/app/models/catalog.py:30`)
- Frontend screen/component: `flutter_app/lib/features/search/presentation/search_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1316`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_unified_search.py`, `flutter_app/test/catalog_search_suggestions_error_test.dart`, `flutter_app/test/contacts_catalog_search_empty_test.dart`, `flutter_app/test/inline_search_field_on_selected_once_test.dart`, `flutter_app/test/purchase_pdf_search_parity_test.dart`, `flutter_app/test/reports_filter_search_empty_test.dart`, `flutter_app/test/search_load_error_test.dart`, `flutter_app/test/search_loading_slow_fallback_empty_test.dart`, `flutter_app/test/search_section_filter_chips_ia_test.dart`, `flutter_app/test/search_zero_results_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/GlobalSearchService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IGlobalSearchService.cs`; frontend `frontend/src/components/search/GlobalSearch.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### L. Barcode

- Feature scope / gap: Tenant unique index and manual lookup/assignment exist; no camera decoder or reference scanner state machine.
- Reference implementation: `backend/app/routers/catalog.py`, `backend/app/routers/stock/stock_barcode.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `GET /barcode/lookup`; `GET /barcode/{item_id}`; `POST /barcode/batch`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); CatalogVariant (`catalog_variants`; `backend/app/models/catalog.py:191`); StockAudit (`stock_audits`; `backend/app/models/stock_audit.py:11`); StockAuditItem (`stock_audit_items`; `backend/app/models/stock_audit.py:43`)
- Frontend screen/component: `flutter_app/lib/features/barcode/presentation/barcode_scan_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:398`); `flutter_app/lib/features/barcode/barcode_scan_controller.dart` (constructor referenced in `flutter_app/lib/features/barcode/presentation/barcode_scan_page.dart:43`); `flutter_app/lib/features/barcode/barcode_lookup_cache.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/barcode/presentation/barcode_print_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:434`); `flutter_app/lib/features/catalog/presentation/barcode_quick_create_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:474`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_barcode_item_code.py`, `backend/tests/test_barcode_lookup_ambiguous.py`, `backend/tests/test_barcode_lookup_cache.py`, `flutter_app/test/barcode_camera_start_gate_empty_test.dart`, `flutter_app/test/barcode_op_log_sanitize_test.dart`, `flutter_app/test/barcode_operation_errors_test.dart`, `flutter_app/test/barcode_print_empty_test.dart`, `flutter_app/test/barcode_print_load_error_test.dart`, `flutter_app/test/barcode_recent_scans_chips_ia_test.dart`, `flutter_app/test/barcode_scan_history_audit_kpi_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ICatalogService.cs`; frontend `frontend/src/pages/catalog/BarcodeManager.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### M. Stock

- Feature scope / gap: Ledger service exists; validation/transaction disposal and concurrency errors require audit.
- Reference implementation: `backend/app/routers/stock/stock_detail.py`, `backend/app/routers/stock/stock_list.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /items/{item_id}/purchase-intelligence`; `GET /{item_id}/activity`; `GET /{item_id}/intelligence`; `GET /item/{item_id}/summary`; `GET /{item_id}/bundle`; `GET /{item_id}`; `POST /{item_id}/opening-stock`; `POST /{item_id}/physical-count`; `POST /{item_id}/physical-update`; `POST /{item_id}/verify-count`; `PATCH /{item_id}`; `POST /{item_id}/undo-last`; `POST /{item_id}/notify-owner`; `GET /list`; `GET /shell-bundle`; `GET /delivery-indicator-counts`; `GET /list/compact`; `GET /search`; `GET /alerts/summary`; `GET /warehouse/alerts-summary`; `GET /low-stock/summary`; `GET /low-stock/operations`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`); StockAdjustmentLog (`stock_adjustment_log`; `backend/app/models/stock_adjustment.py:16`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/stock_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1268`); `flutter_app/lib/features/stock/presentation/update_stock_sheet.dart` (source exists; route/constructor wiring UNKNOWN) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_low_stock_operations.py`, `backend/tests/test_low_stock_priority.py`, `backend/tests/test_opening_stock_setup.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_staff_system_stock_notify.py`, `backend/tests/test_stock_alerts_summary.py`, `backend/tests/test_stock_audit.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.UnitTests/Services/StockServiceTests.cs`, `backend/PurchaseAssistant.Infrastructure/Services/StockService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IStockService.cs`; frontend `frontend/src/pages/stock/StockList.tsx`.
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### N. Physical Stock

- Feature scope / gap: Negative physical counts accepted by service; no count idempotency key.
- Reference implementation: `backend/app/routers/stock/stock_ops.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /opening/setup`; `GET /inventory-summary`; `GET /totals`; `GET /reorder`; `PATCH /reorder/{entry_id}`; `DELETE /reorder/{entry_id}`; `GET /opening/missing`; `POST /{item_id}/quick-purchase`; `POST /{item_id}/reorder`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); StockPhysicalCount (`stock_physical_counts`; `backend/app/models/stock_physical_count.py:15`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/update_stock_sheet.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/barcode/presentation/stock_audit_session_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:414`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_low_stock_operations.py`, `backend/tests/test_low_stock_priority.py`, `backend/tests/test_opening_stock_setup.py`, `backend/tests/test_physical_count_diff_sign.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_staff_system_stock_notify.py`, `backend/tests/test_stock_alerts_summary.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.UnitTests/Services/StockServiceTests.cs`, `backend/PurchaseAssistant.Infrastructure/Services/StockService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IStockService.cs`; frontend `frontend/src/pages/stock/StockDetail.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### O. System Stock

- Feature scope / gap: System/physical/reserved quantities exist; manual system adjustment guarded by policies.
- Reference implementation: `backend/app/routers/stock/stock_ops.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /opening/setup`; `GET /inventory-summary`; `GET /totals`; `GET /reorder`; `PATCH /reorder/{entry_id}`; `DELETE /reorder/{entry_id}`; `GET /opening/missing`; `POST /{item_id}/quick-purchase`; `POST /{item_id}/reorder`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`); StockAdjustmentLog (`stock_adjustment_log`; `backend/app/models/stock_adjustment.py:16`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/stock_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1268`); `flutter_app/lib/features/stock/presentation/update_stock_sheet.dart` (source exists; route/constructor wiring UNKNOWN) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_low_stock_operations.py`, `backend/tests/test_low_stock_priority.py`, `backend/tests/test_opening_stock_setup.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_staff_system_stock_notify.py`, `backend/tests/test_stock_alerts_summary.py`, `backend/tests/test_stock_audit.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.UnitTests/Services/StockServiceTests.cs`, `backend/PurchaseAssistant.Infrastructure/Services/StockService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IStockService.cs`; frontend `frontend/src/pages/stock/StockDetail.tsx`.
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### P. Stock Movements

- Feature scope / gap: Movement ledger exists; purchase reference IDs and DB immutability guard missing.
- Reference implementation: `backend/app/routers/stock/stock_ops.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /opening/setup`; `GET /inventory-summary`; `GET /totals`; `GET /reorder`; `PATCH /reorder/{entry_id}`; `DELETE /reorder/{entry_id}`; `GET /opening/missing`; `POST /{item_id}/quick-purchase`; `POST /{item_id}/reorder`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`); StockAdjustmentLog (`stock_adjustment_log`; `backend/app/models/stock_adjustment.py:16`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/stock_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1268`); `flutter_app/lib/features/stock/presentation/update_stock_sheet.dart` (source exists; route/constructor wiring UNKNOWN) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_low_stock_operations.py`, `backend/tests/test_low_stock_priority.py`, `backend/tests/test_opening_stock_setup.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_staff_system_stock_notify.py`, `backend/tests/test_stock_alerts_summary.py`, `backend/tests/test_stock_audit.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.UnitTests/Services/StockServiceTests.cs`, `backend/PurchaseAssistant.Infrastructure/Services/StockService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IStockService.cs`; frontend `frontend/src/pages/stock/StockActivity.tsx`.
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### Q. Stock Adjustments

- Feature scope / gap: Versioned adjustment/reconciliation exists; reason/quantity validation incomplete.
- Reference implementation: `backend/app/routers/stock/stock_adjustments.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /audit/feed`; `GET /audit/recent`; `GET /physical-counts/recent`; `GET /physical-counts/by-item/{item_id}`; `GET /variances/today`; `GET /audit/{item_id}`; `GET /staff-purchases`; `POST /staff-purchases`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`); StockAdjustmentLog (`stock_adjustment_log`; `backend/app/models/stock_adjustment.py:16`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/stock_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1268`); `flutter_app/lib/features/stock/presentation/update_stock_sheet.dart` (source exists; route/constructor wiring UNKNOWN) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_low_stock_operations.py`, `backend/tests/test_low_stock_priority.py`, `backend/tests/test_opening_stock_setup.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_staff_system_stock_notify.py`, `backend/tests/test_stock_alerts_summary.py`, `backend/tests/test_stock_audit.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.UnitTests/Services/StockServiceTests.cs`, `backend/PurchaseAssistant.Infrastructure/Services/StockService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IStockService.cs`; frontend `frontend/src/pages/stock/StockDetail.tsx`.
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### R. Low Stock

- Feature scope / gap: Filtering exists; reference urgency/reorder list/supplier enrichments missing; reports use inconsistent reserved-stock calculation.
- Reference implementation: `backend/app/routers/stock/stock_list.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /list`; `GET /shell-bundle`; `GET /delivery-indicator-counts`; `GET /list/compact`; `GET /search`; `GET /alerts/summary`; `GET /warehouse/alerts-summary`; `GET /low-stock/summary`; `GET /low-stock/operations`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); ReorderListEntry (`reorder_list`; `backend/app/models/reorder_list.py:11`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/low_stock_dashboard_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:637`); `flutter_app/lib/features/stock/presentation/reorder_list_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:616`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_low_stock_operations.py`, `backend/tests/test_low_stock_priority.py`, `backend/tests/test_opening_stock_setup.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_staff_system_stock_notify.py`, `backend/tests/test_stock_alerts_summary.py`, `backend/tests/test_stock_audit.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.UnitTests/Services/StockServiceTests.cs`, `backend/PurchaseAssistant.Infrastructure/Services/StockService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IStockService.cs`; frontend `frontend/src/pages/stock/StockDashboard.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### S. Purchases

- Feature scope / gap: Server unit totals exist; weight pricing, preview, concurrency and lifecycle guards incomplete.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`)
- Frontend screen/component: `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1017`); `flutter_app/lib/features/purchase/presentation/purchase_detail_page.dart` (constructor referenced in `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart:14`); `flutter_app/lib/features/staff/presentation/staff_pending_deliveries_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:894`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_trade_purchases.py`, `backend/tests/test_trade_purchases_openapi.py`, `backend/tests/test_user_purchases_list.py`, `flutter_app/test/dup_f005_trade_purchases_dedupe_test.dart`, `flutter_app/test/reports_purchases_supplier_ranking_empty_test.dart`, `flutter_app/test/reports_purchases_tab_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseForm.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### T. Purchase Draft

- Feature scope / gap: Draft PO exists; per-user wizard step restore is absent.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchaseDraft (`trade_purchase_drafts`; `backend/app/models/trade_purchase.py:154`)
- Frontend screen/component: `flutter_app/lib/features/purchase/state/purchase_draft_provider.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/purchase/state/purchase_local_wip_draft_provider.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/purchase/presentation/widgets/resume_purchase_draft_banner.dart` (constructor referenced in `flutter_app/lib/features/home/presentation/home_page.dart:622`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `backend/tests/test_trade_purchases.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseForm.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### U. Purchase Preview

- Feature scope / gap: No nonmutating authoritative preview endpoint or preview review step.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`)
- Frontend screen/component: `flutter_app/lib/features/purchase/state/purchase_trade_preview_provider.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1017`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_preview.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseForm.tsx`.
- Target initial status: **MISSING**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### V. Purchase Confirmation

- Feature scope / gap: Explicit status button exists but can bypass preview and state machine.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`)
- Frontend screen/component: `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1017`); `flutter_app/lib/features/purchase/presentation/purchase_detail_page.dart` (constructor referenced in `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart:14`); `flutter_app/lib/features/staff/presentation/staff_pending_deliveries_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:894`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `backend/tests/test_trade_purchases.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### W. Purchase Lifecycle

- Feature scope / gap: Arbitrary Verified/Completed/Cancelled/Draft transitions accepted; no lifecycle event ledger.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`)
- Frontend screen/component: `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1017`); `flutter_app/lib/features/purchase/presentation/purchase_detail_page.dart` (constructor referenced in `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart:14`); `flutter_app/lib/features/staff/presentation/staff_pending_deliveries_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:894`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `backend/tests/test_trade_purchases.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### X. Purchase Receiving

- Feature scope / gap: StockService is correctly used but over-receipt/repeated receipt and purchase concurrency are unguarded.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`)
- Frontend screen/component: `flutter_app/lib/features/staff/presentation/staff_receive_shipment_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:907`); `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:966`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `backend/tests/test_trade_purchases.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### Y. Purchase Verification

- Feature scope / gap: Enum exists; no dedicated staff verification workflow.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`)
- Frontend screen/component: `flutter_app/lib/features/staff/presentation/staff_receive_shipment_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:907`); `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:966`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `backend/tests/test_trade_purchases.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **MISSING**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### Z. Payment Lifecycle

- Feature scope / gap: Pending/Partial/Paid enum exists without payment amount or endpoint; reference due-date derived state absent.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`)
- Frontend screen/component: `flutter_app/lib/features/purchase/presentation/purchase_detail_page.dart` (constructor referenced in `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart:14`); `flutter_app/lib/features/contacts/presentation/trade_ledger_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:686`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_trade_list_payment_filters.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **MISSING**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AA. Delivery Lifecycle

- Feature scope / gap: Receipt partial/delivered states exist; no reference delivery date/carrier/dispatch-note operations.
- Reference implementation: `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`)
- Frontend screen/component: `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1017`); `flutter_app/lib/features/purchase/presentation/purchase_detail_page.dart` (constructor referenced in `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart:14`); `flutter_app/lib/features/staff/presentation/staff_pending_deliveries_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:894`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_stock_delivery_indicator_counts.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `flutter_app/test/delivery_invalidation_test.dart`, `flutter_app/test/home_delivery_pipeline_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AB. Damage Management

- Feature scope / gap: Reference damage reports/resolution are implemented; target has no damage entity/service/UI.
- Reference implementation: `backend/app/routers/damage_reports.py`, `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/damage-reports/pending-count`; `PATCH /v1/businesses/{business_id}/damage-reports/{report_id}`; `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: PurchaseDamageReport (`purchase_damage_reports`; `backend/app/models/purchase_damage_report.py:15`); TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`)
- Frontend screen/component: `flutter_app/lib/features/staff/presentation/staff_receive_shipment_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:907`); `flutter_app/lib/features/purchase/presentation/purchase_detail_page.dart` (constructor referenced in `flutter_app/lib/features/staff/presentation/staff_purchase_order_detail_page.dart:14`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_users_management.py`, `flutter_app/test/purchase_detail_damage_empty_test.dart`, `flutter_app/test/user_management_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/PurchaseService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IPurchaseService.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **MISSING**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AC. Daily Operations

Current target override: the core checklist, assignment lifecycle, cumulative stock-safe usage, snapshot history and operational report contract is implemented and verified in the 2026-10-02 checkpoint above. The following feature/target-status lines describe the initial audit only. WhatsApp delivery routes listed here belong to the separately deferred AQ integration scope.

- Feature scope / gap: Reference checklists, daily usage, snapshots and staff tasks have no target equivalents.
- Reference implementation: `backend/app/routers/operations.py`, `backend/app/routers/owner_ops.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/operations/checklist/today`; `POST /v1/businesses/{business_id}/operations/checklist/{slot}/complete`; `GET /v1/businesses/{business_id}/operations/usage/today`; `POST /v1/businesses/{business_id}/operations/usage/today`; `GET /v1/businesses/{business_id}/operations/checklist/templates`; `PUT /v1/businesses/{business_id}/operations/checklist/templates`; `GET /v1/businesses/{business_id}/operations/checklist/summary`; `POST /v1/businesses/{business_id}/operations/snapshots/materialize`; `GET /v1/businesses/{business_id}/operations/snapshots`; `GET /v1/businesses/{business_id}/operations/usage/summary`; `GET /v1/businesses/{business_id}/operations/reports/summary`; `GET /v1/businesses/{business_id}/settings/credentials`; `PUT /v1/businesses/{business_id}/settings/credentials/{credential_type}`; `GET /v1/businesses/{business_id}/staff/tasks`; `GET /v1/businesses/{business_id}/staff/{staff_id}/tasks`; `POST /v1/businesses/{business_id}/staff/tasks`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete`; `GET /v1/businesses/{business_id}/staff/performance-summary`; `GET /v1/businesses/{business_id}/owner/dashboard`; `GET /v1/businesses/{business_id}/owner/whatsapp-deliveries`; `POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend`.
- Database entities/tables: DailyUsageLog (`daily_usage_logs`; `backend/app/models/operations.py:15`); StaffChecklistTemplate (`staff_checklist_templates`; `backend/app/models/operations.py:40`); StaffChecklistCompletion (`staff_checklist_completions`; `backend/app/models/operations.py:56`); StaffTask (`staff_tasks`; `backend/app/models/owner_ops.py:70`)
- Frontend screen/component: `flutter_app/lib/features/operations/presentation/daily_usage_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1116`); `flutter_app/lib/features/operations/presentation/staff_checklist_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1123`); `flutter_app/lib/features/operations/presentation/owner_tasks_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1130`); `flutter_app/lib/features/staff/presentation/staff_tasks_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:854`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_checklist_operations.py`, `backend/tests/test_low_stock_operations.py`, `flutter_app/test/daily_usage_empty_test.dart`, `flutter_app/test/daily_usage_load_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/DashboardService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IDashboardService.cs`; frontend `frontend/src/pages/Dashboard.tsx`.
- Target initial status: **MISSING**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AD. Notifications

- Feature scope / gap: Read/list exists; no scheduled emission jobs verified in target.
- Reference implementation: `backend/app/routers/notifications.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/notifications`; `GET /v1/businesses/{business_id}/notifications/summary`; `GET /v1/businesses/{business_id}/notifications/unread-count`; `POST /v1/businesses/{business_id}/notifications/mark-all-read`; `DELETE /v1/businesses/{business_id}/notifications/clear-all`; `POST /v1/businesses/{business_id}/notifications/client-event`; `PATCH /v1/businesses/{business_id}/notifications/{notification_id}`.
- Database entities/tables: AppNotification (`notifications`; `backend/app/models/notification.py:16`)
- Frontend screen/component: `flutter_app/lib/features/notifications/presentation/notifications_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1109`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_notifications.py`, `flutter_app/test/notifications_category_filter_chips_ia_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/NotificationService.cs`, `backend/PurchaseAssistant.Application/Interfaces/INotificationService.cs`; frontend `frontend/src/pages/NotificationsPage.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AE. Reports

- Feature scope / gap: Target includes drafts in spend; server financial protection and safe errors need verification.
- Reference implementation: `backend/app/routers/report_views.py`, `backend/app/routers/reports_trade.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/report-views`; `POST /v1/businesses/{business_id}/report-views`; `PATCH /v1/businesses/{business_id}/report-views/{view_id}`; `DELETE /v1/businesses/{business_id}/report-views/{view_id}`; `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); ReportSavedView (`report_saved_views`; `backend/app/models/report_saved_view.py:15`)
- Frontend screen/component: `flutter_app/lib/features/reports/shell/reports_shell_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/presentation/reports_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/drill/reports_purchase_report_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1183`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_isce_home_reports_bundle.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_reports_trade_breakdowns.py`, `flutter_app/test/api_dup_r001_reports_tab_gate_test.dart`, `flutter_app/test/reports_filter_search_empty_test.dart`, `flutter_app/test/reports_filter_simple_chips_empty_test.dart`, `flutter_app/test/reports_item_detail_empty_test.dart`, `flutter_app/test/reports_item_report_load_error_test.dart`, `flutter_app/test/reports_items_tab_empty_test.dart`, `flutter_app/test/reports_overview_aside_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AF. Analytics

- Feature scope / gap: Summaries exist; saved report views/item drilldowns/BI breadth absent.
- Reference implementation: `backend/app/routers/reports_trade.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); ReportSavedView (`report_saved_views`; `backend/app/models/report_saved_view.py:15`)
- Frontend screen/component: `flutter_app/lib/features/reports/shell/reports_shell_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/presentation/reports_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/drill/reports_purchase_report_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1183`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_home_analytics_panel.py`, `flutter_app/test/item_analytics_load_error_test.dart`, `flutter_app/test/item_analytics_redirect_unresolved_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AG. Period Comparison

- Feature scope / gap: Two periods exist; date bounds/role protections require tests.
- Reference implementation: `backend/app/routers/reports_trade.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); ReportSavedView (`report_saved_views`; `backend/app/models/report_saved_view.py:15`)
- Frontend screen/component: `flutter_app/lib/features/reports/shell/reports_shell_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/presentation/reports_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/drill/reports_purchase_report_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1183`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `flutter_app/test/home_period_filter_chips_ia_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AH. Inventory Valuation

- Feature scope / gap: CurrentStock * 10.0m is fabricated financial value, unrelated to purchase/catalog cost.
- Reference implementation: `backend/app/routers/stock/stock_list.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /list`; `GET /shell-bundle`; `GET /delivery-indicator-counts`; `GET /list/compact`; `GET /search`; `GET /alerts/summary`; `GET /warehouse/alerts-summary`; `GET /low-stock/summary`; `GET /low-stock/operations`.
- Database entities/tables: CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`)
- Frontend screen/component: `flutter_app/lib/features/stock/presentation/stock_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1268`); `flutter_app/lib/features/reports/drill/reports_item_report_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1165`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_stock_inventory.py`, `backend/tests/test_stock_inventory_summary.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AI. Spend Analytics

- Feature scope / gap: Draft purchases counted as real spend; must use confirmed purchase data.
- Reference implementation: `backend/app/routers/reports_trade.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`.
- Database entities/tables: TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`); ReportSavedView (`report_saved_views`; `backend/app/models/report_saved_view.py:15`)
- Frontend screen/component: `flutter_app/lib/features/reports/shell/reports_shell_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/presentation/reports_page.dart` (source exists; route/constructor wiring UNKNOWN); `flutter_app/lib/features/reports/drill/reports_purchase_report_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1183`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_home_analytics_panel.py`, `flutter_app/test/item_analytics_load_error_test.dart`, `flutter_app/test/item_analytics_redirect_unresolved_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AJ. Supplier Analytics

- Feature scope / gap: Supplier summary exists; ledgers/item details/discount/commission semantics missing.
- Reference implementation: `backend/app/routers/contacts.py`, `backend/app/routers/reports_trade.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/suppliers`; `POST /v1/businesses/{business_id}/suppliers`; `PATCH /v1/businesses/{business_id}/suppliers/{supplier_id}`; `DELETE /v1/businesses/{business_id}/suppliers/{supplier_id}`; `GET /v1/businesses/{business_id}/brokers`; `POST /v1/businesses/{business_id}/brokers`; `PATCH /v1/businesses/{business_id}/brokers/{broker_id}`; `DELETE /v1/businesses/{business_id}/brokers/{broker_id}`; `GET /v1/businesses/{business_id}/brokers/{broker_id}`; `GET /v1/businesses/{business_id}/brokers/{broker_id}/linked-suppliers`; `GET /v1/businesses/{business_id}/suppliers/{supplier_id}`; `GET /v1/businesses/{business_id}/suppliers/{supplier_id}/metrics`; `GET /v1/businesses/{business_id}/brokers/{broker_id}/metrics`; `GET /v1/businesses/{business_id}/contacts/search`; `GET /v1/businesses/{business_id}/contacts/category-items`; `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`.
- Database entities/tables: Supplier (`suppliers`; `backend/app/models/contacts.py:38`); TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); TradePurchaseLine (`trade_purchase_lines`; `backend/app/models/trade_purchase.py:96`)
- Frontend screen/component: `flutter_app/lib/features/supplier/presentation/supplier_ledger_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:742`); `flutter_app/lib/features/contacts/presentation/supplier_detail_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:729`); `flutter_app/lib/features/reports/presentation/reports_page.dart` (source exists; route/constructor wiring UNKNOWN) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_broker_linked_suppliers.py`, `backend/tests/test_home_analytics_panel.py`, `backend/tests/test_supplier_api.py`, `backend/tests/test_suppliers_list_compact.py`, `flutter_app/test/item_analytics_load_error_test.dart`, `flutter_app/test/item_analytics_redirect_unresolved_test.dart`, `flutter_app/test/item_supplier_intelligence_empty_test.dart`, `flutter_app/test/item_supplier_intelligence_load_error_test.dart`, `flutter_app/test/reports_purchases_supplier_ranking_empty_test.dart`, `flutter_app/test/supplier_detail_load_error_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AK. Export

Current target override: existing ExportsController was extended and Settings now exposes the source-supported downloads. Stock XLSX, monthly PDF, 90-day native JSON, preset ZIP and selected-purchase CSV have VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME evidence. Missing CSV/stock metadata and preserved native layout/schema adaptations keep full AK PARTIAL. The target-missing wording below is historical.

- Feature scope / gap: Reference stock XLSX, monthly PDF, ZIP/JSON downloads implemented; no target export endpoints.
- Reference implementation: `backend/app/routers/exports.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/exports/backup`; `GET /v1/businesses/{business_id}/exports/stock-inventory.xlsx`; `GET /v1/businesses/{business_id}/exports/purchases-month.pdf`; `GET /v1/businesses/{business_id}/exports/backup/export`; `GET /v1/businesses/{business_id}/exports/backup/logs`; `POST /v1/businesses/{business_id}/exports/backup/run`; `POST /v1/businesses/{business_id}/exports/restore/dry-run`; `POST /v1/businesses/{business_id}/exports/restore/commit`.
- Database entities/tables: BackupLog (`backup_logs`; `backend/app/models/owner_ops.py:29`); TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/backup_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:830`); `flutter_app/lib/features/settings/widgets/backup_monthly_banner.dart` (constructor referenced in `flutter_app/lib/features/settings/presentation/settings_page.dart:72`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_exports_backup.py`, `flutter_app/test/item_export_filename_test.dart`, `flutter_app/test/purchase_export_filename_test.dart`, `flutter_app/test/purchase_export_locale_test.dart`, `flutter_app/test/purchase_export_validation_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/ReportService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IReportService.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **MISSING**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AL. Backup / Restore

Current target override: stored source business JSON, safe immutable run history and owner dry-run have VERIFIED_CODE / VERIFIED_TEST / manual VERIFIED_RUNTIME evidence. Scheduler/retention also have VERIFIED_CODE / VERIFIED_TEST; nightly wall-clock runtime is UNKNOWN. Restore commit deliberately remains 501; application backups are distinct from operational database backups. The initial runner-missing wording below is historical.

- Feature scope / gap: Export and dry-run are reference code; restore commit explicitly returns 501 pending production-copy approval. No target backup runner.
- Reference implementation: `backend/app/routers/exports.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/exports/backup`; `GET /v1/businesses/{business_id}/exports/stock-inventory.xlsx`; `GET /v1/businesses/{business_id}/exports/purchases-month.pdf`; `GET /v1/businesses/{business_id}/exports/backup/export`; `GET /v1/businesses/{business_id}/exports/backup/logs`; `POST /v1/businesses/{business_id}/exports/backup/run`; `POST /v1/businesses/{business_id}/exports/restore/dry-run`; `POST /v1/businesses/{business_id}/exports/restore/commit`.
- Database entities/tables: BackupLog (`backup_logs`; `backend/app/models/owner_ops.py:29`); TradePurchase (`trade_purchases`; `backend/app/models/trade_purchase.py:32`); CatalogItem (`catalog_items`; `backend/app/models/catalog.py:47`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/backup_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:830`); `flutter_app/lib/features/settings/widgets/backup_monthly_banner.dart` (constructor referenced in `flutter_app/lib/features/settings/presentation/settings_page.dart:72`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_exports_backup.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/pages/reports/ReportsDashboard.tsx`.
- Target initial status: **BLOCKED**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AM. AI

- Feature scope / gap: Target provider abstraction/failover is verified and preserved; reference llm_failover code remains but intent router is absent.
- Reference implementation: `backend/app/routers/media.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/media/ocr`.
- Database entities/tables: AiUsageLog (`ai_usage_logs`; `backend/app/models/owner_ops.py:103`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`); OcrItemAlias (`ocr_item_aliases`; `backend/app/models/unit_intelligence.py:60`); ItemLearningHistory (`item_learning_history`; `backend/app/models/unit_intelligence.py:100`)
- Frontend screen/component: `flutter_app/lib/features/purchase/presentation/purchase_entry_wizard.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1017`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/AI/AIRoutingService.cs`; frontend `frontend/src/components/AI/PurchaseAssistant.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AN. Purchase Intent Parsing

- Feature scope / gap: Target supports read-only catalog/supplier matching and user review; reference active natural-language intent UI/API not located.
- Reference implementation: `backend/app/routers/media.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/media/ocr`.
- Database entities/tables: AiUsageLog (`ai_usage_logs`; `backend/app/models/owner_ops.py:103`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`); OcrItemAlias (`ocr_item_aliases`; `backend/app/models/unit_intelligence.py:60`); ItemLearningHistory (`item_learning_history`; `backend/app/models/unit_intelligence.py:100`)
- Frontend screen/component: UNKNOWN/BLOCKED: no active natural-language/OCR correction/voice screen confirmed; purchase wizard source is inspected but is not proof that this feature is wired. Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_purchase_commit_vs_quick_purchase.py`, `backend/tests/test_purchase_damage_reports.py`, `backend/tests/test_purchase_status_due_soon.py`, `backend/tests/test_purchase_stock_increment.py`, `backend/tests/test_purchase_stock_reversal.py`, `backend/tests/test_purchase_stock_unit_normalize.py`, `backend/tests/test_realtime_purchase_payload.py`, `backend/tests/test_stock_list_purchased_filter.py`, `backend/tests/test_trade_purchase_delivery_pipeline.py`, `backend/tests/test_trade_purchases.py`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/AI/PurchaseParsingService.cs`; frontend `frontend/src/components/AI/PurchaseAssistant.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AO. OCR / Vision

- Feature scope / gap: Reference media/ocr parses pasted/plain text; image bytes are decoded as UTF-8, no actual Google vision call. TASKS defers OCR.
- Reference implementation: `backend/app/routers/media.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/media/ocr`.
- Database entities/tables: AiUsageLog (`ai_usage_logs`; `backend/app/models/owner_ops.py:103`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`); OcrItemAlias (`ocr_item_aliases`; `backend/app/models/unit_intelligence.py:60`); ItemLearningHistory (`item_learning_history`; `backend/app/models/unit_intelligence.py:100`)
- Frontend screen/component: UNKNOWN/BLOCKED: no active natural-language/OCR correction/voice screen confirmed; purchase wizard source is inspected but is not proof that this feature is wired. Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/AI/AIRoutingService.cs`; frontend `frontend/src/components/AI/PurchaseAssistant.tsx`.
- Target initial status: **BLOCKED**; priority **P3**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AP. OCR Learning / Corrections

- Feature scope / gap: SQL tables ocr_item_aliases/ocr_correction_events exist; active correction service/API/frontend not located. Do not fabricate learning feature.
- Reference implementation: `backend/app/routers/media.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/media/ocr`.
- Database entities/tables: AiUsageLog (`ai_usage_logs`; `backend/app/models/owner_ops.py:103`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`); OcrItemAlias (`ocr_item_aliases`; `backend/app/models/unit_intelligence.py:60`); ItemLearningHistory (`item_learning_history`; `backend/app/models/unit_intelligence.py:100`)
- Frontend screen/component: UNKNOWN/BLOCKED: no active natural-language/OCR correction/voice screen confirmed; purchase wizard source is inspected but is not proof that this feature is wired. Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs`; frontend `frontend/src/components/AI/PurchaseAssistant.tsx`.
- Target initial status: **BLOCKED**; priority **P3**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AQ. WhatsApp

- Feature scope / gap: Graph v19 media+document send/resend and delivery logs exist; webhook/signature/status callback not found. TASKS defers expansion; credentials missing.
- Reference implementation: `backend/app/routers/owner_ops.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/settings/credentials`; `PUT /v1/businesses/{business_id}/settings/credentials/{credential_type}`; `GET /v1/businesses/{business_id}/staff/tasks`; `GET /v1/businesses/{business_id}/staff/{staff_id}/tasks`; `POST /v1/businesses/{business_id}/staff/tasks`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete`; `GET /v1/businesses/{business_id}/staff/performance-summary`; `GET /v1/businesses/{business_id}/owner/dashboard`; `GET /v1/businesses/{business_id}/owner/whatsapp-deliveries`; `POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend`.
- Database entities/tables: WhatsAppDeliveryLog (`whatsapp_delivery_logs`; `backend/app/models/owner_ops.py:128`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/owner_command_center_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:846`); `flutter_app/lib/features/settings/presentation/owner_credentials_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:838`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/pages/purchases/PurchaseDetail.tsx`.
- Target initial status: **BLOCKED**; priority **P3**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AR. Voice

- Feature scope / gap: enable_voice flag exists; no voice processing route/provider/UI located. Documentation/config claim only.
- Reference implementation: `backend/app/routers/media.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/media/ocr`.
- Database entities/tables: AiUsageLog (`ai_usage_logs`; `backend/app/models/owner_ops.py:103`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`); OcrItemAlias (`ocr_item_aliases`; `backend/app/models/unit_intelligence.py:60`); ItemLearningHistory (`item_learning_history`; `backend/app/models/unit_intelligence.py:100`)
- Frontend screen/component: UNKNOWN/BLOCKED: no active natural-language/OCR correction/voice screen confirmed; purchase wizard source is inspected but is not proof that this feature is wired. Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `flutter_app/test/purchase_invoice_totals_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/AI/AIRoutingService.cs`; frontend `frontend/src/components/AI/PurchaseAssistant.tsx`.
- Target initial status: **BLOCKED**; priority **P3**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AS. Settings

Current target override: profile/preferences/encrypted credentials and the operational overview are verified, including actual backup/AI usage metadata, backup/history/dry-run UI and English role help. Full AS remains PARTIAL for absent WhatsApp delivery telemetry, Arabic help and documented source/native breadth differences. Nightly wall-clock runtime is UNKNOWN. The following missing/initial-status wording is historical; see the current checkpoint and implementation status for evidence and limits.

- Feature scope / gap: Reference business profile, encrypted provider credentials, backups/settings screens exist; target menu lacks working settings route.
- Reference implementation: `backend/app/routers/me.py`, `backend/app/routers/owner_ops.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/me/profile`; `PATCH /v1/me/profile`; `POST /v1/me/bootstrap-workspace`; `GET /v1/me/businesses`; `PATCH /v1/me/businesses/{business_id}/branding`; `POST /v1/me/businesses/{business_id}/branding/logo`; `GET /v1/businesses/{business_id}/settings/credentials`; `PUT /v1/businesses/{business_id}/settings/credentials/{credential_type}`; `GET /v1/businesses/{business_id}/staff/tasks`; `GET /v1/businesses/{business_id}/staff/{staff_id}/tasks`; `POST /v1/businesses/{business_id}/staff/tasks`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete`; `GET /v1/businesses/{business_id}/staff/performance-summary`; `GET /v1/businesses/{business_id}/owner/dashboard`; `GET /v1/businesses/{business_id}/owner/whatsapp-deliveries`; `POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend`.
- Database entities/tables: Business (`businesses`; `backend/app/models/business.py:10`); ProviderCredential (`provider_credentials`; `backend/app/models/owner_ops.py:50`); BackupLog (`backup_logs`; `backend/app/models/owner_ops.py:29`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/settings_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:814`); `flutter_app/lib/features/settings/presentation/business_profile_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:822`); `flutter_app/lib/features/settings/presentation/owner_credentials_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:838`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_production_settings.py`, `flutter_app/test/app_settings_action_help_test.dart`, `flutter_app/test/settings_sidebar_discoverability_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/layouts/AppShell.tsx`.
- Target initial status: **MISSING**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AT. User Management

- Feature scope / gap: Tenant memberships scoped, but tenant admin can alter shared global account and assign privileged roles.
- Reference implementation: `backend/app/routers/users.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users/active-sessions`; `POST /v1/businesses/{business_id}/users/bulk`; `GET /v1/businesses/{business_id}/users/{user_id}`; `PATCH /v1/businesses/{business_id}/users/{user_id}`; `DELETE /v1/businesses/{business_id}/users/{user_id}`; `POST /v1/businesses/{business_id}/users/{user_id}/reset-password`; `GET /v1/businesses/{business_id}/users/{user_id}/credentials`; `GET /v1/businesses/{business_id}/users/{user_id}/created-items`; `GET /v1/businesses/{business_id}/users/{user_id}/stock-adjustments`; `GET /v1/businesses/{business_id}/users/{user_id}/purchases`; `GET /v1/businesses/{business_id}/users/{user_id}/ledger`; `GET /v1/businesses/{business_id}/users/{user_id}/permissions`; `PATCH /v1/businesses/{business_id}/users/{user_id}/permissions`; `POST /v1/businesses/{business_id}/activity-log`; `GET /v1/businesses/{business_id}/activity-log`.
- Database entities/tables: User (`users`; `backend/app/models/user.py:12`); Membership (`memberships`; `backend/app/models/membership.py:12`); UserSession (`user_sessions`; `backend/app/models/user_session.py:16`); StaffActivityLog (`staff_activity_log`; `backend/app/models/user_session.py:32`)
- Frontend screen/component: `flutter_app/lib/features/settings/presentation/user_management_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:870`); `flutter_app/lib/features/staff/presentation/staff_shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1329`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_users_management.py`, `flutter_app/test/user_management_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Services/CurrentUserService.cs`, `backend/PurchaseAssistant.Infrastructure/Services/UserService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ICurrentUserService.cs`, `backend/PurchaseAssistant.Application/Interfaces/IUserService.cs`; frontend `frontend/src/pages/users/UsersPage.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AU. Audit / Activity

- Feature scope / gap: Auth audit and stock ledger exist; reference staff activity/lifecycle/damage logs not all mapped.
- Reference implementation: `backend/app/routers/stock_audits.py`, `backend/app/routers/trade_purchases.py`, `backend/app/routers/users.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/businesses/{business_id}/stock-audits`; `GET /v1/businesses/{business_id}/stock-audits/active`; `GET /v1/businesses/{business_id}/stock-audits`; `GET /v1/businesses/{business_id}/stock-audits/kpis`; `GET /v1/businesses/{business_id}/stock-audits/pending-lines`; `GET /v1/businesses/{business_id}/stock-audits/{audit_id}`; `PUT /v1/businesses/{business_id}/stock-audits/{audit_id}`; `POST /v1/businesses/{business_id}/stock-audits/{audit_id}/lines`; `POST /v1/businesses/{business_id}/stock-audits/{audit_id}/complete`; `POST /v1/businesses/{business_id}/stock-audits/{audit_id}/lines/{line_id}/approve`; `DELETE /v1/businesses/{business_id}/stock-audits/{audit_id}`; `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`; `POST /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users/active-sessions`; `POST /v1/businesses/{business_id}/users/bulk`; `GET /v1/businesses/{business_id}/users/{user_id}`; `PATCH /v1/businesses/{business_id}/users/{user_id}`; `DELETE /v1/businesses/{business_id}/users/{user_id}`; `POST /v1/businesses/{business_id}/users/{user_id}/reset-password`; `GET /v1/businesses/{business_id}/users/{user_id}/credentials`; `GET /v1/businesses/{business_id}/users/{user_id}/created-items`; `GET /v1/businesses/{business_id}/users/{user_id}/stock-adjustments`; `GET /v1/businesses/{business_id}/users/{user_id}/purchases`; `GET /v1/businesses/{business_id}/users/{user_id}/ledger`; `GET /v1/businesses/{business_id}/users/{user_id}/permissions`; `PATCH /v1/businesses/{business_id}/users/{user_id}/permissions`; `POST /v1/businesses/{business_id}/activity-log`; `GET /v1/businesses/{business_id}/activity-log`.
- Database entities/tables: StaffActivityLog (`staff_activity_log`; `backend/app/models/user_session.py:32`); AdminAuditLog (`admin_audit_logs`; `backend/app/models/admin_audit_log.py:12`); PurchaseLifecycleEvent (`purchase_lifecycle_events`; `backend/app/models/purchase_lifecycle_event.py:16`); StockMovement (`stock_movements`; `backend/app/models/stock_movement.py:15`)
- Frontend screen/component: `flutter_app/lib/features/staff/presentation/staff_activity_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:953`); `flutter_app/lib/features/catalog/presentation/catalog_item_timeline_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:596`); `flutter_app/lib/features/home/presentation/home_warehouse_activity_page.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1241`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_staff_activity_action_types_stock.py`, `backend/tests/test_stock_audit.py`, `flutter_app/test/api_dup_h004_activity_feed_gate_test.dart`, `flutter_app/test/barcode_scan_history_audit_kpi_error_test.dart`, `flutter_app/test/home_activity_units_test.dart`, `flutter_app/test/home_warehouse_activity_empty_test.dart`, `flutter_app/test/home_warehouse_activity_error_test.dart`, `flutter_app/test/home_warehouse_activity_page_error_test.dart`, `flutter_app/test/staff_activity_empty_test.dart`, `flutter_app/test/staff_home_recent_activity_empty_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs`; frontend `frontend/src/pages/stock/StockActivity.tsx`.
- Target initial status: **PARTIAL**; priority **P1**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AV. Performance

- Feature scope / gap: Reference indexes/ETags/read bundles are code; target category N+1, uncapped pagination/duplicate algorithm require work.
- Reference implementation: `backend/app/routers/catalog.py`, `backend/app/routers/reports_trade.py`, `backend/app/routers/stock/stock_list.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `POST /v1/businesses/{business_id}/reports/sales-comparison`; `GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map`; `GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill`; `GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot`; `GET /v1/businesses/{business_id}/reports/home-overview`; `GET /v1/businesses/{business_id}/reports/trade-summary`; `GET /v1/businesses/{business_id}/reports/trade-daily-profit`; `GET /v1/businesses/{business_id}/reports/trade-items`; `GET /v1/businesses/{business_id}/reports/trade-suppliers`; `GET /v1/businesses/{business_id}/reports/trade-categories`; `GET /v1/businesses/{business_id}/reports/trade-types`; `GET /v1/businesses/{business_id}/reports/period-comparison`; `GET /v1/businesses/{business_id}/reports/movement-summary`; `GET /v1/businesses/{business_id}/reports/activity-feed`; `GET /v1/businesses/{business_id}/reports/item/{catalog_item_id}`; `GET /list`; `GET /shell-bundle`; `GET /delivery-indicator-counts`; `GET /list/compact`; `GET /search`; `GET /alerts/summary`; `GET /warehouse/alerts-summary`; `GET /low-stock/summary`; `GET /low-stock/operations`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Services/CatalogService.cs`, `backend/PurchaseAssistant.Application/Interfaces/ICatalogService.cs`; frontend `frontend/src/lib/queryKeys` (module/path search; exact component absent).
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AW. Caching

- Feature scope / gap: TanStack Query exists; backend Redis/ETag generation absent. No broad polling should be added.
- Reference implementation: `backend/app/routers/catalog.py`, `backend/app/routers/stock/stock_list.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `GET /list`; `GET /shell-bundle`; `GET /delivery-indicator-counts`; `GET /list/compact`; `GET /search`; `GET /alerts/summary`; `GET /warehouse/alerts-summary`; `GET /low-stock/summary`; `GET /low-stock/operations`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/lib/queryKeys` (module/path search; exact component absent).
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AX. Realtime / SignalR-equivalent behavior

Current target override: existing SignalR and RealtimeUpdates have VERIFIED_CODE / VERIFIED_TEST / VERIFIED_RUNTIME evidence for stock delivery to two permitted clients, exact-once receipt in that tested scenario, correct entity, business/permission isolation and reconnect refresh. Other event families and production scale remain UNKNOWN. The no-hub/client wording below describes the initial audit only.

- Feature scope / gap: Reference business SSE queues/recent feed exist; target has no hub registration or SignalR client despite README claims.
- Reference implementation: `backend/app/routers/realtime.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/realtime/events`; `GET /v1/businesses/{business_id}/realtime/recent`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `backend/tests/test_realtime_purchase_payload.py`, `flutter_app/test/realtime_item_ids_test.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/lib/queryKeys` (module/path search; exact component absent).
- Target initial status: **MISSING**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AY. PWA / Offline-related behavior

- Feature scope / gap: Reference manifest/service worker exists; target has no manifest, worker or PWA plugin. Offline mutations not proven safe in either project.
- Reference implementation: `backend/app/routers/health.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /`; `GET /health/live`; `GET /health`; `GET /health/ready`; `GET /health/db-check`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/main.tsx`.
- Target initial status: **MISSING**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### AZ. Deployment

- Feature scope / gap: Target local build exists; production hosting/health readiness/secret and migration operation contract incomplete.
- Reference implementation: `backend/app/routers/health.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /`; `GET /health/live`; `GET /health`; `GET /health/ready`; `GET /health/db-check`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/main.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### BA. Security

- Feature scope / gap: JWT/password hashing present; live membership checks, rate limit, financial redaction, refresh race and global account mutation require fixes.
- Reference implementation: `backend/app/routers/auth.py`, `backend/app/routers/owner_ops.py`, `backend/app/routers/users.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `POST /v1/auth/register`; `POST /v1/auth/forgot-password`; `POST /v1/auth/reset-password`; `POST /v1/auth/login`; `POST /v1/auth/google`; `POST /v1/auth/refresh`; `GET /v1/businesses/{business_id}/settings/credentials`; `PUT /v1/businesses/{business_id}/settings/credentials/{credential_type}`; `GET /v1/businesses/{business_id}/staff/tasks`; `GET /v1/businesses/{business_id}/staff/{staff_id}/tasks`; `POST /v1/businesses/{business_id}/staff/tasks`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept`; `POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete`; `GET /v1/businesses/{business_id}/staff/performance-summary`; `GET /v1/businesses/{business_id}/owner/dashboard`; `GET /v1/businesses/{business_id}/owner/whatsapp-deliveries`; `POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend`; `POST /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users`; `GET /v1/businesses/{business_id}/users/active-sessions`; `POST /v1/businesses/{business_id}/users/bulk`; `GET /v1/businesses/{business_id}/users/{user_id}`; `PATCH /v1/businesses/{business_id}/users/{user_id}`; `DELETE /v1/businesses/{business_id}/users/{user_id}`; `POST /v1/businesses/{business_id}/users/{user_id}/reset-password`; `GET /v1/businesses/{business_id}/users/{user_id}/credentials`; `GET /v1/businesses/{business_id}/users/{user_id}/created-items`; `GET /v1/businesses/{business_id}/users/{user_id}/stock-adjustments`; `GET /v1/businesses/{business_id}/users/{user_id}/purchases`; `GET /v1/businesses/{business_id}/users/{user_id}/ledger`; `GET /v1/businesses/{business_id}/users/{user_id}/permissions`; `PATCH /v1/businesses/{business_id}/users/{user_id}/permissions`; `POST /v1/businesses/{business_id}/activity-log`; `GET /v1/businesses/{business_id}/activity-log`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/auth/AuthProvider.tsx`.
- Target initial status: **BROKEN**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### BB. Database / migrations

- Feature scope / gap: EF migrations/tenant filters exist; relationship tenant integrity and purchase concurrency missing. Reference Alembic/SQL are evidence, not copy templates.
- Reference implementation: `backend/app/routers/catalog.py`, `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Infrastructure/Data/AppDbContext.cs`; frontend `frontend/src/api/catalogApi` (module/path search; exact component absent).
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### BC. Testing

- Feature scope / gap: Baseline target 63 unit + 13 PostgreSQL integration + 29 frontend tests pass; reference test files present but not executed; endpoint/runtime coverage incomplete.
- Reference implementation: `backend/app/routers/health.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /`; `GET /health/live`; `GET /health`; `GET /health/ready`; `GET /health/db-check`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: No directly matching test found; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.IntegrationTests/UnitTest1.cs`, `backend/PurchaseAssistant.IntegrationTests/Dashboard/DashboardIntegrationTests.cs`, `backend/PurchaseAssistant.IntegrationTests/Notifications/NotificationIntegrationTests.cs`, `backend/PurchaseAssistant.IntegrationTests/Purchases/PurchaseIntegrationTests.cs`, `backend/PurchaseAssistant.IntegrationTests/Stock/StockIntegrationTests.cs`; frontend `frontend/src/tests` (module/path search; exact component absent).
- Target initial status: **PARTIAL**; priority **P0**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

### BD. UX / responsive behavior

- Feature scope / gap: React/Tailwind preserved; nine widths and role/error/keyboard states need browser validation. Flutter layout bug fixes are not directly portable.
- Reference implementation: `backend/app/routers/catalog.py`, `backend/app/routers/trade_purchases.py`. Related implementations are in the route contracts/model appendix below.
- API endpoints: `GET /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/category-types-index`; `POST /v1/businesses/{business_id}/item-categories`; `GET /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types`; `PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id}`; `GET /v1/businesses/{business_id}/catalog/duplicate-clusters`; `POST /v1/businesses/{business_id}/catalog/items/bulk-archive`; `PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder`; `GET /v1/businesses/{business_id}/catalog/fuzzy-check`; `GET /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/from-scan`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode`; `POST /v1/businesses/{business_id}/catalog-items`; `POST /v1/businesses/{business_id}/catalog-items/batch`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines`; `GET /v1/businesses/{business_id}/item-categories/{category_id}/insights`; `PATCH /v1/businesses/{business_id}/catalog-items/{item_id}`; `DELETE /v1/businesses/{business_id}/catalog-items/{item_id}`; `GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants`; `PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id}`; `GET /v1/businesses/{business_id}/trade-purchases/draft`; `PUT /v1/businesses/{business_id}/trade-purchases/draft`; `DELETE /v1/businesses/{business_id}/trade-purchases/draft`; `POST /v1/businesses/{business_id}/trade-purchases/preview-lines`; `POST /v1/businesses/{business_id}/trade-purchases/validate`; `POST /v1/businesses/{business_id}/trade-purchases/check-duplicate`; `GET /v1/businesses/{business_id}/trade-purchases/next-human-id`; `GET /v1/businesses/{business_id}/trade-purchases`; `GET /v1/businesses/{business_id}/trade-purchases/last-defaults`; `POST /v1/businesses/{business_id}/trade-purchases`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment`; `GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline`; `PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel`; `PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports`; `GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events`; `POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle`.
- Database entities/tables: Cross-cutting; per-router/model catalogue below identifies affected data. No additional standalone feature table inferred.
- Frontend screen/component: `flutter_app/lib/features/shell/shell_screen.dart` (constructor referenced in `flutter_app/lib/core/router/app_router.dart:1220`) Reference browser/device runtime not executed.
- Roles / authorization: exact `Depends(require_role/require_permission/require_membership)` signatures are recorded per endpoint below; source permission templates in `backend/app/services/permissions.py`. AGENTS owner-only financial rule is stricter than reference staff-only redaction; record conflict, enforce user-requested owner financial protection in target.
- Business/validation/error rules: handler calls and HTTP errors are recorded below. Tenant path membership is required for business APIs. Missing runtime checks are UNKNOWN, not inferred passes. Financial, stock and AI boundaries are detailed in REFERENCE_ENGINEERING_RULES.md.
- Concurrency: reference stock version/409 + movement idempotency are implemented in stock services/routers; purchase paths vary (inspect individual service); no blanket concurrency guarantee. Target gaps listed explicitly.
- Security requirements: server-side permission + business isolation; owner financial restriction; no secrets or AI authority. Feature-specific upload/webhook/provider behavior is unverified unless located.
- Tests located: `flutter_app/test/responsive_layout_smoke_test.dart`, `flutter_app/test/responsive_test_utils.dart`; NOT EXECUTED for reference.
- Target implementation: `backend/PurchaseAssistant.Web/Program.cs`; frontend `frontend/src/layouts/AppShell.tsx`.
- Target initial status: **PARTIAL**; priority **P2**. Reference evidence is VERIFIED_CODE for located routes/helpers, DOCUMENTATION_CLAIM for docs, UNKNOWN/BLOCKED for unwired/deferred behavior.

## Route contracts: all 229 definitions

Paths below are router-local compositions. Stock child routers receive `/v1/businesses/{business_id}/stock` from `stock/__init__.py`; verify parent registration before calling. Routers with no prefix are not assumed root APIs. Signature records request types, query bounds and dependencies; response records declared schema or explicitly untyped. Source files contain field-level schema details. Target equivalents use existing controllers/services; no compatibility route is requested.

### POST /v1/auth/register — register

- VERIFIED_CODE: `backend/app/routers/auth.py:75`.
- Request / authorization / tenant contract: `db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)], body: RegisterRequest`.
- Response: `TokenPair`.
- Explicit error branches: status.HTTP_403_FORBIDDEN, status.HTTP_409_CONFLICT, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Business`, `Depends`, `HTTPException`, `Membership`, `TokenPair`, `User`, `create_access_token`, `create_refresh_token`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `ex.first`, `getattr`, `hash_password`, `or_`, `router.post`, `select`, `select(User.id).where`, `settings.superadmin_bootstrap_email.strip`, `settings.superadmin_bootstrap_email.strip().lower`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/auth/forgot-password — forgot_password

- VERIFIED_CODE: `backend/app/routers/auth.py:141`.
- Request / authorization / tenant contract: `body: ForgotPasswordRequest, request: Request, db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(body.email or '').strip`, `(body.email or '').strip().lower`, `(settings.app_env or '').lower`, `Depends`, `PasswordResetToken`, `_client_ip`, `_enforce_limiter`, `datetime.now`, `db.add`, `db.commit`, `db.execute`, `delete`, `delete(PasswordResetToken).where`, `hash_reset_token`, `logger.info`, `new_reset_token_raw`, `r.scalar_one_or_none`, `router.post`, `select`, `select(User).where`, `timedelta`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/auth/reset-password — reset_password_with_token

- VERIFIED_CODE: `backend/app/routers/auth.py:196`.
- Request / authorization / tenant contract: `body: ResetPasswordRequest, db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Depends`, `HTTPException`, `PasswordResetToken.used_at.is_`, `body.token.strip`, `datetime.now`, `db.commit`, `db.execute`, `hash_password`, `hash_reset_token`, `q.scalar_one_or_none`, `router.post`, `select`, `select(PasswordResetToken).where`, `select(User).where`, `ur.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/auth/login — login

- VERIFIED_CODE: `backend/app/routers/auth.py:235`.
- Request / authorization / tenant contract: `request: Request, db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)], body: LoginRequest`.
- Response: `TokenPair`.
- Explicit error branches: status.HTTP_401_UNAUTHORIZED, status.HTTP_403_FORBIDDEN, status.HTTP_503_SERVICE_UNAVAILABLE.
- Implementation calls: `(body.device_token or '').strip`, `Depends`, `HTTPException`, `TokenPair`, `UserSession`, `_client_ip`, `_enforce_limiter`, `create_access_token`, `create_refresh_token`, `datetime.now`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `getattr`, `log_staff_login_if_applicable`, `logger.exception`, `mem_q.scalar_one_or_none`, `now.isoformat`, `resolve_user_by_email`, `router.post`, `select`, `select(Membership).where`, `select(Membership).where(Membership.user_id == user.id).limit`, `verify_password`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/auth/google — auth_google

- VERIFIED_CODE: `backend/app/routers/auth.py:327`.
- Request / authorization / tenant contract: `db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)], body: GoogleAuthRequest`.
- Response: `TokenPair`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_401_UNAUTHORIZED, status.HTTP_403_FORBIDDEN, status.HTTP_409_CONFLICT, status.HTTP_503_SERVICE_UNAVAILABLE.
- Implementation calls: `(claims.get('email') or '').strip`, `(claims.get('email') or '').strip().lower`, `Business`, `Depends`, `HTTPException`, `Membership`, `TokenPair`, `User`, `_allocate_username`, `claims.get`, `create_access_token`, `create_refresh_token`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `getattr`, `gn.strip`, `gn2.strip`, `r_email.scalar_one_or_none`, `r_sub.scalar_one_or_none`, `raw_name.strip`, `router.post`, `select`, `select(User).where`, `settings.google_oauth_client_id_list`, `settings.superadmin_bootstrap_email.strip`, `settings.superadmin_bootstrap_email.strip().lower`, `user.name.strip`, `verify_google_id_token_async`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/auth/refresh — refresh_token

- VERIFIED_CODE: `backend/app/routers/auth.py:433`.
- Request / authorization / tenant contract: `db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)], body: RefreshRequest`.
- Response: `TokenPair`.
- Explicit error branches: status.HTTP_401_UNAUTHORIZED, status.HTTP_403_FORBIDDEN.
- Implementation calls: `Depends`, `HTTPException`, `TokenPair`, `create_access_token`, `create_refresh_token`, `db.execute`, `decode_refresh_token`, `getattr`, `result.scalar_one_or_none`, `router.post`, `select`, `select(User).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/item-categories — list_item_categories

- VERIFIED_CODE: `backend/app/routers/catalog.py:1125`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[ItemCategoryOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `ItemCategoryOut`, `db.execute`, `func.lower`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(ItemCategory).where`, `select(ItemCategory).where(ItemCategory.business_id == business_id).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/category-types-index — list_category_types_index

- VERIFIED_CODE: `backend/app/routers/catalog.py:1141`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[CategoryTypeIndexOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CategoryTypeIndexOut`, `Depends`, `db.execute`, `func.lower`, `r.all`, `router.get`, `select`, `select(CategoryType.id, CategoryType.category_id, ItemCategory.name, CategoryType.name).join`, `select(CategoryType.id, CategoryType.category_id, ItemCategory.name, CategoryType.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(CategoryType.id, CategoryType.category_id, ItemCategory.name, CategoryType.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(ItemCategory.business_id == business_id).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/item-categories — create_item_category

- VERIFIED_CODE: `backend/app/routers/catalog.py:1171`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: ItemCategoryCreate`.
- Response: `ItemCategoryOut`.
- Explicit error branches: status.HTTP_409_CONFLICT.
- Implementation calls: `CategoryType`, `Depends`, `HTTPException`, `ItemCategory`, `ItemCategoryOut`, `_category_dup`, `body.name.strip`, `db.add`, `db.commit`, `db.flush`, `db.refresh`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/item-categories/{category_id} — get_item_category

- VERIFIED_CODE: `backend/app/routers/catalog.py:1193`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `ItemCategoryOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `ItemCategoryOut`, `db.execute`, `r.scalar_one_or_none`, `router.get`, `select`, `select(ItemCategory).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/item-categories/{category_id}/trade-summary — category_trade_summary

- VERIFIED_CODE: `backend/app/routers/catalog.py:1216`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `CategoryTradeSummaryOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CategoryTradeItemRow`, `CategoryTradeSummaryOut`, `Depends`, `HTTPException`, `TradePurchase.id.isnot`, `_supplier_broker_name_maps_by_ids`, `and_`, `case`, `cr.scalar_one_or_none`, `db.execute`, `execute_with_retry`, `func.coalesce`, `func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label`, `func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label`, `func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label`, `func.lower`, `func.max`, `func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label`, `func.sum`, `items_out.append`, `lbn_map.get`, `lsn_map.get`, `r.all`, `router.get`, `select`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label('period_line_total'), func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label('period_qty_bags'), func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label('period_weight_kg'), CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label('last_trade_human_id')).select_from`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label('period_line_total'), func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label('period_qty_bags'), func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label('period_weight_kg'), CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label('last_trade_human_id')).select_from(CatalogItem).outerjoin`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label('period_line_total'), func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label('period_qty_bags'), func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label('period_weight_kg'), CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label('last_trade_human_id')).select_from(CatalogItem).outerjoin(TradePurchaseLine, TradePurchaseLine.catalog_item_id == CatalogItem.id).outerjoin`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label('period_line_total'), func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label('period_qty_bags'), func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label('period_weight_kg'), CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label('last_trade_human_id')).select_from(CatalogItem).outerjoin(TradePurchaseLine, TradePurchaseLine.catalog_item_id == CatalogItem.id).outerjoin(TradePurchase, and_(TradePurchase.id == TradePurchaseLine.trade_purchase_id, TradePurchase.business_id == business_id)).where`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label('period_line_total'), func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label('period_qty_bags'), func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label('period_weight_kg'), CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label('last_trade_human_id')).select_from(CatalogItem).outerjoin(TradePurchaseLine, TradePurchaseLine.catalog_item_id == CatalogItem.id).outerjoin(TradePurchase, and_(TradePurchase.id == TradePurchaseLine.trade_purchase_id, TradePurchase.business_id == business_id)).where(CatalogItem.business_id == business_id, CatalogItem.category_id == category_id).group_by`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(case((in_reports, line_amt), else_=0)), 0).label('period_line_total'), func.coalesce(func.sum(case((in_reports, qty_bags), else_=0)), 0).label('period_qty_bags'), func.coalesce(func.sum(case((in_reports, weight_kg), else_=0)), 0).label('period_weight_kg'), CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, func.max(case((TradePurchase.id == CatalogItem.last_trade_purchase_id, TradePurchase.human_id), else_=None)).label('last_trade_human_id')).select_from(CatalogItem).outerjoin(TradePurchaseLine, TradePurchaseLine.catalog_item_id == CatalogItem.id).outerjoin(TradePurchase, and_(TradePurchase.id == TradePurchaseLine.trade_purchase_id, TradePurchase.business_id == business_id)).where(CatalogItem.business_id == business_id, CatalogItem.category_id == category_id).group_by(CatalogItem.id, CatalogItem.name, CatalogItem.last_purchase_price, CatalogItem.last_selling_rate, CatalogItem.last_supplier_id, CatalogItem.last_broker_id, CatalogItem.last_trade_purchase_id).order_by`, `select(ItemCategory.id).where`, `tq.trade_line_amount_expr`, `tq.trade_line_qty_bags_expr`, `tq.trade_line_weight_expr`, `tq.trade_purchase_status_in_reports`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/item-categories/{category_id} — update_item_category

- VERIFIED_CODE: `backend/app/routers/catalog.py:1341`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: ItemCategoryUpdate`.
- Response: `ItemCategoryOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `Depends`, `HTTPException`, `ItemCategoryOut`, `_category_dup`, `body.model_dump`, `data['name'].strip`, `db.commit`, `db.execute`, `db.refresh`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(ItemCategory).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/item-categories/{category_id} — delete_item_category

- VERIFIED_CODE: `backend/app/routers/catalog.py:1372`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `db.commit`, `db.delete`, `db.execute`, `func.count`, `ic.scalar`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(ItemCategory).where`, `select(func.count(CatalogItem.id)).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/item-categories/{category_id}/category-types — list_category_types

- VERIFIED_CODE: `backend/app/routers/catalog.py:1407`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[CategoryTypeOut]`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CategoryTypeOut`, `Depends`, `HTTPException`, `cr.first`, `db.execute`, `func.lower`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(CategoryType).where`, `select(CategoryType).where(CategoryType.category_id == category_id).order_by`, `select(ItemCategory.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/item-categories/{category_id}/category-types — create_category_type

- VERIFIED_CODE: `backend/app/routers/catalog.py:1436`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CategoryTypeCreate`.
- Response: `CategoryTypeOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `CategoryType`, `CategoryTypeOut`, `Depends`, `HTTPException`, `_type_name_dup`, `body.name.strip`, `cr.first`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `router.post`, `select`, `select(ItemCategory.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id} — update_category_type

- VERIFIED_CODE: `backend/app/routers/catalog.py:1468`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, type_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CategoryTypeUpdate`.
- Response: `CategoryTypeOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `CategoryTypeOut`, `Depends`, `HTTPException`, `_type_name_dup`, `body.model_dump`, `data['name'].strip`, `db.commit`, `db.execute`, `db.refresh`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(CategoryType).join`, `select(CategoryType).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/item-categories/{category_id}/category-types/{type_id} — delete_category_type

- VERIFIED_CODE: `backend/app/routers/catalog.py:1506`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, type_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `catalog_items_has_type_id_column`, `db.commit`, `db.delete`, `db.execute`, `func.count`, `ic.scalar`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(CategoryType).join`, `select(CategoryType).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(func.count(CatalogItem.id)).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog/duplicate-clusters — catalog_duplicate_clusters

- VERIFIED_CODE: `backend/app/routers/catalog.py:1540`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_owner_membership)], min_score: float=Query(0.85, ge=0.5, le=1.0)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `DuplicatePairOut`, `Query`, `db.execute`, `fuzz.token_sort_ratio`, `name_a.lower`, `name_b.lower`, `pairs.append`, `pairs.sort`, `r.all`, `range`, `round`, `router.get`, `select`, `select(CatalogItem.id, CatalogItem.name).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/catalog/items/bulk-archive — bulk_archive_catalog_items

- VERIFIED_CODE: `backend/app/routers/catalog.py:1578`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: BulkItemIdsIn, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_owner_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `CatalogItem.id.in_`, `Depends`, `_invalidate_catalog_read_caches`, `datetime.now`, `db.commit`, `db.execute`, `r.scalars`, `r.scalars().all`, `router.post`, `select`, `select(CatalogItem).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/catalog/items/bulk-reorder — bulk_reorder_catalog_items

- VERIFIED_CODE: `backend/app/routers/catalog.py:1601`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: BulkReorderIn, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_owner_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `CatalogItem.id.in_`, `D`, `Depends`, `_invalidate_catalog_read_caches`, `db.commit`, `db.execute`, `r.scalars`, `r.scalars().all`, `router.patch`, `select`, `select(CatalogItem).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog/fuzzy-check — catalog_fuzzy_check

- VERIFIED_CODE: `backend/app/routers/catalog.py:1626`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], name: Annotated[str, Query(min_length=1, max_length=512)], supplier_id: Annotated[uuid.UUID | None, Query()]=None, category_id: Annotated[uuid.UUID | None, Query()]=None, type_id: Annotated[uuid.UUID | None, Query()]=None`.
- Response: `CatalogFuzzyCheckOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogFuzzyCheckHitOut`, `CatalogFuzzyCheckOut`, `CatalogItem.deleted_at.is_`, `Depends`, `Query`, `db.execute`, `exists`, `hits.append`, `id_to_name.get`, `name.strip`, `pairs.append`, `r.all`, `rank_ids_by_token_sort`, `round`, `router.get`, `select`, `select(1).where`, `select(CatalogItem.id, CatalogItem.name).where`, `stmt.where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items — list_catalog_items

- VERIFIED_CODE: `backend/app/routers/catalog.py:1671`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], category_id: uuid.UUID | None=Query(None, description='Filter by category'), type_id: uuid.UUID | None=Query(None, description='Filter by category type'), page: int=Query(1, ge=1), per_page: int=Query(200, ge=1, le=500)`.
- Response: `list[CatalogItemOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `CatalogItemOut`, `Depends`, `Query`, `_catalog_item_out`, `_dedupe_preserve_order`, `_default_supplier_broker_ids_for_items`, `_is_delivered_for_trade_purchase_ids`, `_last_party_names_for_catalog_items`, `_max_purchase_dates_for_catalog_items_bulk`, `_maybe_redact_catalog_out`, `br_m.get`, `catalog_items_has_type_id_column`, `catalog_items_list_cache_key`, `catalog_items_ttl_s`, `date_map.get`, `db.execute`, `del_map.get`, `execute_with_retry`, `func.lower`, `get_cached`, `lbn.get`, `logger.exception`, `logger.info`, `lsn.get`, `q.order_by`, `q.order_by(func.lower(CatalogItem.name)).offset`, `q.order_by(func.lower(CatalogItem.name)).offset(offset).limit`, `q.where`, `r.all`, `router.get`, `select`, `select(CatalogItem, CategoryType.name, ItemCategory.name).join`, `select(CatalogItem, CategoryType.name, ItemCategory.name).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).outerjoin`, `select(CatalogItem, CategoryType.name, ItemCategory.name).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).outerjoin(CategoryType, CategoryType.id == CatalogItem.type_id).where`, `select(CatalogItem, ItemCategory.name).join`, `select(CatalogItem, ItemCategory.name).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `set_cached`, `should_redact_financials`, `sup_m.get`, `time.perf_counter`, `trade_read_cache_generation`, `x.model_dump`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/catalog-items/from-scan — create_catalog_item_from_scan

- VERIFIED_CODE: `backend/app/routers/catalog.py:1947`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CatalogItemFromScanIn`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_409_CONFLICT.
- Implementation calls: `CatalogItem`, `Depends`, `HTTPException`, `_assert_unique_barcode`, `_assert_unique_item_code`, `_catalog_item_out`, `_item_dup`, `body.name.strip`, `catalog_items_has_type_id_column`, `crn.scalar_one_or_none`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `merge_unit_resolution_into_catalog_row`, `resolve_for_catalog_item`, `router.post`, `select`, `select(CategoryType.id, CategoryType.category_id).join`, `select(CategoryType.id, CategoryType.category_id).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(CategoryType.name).where`, `select(ItemCategory.name).where`, `tr.first`, `trn.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/item-code — patch_catalog_item_code

- VERIFIED_CODE: `backend/app/routers/catalog.py:2014`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: ItemCodePatchIn`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `_assert_unique_item_code`, `_catalog_item_out`, `db.commit`, `db.execute`, `db.refresh`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(CatalogItem).where`, `select(CategoryType.name).where`, `tr.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/catalog-items/{item_id}/barcode — patch_catalog_item_barcode

- VERIFIED_CODE: `backend/app/routers/catalog.py:2044`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('stock_edit'))], db: Annotated[AsyncSession, Depends(get_db)], body: BarcodePatchIn`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `_assert_unique_barcode`, `_catalog_item_out`, `db.commit`, `db.execute`, `db.refresh`, `r.scalar_one_or_none`, `require_permission`, `router.patch`, `select`, `select(CatalogItem).where`, `select(CategoryType.name).where`, `tr.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/catalog-items — create_catalog_item

- VERIFIED_CODE: `backend/app/routers/catalog.py:2074`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CatalogItemCreate`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_409_CONFLICT.
- Implementation calls: `(body.hsn_code or '').strip`, `(body.item_code or '').strip`, `(i.name or '').split`, `CatalogItem`, `Depends`, `HTTPException`, `_apply_canonical_unit_profile`, `_assert_broker_ids_in_business`, `_assert_supplier_ids_in_business`, `_catalog_item_out`, `_coerce_box_items_per_box`, `_dedupe_preserve_order`, `_get_or_create_general_type_id`, `_invalidate_catalog_read_caches`, `_item_dup`, `_last_party_names_for_catalog_items`, `_last_purchase_delivered_for_snapshot`, `_max_purchase_date_for_catalog_item`, `_next_item_code`, `_norm_name`, `_normalize_barcode`, `_normalize_package_type`, `_replace_default_broker_rows`, `_replace_default_supplier_rows`, `_seed_supplier_item_defaults`, `_verify_type_in_category`, `body.name.strip`, `catalog_items_has_type_id_column`, `crn.scalar_one_or_none`, `crn0.scalar_one_or_none`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `dup_q.where`, `er.scalar_one`, `func.lower`, `lbn.get`, `lsn.get`, `merge_unit_resolution_into_catalog_row`, `parts0[0].isalpha`, `rc.first`, `resolve_for_catalog_item`, `router.post`, `select`, `select(CatalogItem.id).where`, `select(CategoryType.name).where`, `select(ItemCategory.id).where`, `select(ItemCategory.name).where`, `tr.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/catalog-items/batch — batch_create_catalog_items

- VERIFIED_CODE: `backend/app/routers/catalog.py:2208`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CatalogBatchCreate`.
- Response: `CatalogBatchOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(i.name or '').split`, `CatalogBatchOut`, `CatalogItem`, `Depends`, `_assert_broker_ids_in_business`, `_assert_supplier_ids_in_business`, `_catalog_item_out`, `_coerce_box_items_per_box`, `_dedupe_preserve_order`, `_default_supplier_broker_ids_for_items`, `_invalidate_catalog_read_caches`, `_item_dup`, `_last_party_names_for_catalog_items`, `_last_purchase_delivered_for_snapshot`, `_max_purchase_date_for_catalog_item`, `_normalize_package_type`, `_replace_default_broker_rows`, `_replace_default_supplier_rows`, `_seed_supplier_item_defaults`, `_verify_type_in_category`, `br_m.get`, `catalog_items_has_type_id_column`, `created_rows.append`, `crn.scalar_one_or_none`, `crn0.scalar_one_or_none`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `lbn.get`, `line.name.strip`, `lsn.get`, `merge_unit_resolution_into_catalog_row`, `outs.append`, `parts0[0].isalpha`, `resolve_for_catalog_item`, `router.post`, `select`, `select(CategoryType.id, CategoryType.category_id).join`, `select(CategoryType.id, CategoryType.category_id).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(CategoryType.name).where`, `select(ItemCategory.name).where`, `sup_m.get`, `tr.first`, `tr.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items/{item_id} — get_catalog_item

- VERIFIED_CODE: `backend/app/routers/catalog.py:2336`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_catalog_item_out`, `_default_supplier_broker_ids_for_items`, `_last_party_names_for_catalog_items`, `_last_purchase_delivered_for_snapshot`, `_max_purchase_date_for_catalog_item`, `_maybe_redact_catalog_out`, `br_m.get`, `catalog_items_has_type_id_column`, `crn.scalar_one_or_none`, `db.execute`, `lbn.get`, `load_only`, `logger.exception`, `lsn.get`, `r.one_or_none`, `r.scalar_one_or_none`, `router.get`, `select`, `select(CatalogItem).options`, `select(CatalogItem).options(load_only(*_CATALOG_ITEM_CORE)).where`, `select(CatalogItem, CategoryType.name, ItemCategory.name).outerjoin`, `select(CatalogItem, CategoryType.name, ItemCategory.name).outerjoin(CategoryType, CategoryType.id == CatalogItem.type_id).join`, `select(CatalogItem, CategoryType.name, ItemCategory.name).outerjoin(CategoryType, CategoryType.id == CatalogItem.type_id).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `select(ItemCategory.name).where`, `sup_m.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/catalog-items/{item_id}/generate-code — generate_catalog_item_code

- VERIFIED_CODE: `backend/app/routers/catalog.py:2420`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `(i.item_code or '').strip`, `Depends`, `HTTPException`, `_catalog_item_out`, `_default_supplier_broker_ids_for_items`, `_last_party_names_for_catalog_items`, `_last_purchase_delivered_for_snapshot`, `_max_purchase_date_for_catalog_item`, `_maybe_redact_catalog_out`, `_next_item_code`, `br_m.get`, `crn.scalar_one_or_none`, `db.commit`, `db.execute`, `db.refresh`, `lbn.get`, `lsn.get`, `r.scalar_one_or_none`, `router.post`, `select`, `select(CatalogItem).where`, `select(CategoryType.name).where`, `select(ItemCategory.name).where`, `sup_m.get`, `tr.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items/{item_id}/supplier-purchase-defaults — supplier_purchase_defaults

- VERIFIED_CODE: `backend/app/routers/catalog.py:2477`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], supplier_id: uuid.UUID=Query(...)`.
- Response: `SupplierPurchaseDefaultsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `Query`, `SupplierPurchaseDefaultsOut`, `db.execute`, `dr.scalar_one_or_none`, `ir.scalar_one_or_none`, `router.get`, `select`, `select(CatalogItem).where`, `select(Supplier.id).where`, `select(SupplierItemDefault).where`, `sr.first`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items/{item_id}/trade-supplier-prices — catalog_item_trade_supplier_prices

- VERIFIED_CODE: `backend/app/routers/catalog.py:2526`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `CatalogItemTradeSupplierPricesOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItemTradeSupplierPricesOut`, `Depends`, `HTTPException`, `Supplier.name.label`, `TradePurchase.id.label`, `TradePurchase.supplier_id.isnot`, `TradeSupplierPriceRow`, `db.execute`, `deals.get`, `deals[sid].add`, `desc`, `ir.first`, `landing_for_avg.append`, `last_five_prices.append`, `line_amt.label`, `lr.mappings`, `lr.mappings().all`, `min`, `router.get`, `seen_suppliers.add`, `select`, `select(CatalogItem.id).where`, `select(TradePurchase.id.label('tp_id'), TradePurchase.supplier_id, Supplier.name.label('supplier_name'), TradePurchaseLine.landing_cost, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchase.purchase_date, TradePurchaseLine.id, line_amt.label('line_amt')).select_from`, `select(TradePurchase.id.label('tp_id'), TradePurchase.supplier_id, Supplier.name.label('supplier_name'), TradePurchaseLine.landing_cost, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchase.purchase_date, TradePurchaseLine.id, line_amt.label('line_amt')).select_from(TradePurchaseLine).join`, `select(TradePurchase.id.label('tp_id'), TradePurchase.supplier_id, Supplier.name.label('supplier_name'), TradePurchaseLine.landing_cost, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchase.purchase_date, TradePurchaseLine.id, line_amt.label('line_amt')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).join`, `select(TradePurchase.id.label('tp_id'), TradePurchase.supplier_id, Supplier.name.label('supplier_name'), TradePurchaseLine.landing_cost, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchase.purchase_date, TradePurchaseLine.id, line_amt.label('line_amt')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).join(Supplier, Supplier.id == TradePurchase.supplier_id).where`, `select(TradePurchase.id.label('tp_id'), TradePurchase.supplier_id, Supplier.name.label('supplier_name'), TradePurchaseLine.landing_cost, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchase.purchase_date, TradePurchaseLine.id, line_amt.label('line_amt')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).join(Supplier, Supplier.id == TradePurchase.supplier_id).where(TradePurchase.business_id == business_id, TradePurchaseLine.catalog_item_id == item_id, TradePurchase.supplier_id.isnot(None), tq.trade_purchase_status_in_reports()).order_by`, `set`, `sorted`, `sum`, `sum_amt.get`, `sum_qty.get`, `supplier_latest.append`, `suppliers_out.append`, `suppliers_out.sort`, `tq.trade_line_amount_expr`, `tq.trade_purchase_status_in_reports`, `vwap.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items/{item_id}/insights — catalog_item_insights

- VERIFIED_CODE: `backend/app/routers/catalog.py:2657`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], from_date: date=Query(..., alias='from'), to_date: date=Query(..., alias='to')`.
- Response: `CatalogItemInsightsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItemInsightsOut`, `Depends`, `HTTPException`, `Query`, `_trade_purchase_date_filter`, `db.execute`, `func.avg`, `func.coalesce`, `func.count`, `func.distinct`, `func.max`, `func.sum`, `ir.first`, `r.one`, `rev_r.scalar`, `router.get`, `select`, `select(CatalogItem.id).where`, `select(func.coalesce(func.sum(TradePurchaseLine.qty * sell_e), 0)).select_from`, `select(func.coalesce(func.sum(TradePurchaseLine.qty * sell_e), 0)).select_from(TradePurchaseLine).join`, `select(func.coalesce(func.sum(TradePurchaseLine.qty * sell_e), 0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(func.count(TradePurchaseLine.id), func.count(func.distinct(TradePurchaseLine.trade_purchase_id)), func.coalesce(func.sum(profit_e), 0), func.avg(TradePurchaseLine.landing_cost), func.avg(sell_e), func.max(TradePurchase.purchase_date)).select_from`, `select(func.count(TradePurchaseLine.id), func.count(func.distinct(TradePurchaseLine.trade_purchase_id)), func.coalesce(func.sum(profit_e), 0), func.avg(TradePurchaseLine.landing_cost), func.avg(sell_e), func.max(TradePurchase.purchase_date)).select_from(TradePurchaseLine).join`, `select(func.count(TradePurchaseLine.id), func.count(func.distinct(TradePurchaseLine.trade_purchase_id)), func.coalesce(func.sum(profit_e), 0), func.avg(TradePurchaseLine.landing_cost), func.avg(sell_e), func.max(TradePurchase.purchase_date)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `sell_e.isnot`, `tq.trade_line_profit_expr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items/{item_id}/lines — catalog_item_lines

- VERIFIED_CODE: `backend/app/routers/catalog.py:2732`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], from_date: date=Query(..., alias='from'), to_date: date=Query(..., alias='to'), limit: int=Query(20, ge=1, le=50), offset: int=Query(0, ge=0)`.
- Response: `list[CatalogItemLineRow]`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItemLineRow`, `Depends`, `HTTPException`, `Query`, `_trade_purchase_date_filter`, `db.execute`, `desc`, `func.coalesce`, `ir.first`, `max`, `min`, `redact_catalog_line_row_model`, `resolve_from_text`, `resolve_from_text(str(iname or '')).as_dict`, `router.get`, `select`, `select(CatalogItem.id).where`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from(TradePurchaseLine).join`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).outerjoin`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).outerjoin`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).outerjoin(Broker, Broker.id == TradePurchase.broker_id).where`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).outerjoin(Broker, Broker.id == TradePurchase.broker_id).where(trade_bf, TradePurchaseLine.catalog_item_id == item_id).order_by`, `select(TradePurchaseLine.id, TradePurchase.purchase_date, TradePurchase.human_id, TradePurchaseLine.qty, TradePurchaseLine.unit, TradePurchaseLine.landing_cost, sell_disp, profit_e, TradePurchaseLine.kg_per_unit, TradePurchaseLine.landing_cost_per_kg, Supplier.name, Supplier.phone, Broker.name, Broker.phone, TradePurchaseLine.item_name).select_from(TradePurchaseLine).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).outerjoin(Broker, Broker.id == TradePurchase.broker_id).where(trade_bf, TradePurchaseLine.catalog_item_id == item_id).order_by(desc(TradePurchase.purchase_date), desc(TradePurchaseLine.id)).limit`, `should_redact_financials`, `tq.trade_line_profit_expr`, `tr.all`, `trade_rows.append`, `trade_rows.sort`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/item-categories/{category_id}/insights — category_insights

- VERIFIED_CODE: `backend/app/routers/catalog.py:2833`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, category_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], from_date: date=Query(..., alias='from'), to_date: date=Query(..., alias='to')`.
- Response: `CategoryInsightsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CategoryInsightsOut`, `Depends`, `HTTPException`, `Query`, `TradePurchaseLine.catalog_item_id.in_`, `_trade_purchase_date_filter`, `and_`, `cr.first`, `db.execute`, `func.coalesce`, `func.count`, `func.sum`, `ic.scalar`, `lc.scalar`, `max`, `min`, `per_item.all`, `router.get`, `select`, `select(CatalogItem.id).where`, `select(CatalogItem.id).where(CatalogItem.business_id == business_id, CatalogItem.category_id == category_id).scalar_subquery`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(profit_e), 0)).select_from`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(profit_e), 0)).select_from(CatalogItem).join`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(profit_e), 0)).select_from(CatalogItem).join(TradePurchaseLine, and_(TradePurchaseLine.catalog_item_id == CatalogItem.id)).join`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(profit_e), 0)).select_from(CatalogItem).join(TradePurchaseLine, and_(TradePurchaseLine.catalog_item_id == CatalogItem.id)).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(CatalogItem.id, CatalogItem.name, func.coalesce(func.sum(profit_e), 0)).select_from(CatalogItem).join(TradePurchaseLine, and_(TradePurchaseLine.catalog_item_id == CatalogItem.id)).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where(bf, CatalogItem.category_id == category_id, CatalogItem.business_id == business_id).group_by`, `select(ItemCategory.id).where`, `select(func.coalesce(func.sum(profit_e), 0)).select_from`, `select(func.coalesce(func.sum(profit_e), 0)).select_from(TradePurchaseLine).join`, `select(func.coalesce(func.sum(profit_e), 0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(func.count(CatalogItem.id)).where`, `select(func.count(TradePurchaseLine.id)).select_from`, `select(func.count(TradePurchaseLine.id)).select_from(TradePurchaseLine).join`, `select(func.count(TradePurchaseLine.id)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `tp.scalar`, `tq.trade_line_profit_expr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/catalog-items/{item_id} — update_catalog_item

- VERIFIED_CODE: `backend/app/routers/catalog.py:2929`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CatalogItemUpdate`.
- Response: `CatalogItemOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `(i.name or '').split`, `D`, `Decimal`, `Depends`, `HTTPException`, `_assert_broker_ids_in_business`, `_assert_supplier_ids_in_business`, `_catalog_item_out`, `_coerce_box_items_per_box`, `_dedupe_preserve_order`, `_default_supplier_broker_ids_for_items`, `_get_or_create_general_type_id`, `_invalidate_catalog_read_caches`, `_item_dup`, `_last_party_names_for_catalog_items`, `_last_purchase_delivered_for_snapshot`, `_max_purchase_date_for_catalog_item`, `_norm_name`, `_replace_default_broker_rows`, `_replace_default_supplier_rows`, `_seed_supplier_item_defaults`, `_sync_item_unit_extras`, `_sync_stock_fields_from_default_unit`, `_validate_item_unit_constraints`, `_verify_type_in_category`, `any`, `body.model_dump`, `br_m.get`, `catalog_items_has_type_id_column`, `crn.scalar_one_or_none`, `crnx.scalar_one_or_none`, `data.get`, `data['hsn_code'].strip`, `data['name'].strip`, `db.commit`, `db.execute`, `db.refresh`, `dup_q.where`, `er.scalar_one`, `func.lower`, `ic.strip`, `lbn.get`, `load_only`, `lsn.get`, `merge_unit_resolution_into_catalog_row`, `partsx[0].isalpha`, `r.scalar_one_or_none`, `rc.first`, `resolve_for_catalog_item`, `router.patch`, `rr.scalar_one`, `select`, `select(CatalogItem).options`, `select(CatalogItem).options(load_only(*_CATALOG_ITEM_CORE)).where`, `select(CatalogItem).where`, `select(CatalogItem.id).where`, `select(CategoryType.name).where`, `select(ItemCategory.id).where`, `select(ItemCategory.name).where`, `stmt.options`, `sup_m.get`, `tr.scalar_one_or_none`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/catalog-items/{item_id} — delete_catalog_item

- VERIFIED_CODE: `backend/app/routers/catalog.py:3163`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_invalidate_catalog_read_caches`, `catalog_items_has_type_id_column`, `count_archived_entry_lines_for_variants`, `db.commit`, `db.delete`, `db.execute`, `ec.scalar`, `func.count`, `load_only`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(CatalogItem).where`, `select(CatalogVariant.id).where`, `select(func.count(TradePurchaseLine.id)).where`, `stmt.options`, `vr.all`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/catalog-items/{item_id}/variants — list_catalog_variants

- VERIFIED_CODE: `backend/app/routers/catalog.py:3207`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[CatalogVariantOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogVariantOut`, `Depends`, `db.execute`, `func.lower`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(CatalogVariant).where`, `select(CatalogVariant).where(CatalogVariant.business_id == business_id, CatalogVariant.catalog_item_id == item_id).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/catalog-items/{item_id}/variants — create_catalog_variant

- VERIFIED_CODE: `backend/app/routers/catalog.py:3235`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CatalogVariantCreate`.
- Response: `CatalogVariantOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `CatalogVariant`, `CatalogVariantOut`, `Depends`, `HTTPException`, `_variant_dup`, `body.name.strip`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `ir.first`, `router.post`, `select`, `select(CatalogItem.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/catalog-variants/{variant_id} — update_catalog_variant

- VERIFIED_CODE: `backend/app/routers/catalog.py:3268`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, variant_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: CatalogVariantUpdate`.
- Response: `CatalogVariantOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `CatalogVariantOut`, `Depends`, `HTTPException`, `_variant_dup`, `body.model_dump`, `data['name'].strip`, `db.commit`, `db.execute`, `db.refresh`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(CatalogVariant).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/catalog-variants/{variant_id} — delete_catalog_variant

- VERIFIED_CODE: `backend/app/routers/catalog.py:3303`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, variant_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `count_archived_entry_lines_for_variant`, `db.commit`, `db.delete`, `db.execute`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(CatalogVariant).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/client-errors — ingest_client_error

- VERIFIED_CODE: `backend/app/routers/client_errors.py:29`.
- Request / authorization / tenant contract: `payload: ClientErrorPayload`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `logger.info`, `logger.warning`, `router.post`, `time.monotonic`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/suppliers — list_suppliers

- VERIFIED_CODE: `backend/app/routers/contacts.py:378`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], compact: Annotated[bool, Query(description='Omit address/notes; skips loading those DB columns when true.')]=False, limit: Annotated[int | None, Query(ge=1, le=5000, description='Max rows (only applied when compact=true).')]=None`.
- Response: `list[SupplierOut] | list[SupplierOutCompact]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `BrokerSupplierLink.supplier_id.in_`, `Depends`, `Query`, `Supplier.name.asc`, `SupplierOut.model_validate`, `SupplierOut.model_validate(s).model_dump`, `SupplierOutCompact`, `bool`, `broker_map.get`, `broker_map[sid].append`, `db.execute`, `defaultdict`, `execute_with_retry`, `load_only`, `logger.info`, `out.append`, `r.scalars`, `r.scalars().all`, `rb.all`, `router.get`, `select`, `select(BrokerSupplierLink.supplier_id, BrokerSupplierLink.broker_id).where`, `select(Supplier).options`, `select(Supplier).options(load_only(Supplier.id, Supplier.name, Supplier.phone, Supplier.gst_number, Supplier.default_payment_days, Supplier.default_discount, Supplier.default_delivered_rate, Supplier.default_billty_rate, Supplier.location, Supplier.freight_type, Supplier.ai_memory_enabled, Supplier.preferences_json, Supplier.broker_id)).where`, `select(Supplier).options(load_only(Supplier.id, Supplier.name, Supplier.phone, Supplier.gst_number, Supplier.default_payment_days, Supplier.default_discount, Supplier.default_delivered_rate, Supplier.default_billty_rate, Supplier.location, Supplier.freight_type, Supplier.ai_memory_enabled, Supplier.preferences_json, Supplier.broker_id)).where(Supplier.business_id == business_id).order_by`, `select(Supplier).where`, `select(Supplier).where(Supplier.business_id == business_id).order_by`, `select(Supplier).where(Supplier.business_id == business_id).order_by(Supplier.name.asc()).limit`, `stmt.limit`, `time.perf_counter`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/suppliers — create_supplier

- VERIFIED_CODE: `backend/app/routers/contacts.py:514`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: SupplierCreate`.
- Response: `SupplierOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_409_CONFLICT.
- Implementation calls: `BrokerSupplierLink`, `Depends`, `HTTPException`, `Supplier`, `_supplier_dup`, `_supplier_out`, `body.name.strip`, `body.preferences.model_dump_json`, `db.add`, `db.commit`, `db.flush`, `db.refresh`, `db.scalar`, `dedup_brokers.append`, `merged_broker_ids.extend`, `merged_broker_ids.insert`, `router.post`, `select`, `select(Broker.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/suppliers/{supplier_id} — update_supplier

- VERIFIED_CODE: `backend/app/routers/contacts.py:584`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, supplier_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: SupplierUpdate`.
- Response: `SupplierOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `Broker.id.in_`, `BrokerSupplierLink`, `Depends`, `HTTPException`, `SupplierPrefsIn.model_validate`, `SupplierPrefsIn.model_validate(data['preferences']).model_dump_json`, `_supplier_dup`, `_supplier_out`, `body.model_dump`, `bool`, `br.scalars`, `br.scalars().all`, `data.get`, `data['name'].strip`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `dedup_brokers.append`, `delete`, `delete(BrokerSupplierLink).where`, `merged_broker_ids.extend`, `merged_broker_ids.insert`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(Broker.id).where`, `select(Supplier).where`, `set`, `setattr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/suppliers/{supplier_id} — delete_supplier

- VERIFIED_CODE: `backend/app/routers/contacts.py:674`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, supplier_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `TradePurchase.status.notin_`, `db.commit`, `db.delete`, `db.execute`, `delete`, `delete(BrokerSupplierLink).where`, `delete(SupplierItemDefault).where`, `ec.scalar`, `func.count`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(Supplier).where`, `select(func.count(TradePurchase.id)).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/brokers — list_brokers

- VERIFIED_CODE: `backend/app/routers/contacts.py:785`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[BrokerOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `BrokerOut.model_validate`, `BrokerOut.model_validate(b).model_dump`, `BrokerSupplierLink.broker_id.in_`, `Depends`, `db.execute`, `defaultdict`, `execute_with_retry`, `logger.info`, `out.append`, `r.scalars`, `r.scalars().all`, `router.get`, `rs.all`, `select`, `select(Broker).where`, `select(BrokerSupplierLink.broker_id, BrokerSupplierLink.supplier_id).where`, `sup_map.get`, `sup_map[bid].append`, `time.perf_counter`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/brokers — create_broker

- VERIFIED_CODE: `backend/app/routers/contacts.py:843`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: BrokerCreate`.
- Response: `BrokerOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_409_CONFLICT.
- Implementation calls: `Broker`, `BrokerSupplierLink`, `Depends`, `HTTPException`, `_broker_dup`, `_broker_out`, `body.image_url.strip`, `body.name.strip`, `body.preferences.model_dump_json`, `db.add`, `db.commit`, `db.flush`, `db.refresh`, `db.scalar`, `dedup_suppliers.append`, `router.post`, `select`, `select(Supplier).where`, `select(Supplier.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/brokers/{broker_id} — update_broker

- VERIFIED_CODE: `backend/app/routers/contacts.py:896`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, broker_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: BrokerUpdate`.
- Response: `BrokerOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `BrokerSupplierLink`, `Depends`, `HTTPException`, `Supplier.id.in_`, `SupplierPrefsIn.model_validate`, `SupplierPrefsIn.model_validate(data['preferences']).model_dump_json`, `_broker_dup`, `_broker_out`, `body.model_dump`, `data['name'].strip`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `db.scalar`, `dedup_suppliers.append`, `delete`, `delete(BrokerSupplierLink).where`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(Broker).where`, `select(Supplier).where`, `select(Supplier.id).where`, `set`, `sr.scalars`, `sr.scalars().all`, `v.strip`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/brokers/{broker_id} — delete_broker

- VERIFIED_CODE: `backend/app/routers/contacts.py:974`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, broker_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `TradePurchase.status.notin_`, `db.commit`, `db.delete`, `db.execute`, `delete`, `delete(BrokerSupplierLink).where`, `ec.scalar`, `func.count`, `r.scalar_one_or_none`, `router.delete`, `sc.scalar`, `select`, `select(Broker).where`, `select(func.count(Supplier.id)).where`, `select(func.count(TradePurchase.id)).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/brokers/{broker_id} — get_broker

- VERIFIED_CODE: `backend/app/routers/contacts.py:1019`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, broker_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `BrokerOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_broker_out`, `db.execute`, `r.scalar_one_or_none`, `router.get`, `select`, `select(Broker).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/brokers/{broker_id}/linked-suppliers — broker_linked_suppliers

- VERIFIED_CODE: `backend/app/routers/contacts.py:1036`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, broker_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[LinkedSupplierOut]`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `LinkedSupplierOut`, `Supplier.id.in_`, `br.scalar_one_or_none`, `db.execute`, `execute_with_retry`, `func.lower`, `r2.all`, `router.get`, `select`, `select(Broker.id).where`, `select(Supplier.id, Supplier.name, Supplier.phone).where`, `select(Supplier.id, Supplier.name, Supplier.phone).where(Supplier.business_id == business_id, Supplier.id.in_(supplier_ids_subq)).order_by`, `select(Supplier.id, Supplier.name, Supplier.phone).where(Supplier.business_id == business_id, Supplier.id.in_(supplier_ids_subq)).order_by(func.lower(Supplier.name)).limit`, `select(TradePurchase.supplier_id).where`, `select(TradePurchase.supplier_id).where(TradePurchase.business_id == business_id, TradePurchase.broker_id == broker_id, trade_purchase_status_in_reports()).distinct`, `trade_purchase_status_in_reports`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/suppliers/{supplier_id} — get_supplier

- VERIFIED_CODE: `backend/app/routers/contacts.py:1075`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, supplier_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `SupplierOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_supplier_out`, `db.execute`, `r.scalar_one_or_none`, `router.get`, `select`, `select(Supplier).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/suppliers/{supplier_id}/metrics — supplier_metrics

- VERIFIED_CODE: `backend/app/routers/contacts.py:1110`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, supplier_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], from_date: date=Query(..., alias='from'), to_date: date=Query(..., alias='to')`.
- Response: `SupplierMetricsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `(await db.execute(q)).one`, `Depends`, `HTTPException`, `Query`, `SupplierMetricsOut`, `_trade_purchase_date_filter`, `db.execute`, `func.coalesce`, `func.coalesce(func.sum(TradePurchaseLine.qty), 0).label`, `func.coalesce(func.sum(amt), 0).label`, `func.coalesce(func.sum(profit), 0).label`, `func.count`, `func.count(func.distinct(TradePurchase.id)).label`, `func.distinct`, `func.sum`, `r.scalar_one_or_none`, `router.get`, `select`, `select(Supplier).where`, `select(func.count(func.distinct(TradePurchase.id)).label('deals'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('total_qty'), func.coalesce(func.sum(profit), 0).label('total_profit'), func.coalesce(func.sum(amt), 0).label('purchase_amount')).select_from`, `select(func.count(func.distinct(TradePurchase.id)).label('deals'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('total_qty'), func.coalesce(func.sum(profit), 0).label('total_profit'), func.coalesce(func.sum(amt), 0).label('purchase_amount')).select_from(TradePurchaseLine).join`, `select(func.count(func.distinct(TradePurchase.id)).label('deals'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('total_qty'), func.coalesce(func.sum(profit), 0).label('total_profit'), func.coalesce(func.sum(amt), 0).label('purchase_amount')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `tq.trade_line_amount_expr`, `tq.trade_line_profit_expr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/brokers/{broker_id}/metrics — broker_metrics

- VERIFIED_CODE: `backend/app/routers/contacts.py:1163`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, broker_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], from_date: date=Query(..., alias='from'), to_date: date=Query(..., alias='to')`.
- Response: `BrokerMetricsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `(await db.execute(select(func.coalesce(func.sum(TradePurchase.commission_money), 0)).select_from(TradePurchase).where(*bf, TradePurchase.broker_id == broker_id))).scalar`, `(await db.execute(select(func.coalesce(func.sum(profit), 0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where(*bf, TradePurchase.broker_id == broker_id))).scalar`, `(await db.execute(select(func.count(func.distinct(TradePurchase.id))).select_from(TradePurchase).where(*bf, TradePurchase.broker_id == broker_id))).scalar`, `BrokerMetricsOut`, `Depends`, `HTTPException`, `Query`, `_trade_purchase_date_filter`, `db.execute`, `func.coalesce`, `func.count`, `func.distinct`, `func.sum`, `r.scalar_one_or_none`, `router.get`, `select`, `select(Broker).where`, `select(func.coalesce(func.sum(TradePurchase.commission_money), 0)).select_from`, `select(func.coalesce(func.sum(TradePurchase.commission_money), 0)).select_from(TradePurchase).where`, `select(func.coalesce(func.sum(profit), 0)).select_from`, `select(func.coalesce(func.sum(profit), 0)).select_from(TradePurchaseLine).join`, `select(func.coalesce(func.sum(profit), 0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(func.count(func.distinct(TradePurchase.id))).select_from`, `select(func.count(func.distinct(TradePurchase.id))).select_from(TradePurchase).where`, `tq.trade_line_profit_expr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/contacts/search — contacts_search

- VERIFIED_CODE: `backend/app/routers/contacts.py:1246`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], q: str=Query('', max_length=200), limit: int=Query(15, ge=1, le=50), scope: str | None=Query(None, description='Optional: suppliers|brokers|categories|catalog_types|items — fetch only that bucket')`.
- Response: `SearchOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogSubcategoryRow`, `Depends`, `ItemSearchHitOut`, `Query`, `SearchOut`, `_brokers_out_batch`, `_norm_name`, `_suppliers_out_batch`, `brokers.extend`, `brows.sort`, `bucket`, `cat_by_norm.items`, `cat_set.add`, `catalog_subcategories.append`, `db.execute`, `func.lower`, `func.lower(Broker.name).like`, `func.lower(CatalogItem.name).like`, `func.lower(CategoryType.name).like`, `func.lower(ItemCategory.name).like`, `func.lower(Supplier.name).like`, `func.lower(TradePurchaseLine.item_name).like`, `func.trim`, `h.name.strip`, `id_by_norm.get`, `kv[1][0].strip`, `kv[1][0].strip().lower`, `kv[1][0].strip().lower().startswith`, `line_by_norm.items`, `merged.items`, `or_`, `q.strip`, `r2.all`, `rb.scalars`, `rb.scalars().all`, `ri.all`, `ric.all`, `ricc.all`, `router.get`, `rows.sort`, `rs.scalars`, `rs.scalars().all`, `rsub.all`, `scope.strip`, `scope.strip().lower`, `select`, `select(Broker).where`, `select(Broker).where(Broker.business_id == business_id, func.lower(Broker.name).like(like_contains)).limit`, `select(CatalogItem.id, CatalogItem.name).where`, `select(CatalogItem.id, CatalogItem.name).where(CatalogItem.business_id == business_id, func.lower(CatalogItem.name).like(like_contains)).distinct`, `select(CatalogItem.id, CatalogItem.name).where(CatalogItem.business_id == business_id, func.lower(CatalogItem.name).like(like_contains)).distinct().limit`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(ItemCategory.business_id == business_id, or_(func.lower(CategoryType.name).like(like_contains), func.lower(ItemCategory.name).like(like_contains))).distinct`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(ItemCategory.business_id == business_id, or_(func.lower(CategoryType.name).like(like_contains), func.lower(ItemCategory.name).like(like_contains))).distinct().limit`, `select(ItemCategory.name).where`, `select(ItemCategory.name).where(ItemCategory.business_id == business_id, func.lower(ItemCategory.name).like(like_contains)).distinct`, `select(ItemCategory.name).where(ItemCategory.business_id == business_id, func.lower(ItemCategory.name).like(like_contains)).distinct().limit`, `select(Supplier).where`, `select(Supplier).where(Supplier.business_id == business_id, func.lower(Supplier.name).like(like_contains)).limit`, `select(TradePurchaseLine.item_name).distinct`, `select(TradePurchaseLine.item_name).distinct().select_from`, `select(TradePurchaseLine.item_name).distinct().select_from(TradePurchaseLine).join`, `select(TradePurchaseLine.item_name).distinct().select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(TradePurchaseLine.item_name).distinct().select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where(TradePurchase.business_id == business_id, trade_purchase_status_in_reports(), func.lower(TradePurchaseLine.item_name).like(like_contains)).limit`, `set`, `sorted`, `sub_pairs.sort`, `suppliers.extend`, `trade_purchase_status_in_reports`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/contacts/category-items — category_items

- VERIFIED_CODE: `backend/app/routers/contacts.py:1482`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], category: str=Query(..., min_length=1, max_length=255), from_date: date=Query(..., alias='from'), to_date: date=Query(..., alias='to')`.
- Response: `list[CategoryItemRow]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CategoryItemRow`, `Depends`, `ItemCategory.id.is_`, `Query`, `_trade_purchase_date_filter`, `db.execute`, `func.coalesce`, `func.coalesce(func.sum(TradePurchaseLine.profit), 0).desc`, `func.coalesce(func.sum(TradePurchaseLine.profit), 0).label`, `func.coalesce(func.sum(TradePurchaseLine.qty), 0).label`, `func.count`, `func.count(TradePurchaseLine.id).label`, `func.sum`, `r.all`, `router.get`, `select`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).join`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(*tf, ItemCategory.name == category).group_by`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(*tf, ItemCategory.name == category).group_by(TradePurchaseLine.item_name).order_by`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).outerjoin`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(*tf, ItemCategory.id.is_(None)).group_by`, `select(TradePurchaseLine.item_name, func.count(TradePurchaseLine.id).label('lc'), func.coalesce(func.sum(TradePurchaseLine.profit), 0).label('tp'), func.coalesce(func.sum(TradePurchaseLine.qty), 0).label('tq')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).join(CatalogItem, CatalogItem.id == TradePurchaseLine.catalog_item_id).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(*tf, ItemCategory.id.is_(None)).group_by(TradePurchaseLine.item_name).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/damage-reports/pending-count — pending_damage_reports_count

- VERIFIED_CODE: `backend/app/routers/damage_reports.py:25`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `PendingDamageReportsCountOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `PendingDamageReportsCountOut`, `pds.count_pending_damage_reports`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/damage-reports/{report_id} — patch_damage_report_status

- VERIFIED_CODE: `backend/app/routers/damage_reports.py:36`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, report_id: uuid.UUID, body: PurchaseDamageReportStatusPatch, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))]`.
- Response: `PurchaseDamageReportOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `PurchaseDamageReportOut`, `pds.damage_report_to_out`, `pds.update_damage_report_status`, `require_role`, `router.patch`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/exports/backup — post_backup_zip

- VERIFIED_CODE: `backend/app/routers/exports.py:69`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)], body: BackupRequest`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_413_REQUEST_ENTITY_TOO_LARGE.
- Implementation calls: `(br.scalar_one_or_none() or '').strip`, `(plist[0].supplier_row.name or '').strip`, `Depends`, `HTTPException`, `Response`, `TradePurchase.purchase_date.desc`, `TradePurchaseLine.trade_purchase_id.in_`, `_range_dates`, `_zip_path_segment`, `br.scalar_one_or_none`, `buf.getvalue`, `buf.seek`, `build_purchase_order_pdf`, `build_purchases_range_pdf`, `build_stock_inventory_xlsx`, `build_supplier_ledger_pdf`, `by_supplier.items`, `by_supplier[p.supplier_id].append`, `conds.append`, `count_r.scalar_one`, `d_to.isoformat`, `date.today`, `db.execute`, `defaultdict`, `fetch_stock_inventory_rows`, `func.count`, `io.BytesIO`, `lines_by_purchase.get`, `lines_by_purchase[ln.trade_purchase_id].append`, `logger.error`, `lr.scalars`, `lr.scalars().all`, `min`, `pr.scalars`, `pr.scalars().all`, `preset_titles.get`, `range_start.isoformat`, `require_permission`, `router.post`, `select`, `select(Business.name).where`, `select(TradePurchase).where`, `select(TradePurchase).where(*conds).options`, `select(TradePurchase).where(*conds).options(selectinload(TradePurchase.supplier_row)).order_by`, `select(TradePurchase).where(*conds).options(selectinload(TradePurchase.supplier_row)).order_by(TradePurchase.purchase_date.desc()).limit`, `select(TradePurchaseLine).where`, `select(func.count()).select_from`, `select(func.count()).select_from(TradePurchase).where`, `selectinload`, `sum`, `tq.trade_purchase_status_in_reports`, `zf.writestr`, `zipfile.ZipFile`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/exports/stock-inventory.xlsx — get_stock_inventory_xlsx

- VERIFIED_CODE: `backend/app/routers/exports.py:229`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `Response`, `build_stock_inventory_xlsx`, `date.today`, `date.today().isoformat`, `fetch_stock_inventory_rows`, `logger.error`, `require_permission`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/exports/purchases-month.pdf — get_purchases_month_pdf

- VERIFIED_CODE: `backend/app/routers/exports.py:263`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(br.scalar_one_or_none() or '').strip`, `Depends`, `HTTPException`, `Response`, `br.scalar_one_or_none`, `build_purchases_month_pdf`, `date`, `date.today`, `db.execute`, `fetch_month_trade_purchases`, `logger.error`, `require_permission`, `router.get`, `select`, `select(Business.name).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/exports/backup/export — get_backup_export_json

- VERIFIED_CODE: `backend/app/routers/exports.py:304`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(await db.execute(select(CatalogItem).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None)).order_by(CatalogItem.name.asc()).limit(5000))).scalars`, `(await db.execute(select(CatalogItem).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None)).order_by(CatalogItem.name.asc()).limit(5000))).scalars().all`, `(await db.execute(select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.created_at >= d_from).order_by(StockAudit.created_at.desc()).limit(500))).scalars`, `(await db.execute(select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.created_at >= d_from).order_by(StockAudit.created_at.desc()).limit(500))).scalars().all`, `(await db.execute(select(Supplier).where(Supplier.business_id == business_id).order_by(Supplier.name.asc()).limit(2000))).scalars`, `(await db.execute(select(Supplier).where(Supplier.business_id == business_id).order_by(Supplier.name.asc()).limit(2000))).scalars().all`, `CatalogItem.deleted_at.is_`, `CatalogItem.name.asc`, `Depends`, `Response`, `StockAudit.created_at.desc`, `Supplier.name.asc`, `TradePurchase.purchase_date.desc`, `TradePurchaseLine.trade_purchase_id.in_`, `_catalog_item_row`, `a.audit_date.isoformat`, `a.created_at.isoformat`, `date.today`, `db.execute`, `defaultdict`, `getattr`, `json.dumps`, `json.dumps(payload, default=str).encode`, `lines_by_purchase.get`, `lines_by_purchase[ln.trade_purchase_id].append`, `lr.scalars`, `lr.scalars().all`, `p.purchase_date.isoformat`, `pr.scalars`, `pr.scalars().all`, `require_permission`, `router.get`, `select`, `select(CatalogItem).where`, `select(CatalogItem).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None)).order_by`, `select(CatalogItem).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None)).order_by(CatalogItem.name.asc()).limit`, `select(StockAudit).where`, `select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.created_at >= d_from).order_by`, `select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.created_at >= d_from).order_by(StockAudit.created_at.desc()).limit`, `select(Supplier).where`, `select(Supplier).where(Supplier.business_id == business_id).order_by`, `select(Supplier).where(Supplier.business_id == business_id).order_by(Supplier.name.asc()).limit`, `select(TradePurchase).where`, `select(TradePurchase).where(*conds).options`, `select(TradePurchase).where(*conds).options(selectinload(TradePurchase.supplier_row)).order_by`, `select(TradePurchase).where(*conds).options(selectinload(TradePurchase.supplier_row)).order_by(TradePurchase.purchase_date.desc()).limit`, `select(TradePurchaseLine).where`, `selectinload`, `timedelta`, `today.isoformat`, `today.strftime`, `tq.trade_purchase_status_in_reports`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/exports/backup/logs — list_backup_logs

- VERIFIED_CODE: `backend/app/routers/exports.py:446`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(await db.execute(select(BackupLog).where(BackupLog.business_id == business_id).order_by(BackupLog.created_at.desc()).limit(50))).scalars`, `(await db.execute(select(BackupLog).where(BackupLog.business_id == business_id).order_by(BackupLog.created_at.desc()).limit(50))).scalars().all`, `BackupLog.created_at.desc`, `Depends`, `db.execute`, `r.created_at.isoformat`, `require_permission`, `router.get`, `select`, `select(BackupLog).where`, `select(BackupLog).where(BackupLog.business_id == business_id).order_by`, `select(BackupLog).where(BackupLog.business_id == business_id).order_by(BackupLog.created_at.desc()).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/exports/backup/run — run_backup_now

- VERIFIED_CODE: `backend/app/routers/exports.py:481`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `apply_retention`, `require_permission`, `router.post`, `write_scheduled_backup`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/exports/restore/dry-run — restore_dry_run

- VERIFIED_CODE: `backend/app/routers/exports.py:502`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: RestoreDryRunBody, m: Annotated[Membership, Depends(require_permission('export_access'))], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_403_FORBIDDEN.
- Implementation calls: `(body.payload or {}).get`, `Depends`, `HTTPException`, `dry_run_restore`, `require_permission`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/exports/restore/commit — restore_commit

- VERIFIED_CODE: `backend/app/routers/exports.py:523`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: RestoreDryRunBody, m: Annotated[Membership, Depends(require_permission('export_access'))]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_403_FORBIDDEN, status.HTTP_501_NOT_IMPLEMENTED.
- Implementation calls: `Depends`, `HTTPException`, `require_permission`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET / — root

- VERIFIED_CODE: `backend/app/routers/health.py:21`.
- Request / authorization / tenant contract: ``.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /health/live — health_live

- VERIFIED_CODE: `backend/app/routers/health.py:40`.
- Request / authorization / tenant contract: ``.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /health — health

- VERIFIED_CODE: `backend/app/routers/health.py:46`.
- Request / authorization / tenant contract: `settings: Settings=Depends(get_settings)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(settings.ai_provider or 'stub').strip`, `(settings.ai_provider or 'stub').strip().lower`, `(settings.google_ai_api_key or '').strip`, `(settings.groq_api_key or '').strip`, `(settings.openai_api_key or '').strip`, `(settings.openrouter_api_key or '').strip`, `(settings.redis_url or '').strip`, `Depends`, `bool`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /health/ready — health_ready

- VERIFIED_CODE: `backend/app/routers/health.py:85`.
- Request / authorization / tenant contract: `db: AsyncSession=Depends(get_db)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(chk.scalar() or '').upper`, `Depends`, `JSONResponse`, `asyncio.wait_for`, `bool`, `chk.scalar`, `col.scalar`, `db.execute`, `ds.scalar`, `logger.exception`, `router.get`, `schema.get`, `schema.setdefault`, `text`, `time.perf_counter`, `ver.scalar`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /health/db-check — health_db_check

- VERIFIED_CODE: `backend/app/routers/health.py:167`.
- Request / authorization / tenant contract: `db: AsyncSession=Depends(get_db)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `db.execute`, `logger.exception`, `router.get`, `text`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/me/profile — get_my_profile

- VERIFIED_CODE: `backend/app/routers/me.py:59`.
- Request / authorization / tenant contract: `user: Annotated[User, Depends(get_current_user)]`.
- Response: `UserProfileOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `UserProfileOut`, `bool`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/me/profile — patch_my_profile

- VERIFIED_CODE: `backend/app/routers/me.py:70`.
- Request / authorization / tenant contract: `user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], body: UserProfilePatch`.
- Response: `UserProfileOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `UserProfileOut`, `body.name.strip`, `bool`, `db.commit`, `db.refresh`, `router.patch`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/me/bootstrap-workspace — post_bootstrap_workspace

- VERIFIED_CODE: `backend/app/routers/me.py:112`.
- Request / authorization / tenant contract: `user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)]`.
- Response: `BootstrapWorkspaceOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `BootstrapWorkspaceOut`, `Depends`, `bootstrap_user_workspace`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/me/businesses — my_businesses

- VERIFIED_CODE: `backend/app/routers/me.py:123`.
- Request / authorization / tenant contract: `user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `list[BusinessBrief]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `_business_brief`, `db.execute`, `effective_permissions`, `out.append`, `q.all`, `router.get`, `select`, `select(Membership, Business).join`, `select(Membership, Business).join(Business, Business.id == Membership.business_id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/me/businesses/{business_id}/branding — patch_my_business_branding

- VERIFIED_CODE: `backend/app/routers/me.py:154`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)], body: BusinessBrandingPatch`.
- Response: `BusinessBrief`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_business_brief`, `a.strip`, `body.model_dump`, `db.commit`, `db.execute`, `db.refresh`, `e.strip`, `e.strip().lower`, `g.strip`, `g.strip().upper`, `membership_permissions`, `n.strip`, `p.strip`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(Business).where`, `t.strip`, `u.strip`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/me/businesses/{business_id}/branding/logo — upload_business_logo

- VERIFIED_CODE: `backend/app/routers/me.py:212`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _owner: Annotated[Membership, Depends(require_owner_membership)], db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)], file: UploadFile=File(...)`.
- Response: `BusinessBrief`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND, status.HTTP_501_NOT_IMPLEMENTED.
- Implementation calls: `(file.content_type or '').split`, `(file.content_type or '').split(';')[0].strip`, `(file.content_type or '').split(';')[0].strip().lower`, `(settings.app_env or '').lower`, `(settings.s3_bucket or '').strip`, `Depends`, `File`, `HTTPException`, `_branding_storage_dir`, `_business_brief`, `bool`, `db.commit`, `db.execute`, `db.refresh`, `dest.write_bytes`, `dest_dir.mkdir`, `file.read`, `membership_permissions`, `r.scalar_one_or_none`, `router.post`, `select`, `select(Business).where`, `settings.app_url.rstrip`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/media/ocr — ocr_image

- VERIFIED_CODE: `backend/app/routers/media.py:52`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], settings: Annotated[Settings, Depends(get_settings)], body: OcrRequest`.
- Response: `OcrResponse`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(body.paste_text or '').strip`, `Depends`, `OcrLineOut`, `OcrResponse`, `ai_meta.get`, `base64.b64decode`, `extract_item_rows_via_ai`, `extract_purchase_lines_from_text`, `r.get`, `raw.decode`, `raw.decode('utf-8', errors='ignore').strip`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/notifications — list_notifications

- VERIFIED_CODE: `backend/app/routers/notifications.py:69`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(30, ge=1, le=100), kind: str | None=Query(default=None, max_length=64), category: str | None=Query(default=None, max_length=32), priority: str | None=Query(default=None, max_length=16), unread_only: bool=Query(default=False), q: str | None=Query(default=None, max_length=120)`.
- Response: `list[NotificationOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(_m.role or '').strip`, `(_m.role or '').strip().lower`, `(actor_name or '').strip`, `AppNotification.body.ilike`, `AppNotification.created_at.desc`, `AppNotification.read_at.is_`, `AppNotification.title.ilike`, `Depends`, `NotificationOut.model_validate`, `Query`, `_notification_visible_to_role`, `_role_visibility_sql`, `_user_filters`, `base.model_copy`, `category.strip`, `db.execute`, `filters.append`, `kind.strip`, `or_`, `out.append`, `priority.strip`, `q.strip`, `r.all`, `router.get`, `select`, `select(AppNotification, User.name).outerjoin`, `select(AppNotification, User.name).outerjoin(User, AppNotification.triggered_by_user_id == User.id).where`, `select(AppNotification, User.name).outerjoin(User, AppNotification.triggered_by_user_id == User.id).where(*filters).order_by`, `select(AppNotification, User.name).outerjoin(User, AppNotification.triggered_by_user_id == User.id).where(*filters).order_by(AppNotification.created_at.desc()).offset`, `select(AppNotification, User.name).outerjoin(User, AppNotification.triggered_by_user_id == User.id).where(*filters).order_by(AppNotification.created_at.desc()).offset(off).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/notifications/summary — notifications_summary

- VERIFIED_CODE: `backend/app/routers/notifications.py:127`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `NotificationSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(_m.role or '').strip`, `(_m.role or '').strip().lower`, `AppNotification.read_at.is_`, `Depends`, `NotificationSummaryOut`, `_role_visibility_sql`, `_user_filters`, `cr.all`, `db.execute`, `func.count`, `pr.all`, `r.scalar_one`, `router.get`, `select`, `select(AppNotification.category, func.count()).where`, `select(AppNotification.category, func.count()).where(*base).group_by`, `select(AppNotification.priority, func.count()).where`, `select(AppNotification.priority, func.count()).where(*base).group_by`, `select(func.count()).select_from`, `select(func.count()).select_from(AppNotification).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/notifications/unread-count — unread_count

- VERIFIED_CODE: `backend/app/routers/notifications.py:161`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `UnreadCountOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(_m.role or '').strip`, `(_m.role or '').strip().lower`, `AppNotification.read_at.is_`, `Depends`, `UnreadCountOut`, `_role_visibility_sql`, `_user_filters`, `db.execute`, `func.count`, `r.scalar_one`, `router.get`, `select`, `select(func.count()).select_from`, `select(func.count()).select_from(AppNotification).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/notifications/mark-all-read — mark_all_read

- VERIFIED_CODE: `backend/app/routers/notifications.py:181`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], kind: str | None=Query(default=None, max_length=64)`.
- Response: `NotificationBulkActionOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `AppNotification.read_at.is_`, `Depends`, `NotificationBulkActionOut`, `Query`, `_user_filters`, `datetime.now`, `db.commit`, `db.execute`, `filters.append`, `kind.strip`, `publish_notification_changed`, `router.post`, `update`, `update(AppNotification).where`, `update(AppNotification).where(*filters).values`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/notifications/clear-all — clear_all

- VERIFIED_CODE: `backend/app/routers/notifications.py:203`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], kind: str | None=Query(default=None, max_length=64)`.
- Response: `NotificationBulkActionOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `NotificationBulkActionOut`, `Query`, `_user_filters`, `db.commit`, `db.execute`, `delete`, `delete(AppNotification).where`, `filters.append`, `kind.strip`, `publish_notification_changed`, `router.delete`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/notifications/client-event — client_notification_event

- VERIFIED_CODE: `backend/app/routers/notifications.py:221`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: ClientNotificationEventIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `NotificationBulkActionOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `Depends`, `HTTPException`, `NotificationBulkActionOut`, `body.kind.strip`, `body.title.strip`, `db.commit`, `emit_notification`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/notifications/{notification_id} — patch_notification

- VERIFIED_CODE: `backend/app/routers/notifications.py:255`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, notification_id: uuid.UUID, body: NotificationReadPatch, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `NotificationOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `NotificationOut.model_validate`, `datetime.now`, `db.commit`, `db.execute`, `db.refresh`, `publish_notification_changed`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(AppNotification).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/checklist/today — checklist_today

- VERIFIED_CODE: `backend/app/routers/operations.py:139`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `ChecklistTodayOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `ChecklistTaskOut`, `ChecklistTodayOut`, `Depends`, `_templates_for_business`, `cr.scalars`, `cr.scalars().all`, `date.today`, `db.execute`, `done.get`, `round`, `router.get`, `select`, `select(StaffChecklistCompletion).where`, `sum`, `tasks.append`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/operations/checklist/{slot}/complete — checklist_complete

- VERIFIED_CODE: `backend/app/routers/operations.py:175`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, slot: ChecklistSlot, body: ChecklistCompleteIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `Depends`, `HTTPException`, `StaffChecklistCompletion`, `_templates_for_business`, `date.today`, `db.add`, `db.commit`, `db.execute`, `db.rollback`, `existing.scalar_one_or_none`, `log_staff_activity`, `router.post`, `select`, `select(StaffChecklistCompletion).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/usage/today — usage_today

- VERIFIED_CODE: `backend/app/routers/operations.py:227`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `UsageTodayOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.current_stock.isnot`, `CatalogItem.deleted_at.is_`, `DailyUsageLog.item_id.in_`, `Decimal`, `Depends`, `UsageLineOut`, `UsageTodayOut`, `_purchased_today_map`, `catalog_stock_qty`, `date.today`, `db.execute`, `ir.scalars`, `ir.scalars().all`, `lines.append`, `logs.get`, `lr.scalars`, `lr.scalars().all`, `purchased.get`, `router.get`, `select`, `select(CatalogItem).where`, `select(CatalogItem).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), CatalogItem.current_stock.isnot(None), CatalogItem.current_stock > 0).order_by`, `select(DailyUsageLog).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/operations/usage/today — usage_submit

- VERIFIED_CODE: `backend/app/routers/operations.py:298`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: UsageSubmitIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `UsageSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `CatalogItem.id.in_`, `DailyUsageLog`, `Decimal`, `Depends`, `HTTPException`, `UsageSummaryOut`, `_purchased_today_map`, `apply_stock_movement`, `catalog_stock_qty`, `date.today`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `ex.scalar_one_or_none`, `func.count`, `ir.scalar_one`, `ir.scalars`, `ir.scalars().all`, `items_by_id.get`, `log_staff_activity`, `max`, `purchased_map.get`, `require_permission`, `router.post`, `select`, `select(CatalogItem).where`, `select(DailyUsageLog).where`, `select(func.count()).select_from`, `select(func.count()).select_from(CatalogItem).where`, `today.isoformat`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/checklist/templates — list_checklist_templates

- VERIFIED_CODE: `backend/app/routers/operations.py:411`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `list[ChecklistTemplateOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `ChecklistTemplateOut`, `Depends`, `_templates_for_business`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PUT /v1/businesses/{business_id}/operations/checklist/templates — replace_checklist_templates

- VERIFIED_CODE: `backend/app/routers/operations.py:431`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: Annotated[ChecklistTemplatesPutIn, Body()], db: Annotated[AsyncSession, Depends(get_db)], membership: Annotated[Membership, Depends(require_membership)]`.
- Response: `list[ChecklistTemplateOut]`.
- Explicit error branches: status.HTTP_403_FORBIDDEN.
- Implementation calls: `(item.task_key or '').strip`, `Body`, `ChecklistTemplateOut`, `Depends`, `HTTPException`, `StaffChecklistTemplate`, `_slug_task_key`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `delete`, `delete(StaffChecklistTemplate).where`, `enumerate`, `item.label.strip`, `router.put`, `rows.append`, `seen_keys.add`, `set`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/checklist/summary — checklist_summary

- VERIFIED_CODE: `backend/app/routers/operations.py:485`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `ChecklistSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `ChecklistSummaryOut`, `Depends`, `_templates_for_business`, `cr.scalar_one`, `date.today`, `db.execute`, `func.concat`, `func.count`, `func.distinct`, `literal`, `round`, `router.get`, `select`, `select(func.count(func.distinct(slot_key))).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/operations/snapshots/materialize — materialize_daily_snapshots

- VERIFIED_CODE: `backend/app/routers/operations.py:517`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))], snapshot_date: str | None=None`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `DailyUsageLog`, `DailyUsageLog.item_id.in_`, `Decimal`, `Depends`, `_purchased_today_map`, `catalog_stock_qty`, `date.fromisoformat`, `date.today`, `date.today().isoformat`, `db.add`, `db.commit`, `db.execute`, `ex_r.all`, `ir.scalars`, `ir.scalars().all`, `prior_by_id.get`, `prior_r.scalars`, `prior_r.scalars().all`, `purchased_map.get`, `require_permission`, `router.post`, `select`, `select(CatalogItem).where`, `select(DailyUsageLog).where`, `select(DailyUsageLog.item_id).where`, `timedelta`, `ud.isoformat`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/snapshots — list_daily_snapshots

- VERIFIED_CODE: `backend/app/routers/operations.py:588`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], from_date: str | None=None, to_date: str | None=None, item_id: uuid.UUID | None=None`.
- Response: `list[DailySnapshotOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `DailySnapshotOut`, `DailyUsageLog.usage_date.desc`, `Depends`, `date.fromisoformat`, `date.today`, `db.execute`, `r.all`, `router.get`, `select`, `select(DailyUsageLog, CatalogItem.name).join`, `select(DailyUsageLog, CatalogItem.name).join(CatalogItem, DailyUsageLog.item_id == CatalogItem.id).where`, `select(DailyUsageLog, CatalogItem.name).join(CatalogItem, DailyUsageLog.item_id == CatalogItem.id).where(DailyUsageLog.business_id == business_id, DailyUsageLog.usage_date >= fd, DailyUsageLog.usage_date <= td).order_by`, `stmt.limit`, `stmt.where`, `today.isoformat`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/usage/summary — usage_summary

- VERIFIED_CODE: `backend/app/routers/operations.py:630`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], usage_date: str | None=None`.
- Response: `UsageSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `UsageSummaryOut`, `date.fromisoformat`, `date.today`, `date.today().isoformat`, `db.execute`, `func.coalesce`, `func.count`, `func.sum`, `ir.scalar_one`, `lr.one`, `max`, `router.get`, `select`, `select(func.count()).select_from`, `select(func.count()).select_from(CatalogItem).where`, `select(func.count(DailyUsageLog.id), func.coalesce(func.sum(DailyUsageLog.used_qty), 0)).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/operations/reports/summary — operational_reports_summary

- VERIFIED_CODE: `backend/app/routers/operations.py:667`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], stale_days: int=30`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `DailyUsageLog.item_id.in_`, `Decimal`, `Depends`, `StockAdjustmentLog.item_id.in_`, `_aging_bucket`, `_idle_days_for_item`, `_is_dead_stock_item`, `_movement_status`, `adj_r.all`, `catalog_stock_qty`, `date.today`, `db.execute`, `dead.append`, `fast.append`, `fast.sort`, `func.coalesce`, `func.count`, `func.count().desc`, `func.max`, `func.sum`, `ir.all`, `last_adj.get`, `last_adj[item.id].isoformat`, `router.get`, `select`, `select(CatalogItem, ItemCategory.name).join`, `select(CatalogItem, ItemCategory.name).join(ItemCategory, CatalogItem.category_id == ItemCategory.id).where`, `select(DailyUsageLog.item_id, func.coalesce(func.sum(DailyUsageLog.used_qty), 0)).where`, `select(DailyUsageLog.item_id, func.coalesce(func.sum(DailyUsageLog.used_qty), 0)).where(DailyUsageLog.business_id == business_id, DailyUsageLog.usage_date >= since_30, DailyUsageLog.item_id.in_(item_ids)).group_by`, `select(DailyUsageLog.item_id, func.coalesce(func.sum(DailyUsageLog.used_qty), 0)).where(DailyUsageLog.business_id == business_id, DailyUsageLog.usage_date >= since_7, DailyUsageLog.item_id.in_(item_ids)).group_by`, `select(StockAdjustmentLog.item_id, func.max(StockAdjustmentLog.updated_at)).where`, `select(StockAdjustmentLog.item_id, func.max(StockAdjustmentLog.updated_at)).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id.in_(item_ids)).group_by`, `select(TradePurchase.supplier_id, func.count()).where`, `select(TradePurchase.supplier_id, func.count()).where(TradePurchase.business_id == business_id, TradePurchase.status != 'cancelled', TradePurchase.purchase_date >= today.replace(day=1)).group_by`, `select(TradePurchase.supplier_id, func.count()).where(TradePurchase.business_id == business_id, TradePurchase.status != 'cancelled', TradePurchase.purchase_date >= today.replace(day=1)).group_by(TradePurchase.supplier_id).order_by`, `select(TradePurchase.supplier_id, func.count()).where(TradePurchase.business_id == business_id, TradePurchase.status != 'cancelled', TradePurchase.purchase_date >= today.replace(day=1)).group_by(TradePurchase.supplier_id).order_by(func.count().desc()).limit`, `slow.append`, `slow.sort`, `sr.all`, `timedelta`, `today.replace`, `ur.all`, `ur30.all`, `usage_30d.get`, `usage_7d.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/settings/credentials — get_credentials

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:60`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_owner_or_admin_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `credentials_router.get`, `pc.list_credentials`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PUT /v1/businesses/{business_id}/settings/credentials/{credential_type} — put_credential

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:70`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, credential_type: str, body: CredentialPut, m: Annotated[Membership, Depends(require_owner_or_admin_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `AdminAuditLog`, `Depends`, `HTTPException`, `credentials_router.put`, `db.add`, `db.commit`, `pc.upsert_credential`, `row.updated_at.isoformat`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/staff/tasks — list_all_tasks

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:112`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, m: Annotated[Membership, Depends(require_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], status_filter: str | None=None`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `_sees_all_tasks`, `st.list_tasks`, `st.task_to_dict`, `staff_router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/staff/{staff_id}/tasks — list_staff_tasks

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:129`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, staff_id: uuid.UUID, m: Annotated[Membership, Depends(require_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_403_FORBIDDEN.
- Implementation calls: `Depends`, `HTTPException`, `_sees_all_tasks`, `st.list_tasks`, `st.task_to_dict`, `staff_router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/staff/tasks — create_staff_task

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:143`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: StaffTaskCreate, m: Annotated[Membership, Depends(require_owner_or_admin_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `st.create_task`, `st.task_to_dict`, `staff_router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/staff/tasks/{task_id}/accept — accept_staff_task

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:163`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, task_id: uuid.UUID, m: Annotated[Membership, Depends(require_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `st.accept_task`, `st.task_to_dict`, `staff_router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/staff/tasks/{task_id}/complete — complete_staff_task

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:181`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, task_id: uuid.UUID, body: StaffTaskComplete, m: Annotated[Membership, Depends(require_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `st.complete_task`, `st.task_to_dict`, `staff_router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/staff/performance-summary — staff_performance_summary

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:207`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_owner_or_admin_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `st.performance_summary`, `staff_router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/owner/dashboard — owner_dashboard

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:217`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_owner_or_admin_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `od.build_owner_dashboard`, `owner_router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/owner/whatsapp-deliveries — list_whatsapp_deliveries

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:227`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_owner_or_admin_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `owner_router.get`, `wad.delivery_to_dict`, `wad.list_deliveries`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/owner/whatsapp-deliveries/{po_id}/resend — resend_whatsapp_delivery

- VERIFIED_CODE: `backend/app/routers/owner_ops.py:240`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, po_id: uuid.UUID, _m: Annotated[Membership, Depends(require_owner_or_admin_membership)], user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `owner_router.post`, `wad.deliver_po_whatsapp`, `wad.delivery_to_dict`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /public/items/{token}.json — public_item_json

- VERIFIED_CODE: `backend/app/routers/public_items.py:145`.
- Request / authorization / tenant contract: `token: str, request: Request`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `JSONResponse`, `_enforce_public_rate_limit`, `_load_public_item`, `_safe_item_payload`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /public/items/lookup — public_lookup

- VERIFIED_CODE: `backend/app/routers/public_items.py:162`.
- Request / authorization / tenant contract: `request: Request, barcode: str=Query(..., min_length=1, max_length=100), business: str=Query(..., min_length=1, max_length=255)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `HTTPException`, `JSONResponse`, `Query`, `_enforce_public_rate_limit`, `_latest_physical_qty`, `_resolve_business_id`, `_safe_item_payload`, `async_session_factory`, `barcode.strip`, `db.execute`, `delivered_map.get`, `movement_delivered_qty_map`, `router.get`, `row.all`, `select`, `select(CatalogItem, ItemCategory.name).join`, `select(CatalogItem, ItemCategory.name).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `select(CatalogItem, ItemCategory.name).join(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(CatalogItem.business_id == business_id, CatalogItem.barcode == clean, CatalogItem.deleted_at.is_(None)).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /public/items/{token} — public_item_page

- VERIFIED_CODE: `backend/app/routers/public_items.py:199`.
- Request / authorization / tenant contract: `token: str, request: Request`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `HTMLResponse`, `_enforce_public_rate_limit`, `_load_public_item`, `_safe_item_payload`, `html.escape`, `payload.get`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/realtime/events — sse_events

- VERIFIED_CODE: `backend/app/routers/realtime.py:19`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], _rt: Annotated[None, Depends(require_realtime_effective)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `StreamingResponse`, `asyncio.wait_for`, `gen`, `json.dumps`, `queue.get`, `router.get`, `subscribe_business_events`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/realtime/recent — recent_events

- VERIFIED_CODE: `backend/app/routers/realtime.py:39`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, m: Annotated[Membership, Depends(require_membership)], limit: int=Query(50, ge=1, le=100)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `recent_business_events`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/report-views — list_report_views

- VERIFIED_CODE: `backend/app/routers/report_views.py:51`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `ReportSavedView.updated_at.desc`, `_serialize`, `db.execute`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(ReportSavedView).where`, `select(ReportSavedView).where(ReportSavedView.business_id == business_id, ReportSavedView.user_id == user.id).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/report-views — create_report_view

- VERIFIED_CODE: `backend/app/routers/report_views.py:70`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: ReportViewIn, user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `ReportSavedView`, `_serialize`, `body.name.strip`, `body.tab.strip`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `router.post`, `update`, `update(ReportSavedView).where`, `update(ReportSavedView).where(ReportSavedView.business_id == business_id, ReportSavedView.user_id == user.id, ReportSavedView.tab == body.tab).values`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/report-views/{view_id} — patch_report_view

- VERIFIED_CODE: `backend/app/routers/report_views.py:103`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, view_id: uuid.UUID, body: ReportViewPatch, user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_serialize`, `body.name.strip`, `body.tab.strip`, `datetime.now`, `db.commit`, `db.execute`, `db.refresh`, `r.scalar_one_or_none`, `router.patch`, `select`, `select(ReportSavedView).where`, `update`, `update(ReportSavedView).where`, `update(ReportSavedView).where(ReportSavedView.business_id == business_id, ReportSavedView.user_id == user.id, ReportSavedView.tab == tab, ReportSavedView.id != view_id).values`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/report-views/{view_id} — delete_report_view

- VERIFIED_CODE: `backend/app/routers/report_views.py:149`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, view_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_204_NO_CONTENT, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `Response`, `db.commit`, `db.delete`, `db.execute`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(ReportSavedView).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/reports/sales-comparison — compare_sales_lines

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:121`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: SalesCompareIn, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `SequenceMatcher`, `SequenceMatcher(None, src, item['norm']).ratio`, `_norm_name`, `db.execute`, `max`, `out.append`, `r.all`, `round`, `router.post`, `select`, `select(CatalogItem.id, CatalogItem.name, CatalogItem.item_code).where`, `sum`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-supplier-broker-map — trade_supplier_broker_map

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:849`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `router.get`, `trade_map.item_supplier_broker_rows`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-last-supplier-autofill — trade_last_supplier_autofill

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:863`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], supplier_id: uuid.UUID=Query(..., description='Supplier to load latest trade header for')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `router.get`, `trade_map.latest_supplier_trade_header_defaults`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-dashboard-snapshot — trade_dashboard_snapshot

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:876`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_compute_trade_dashboard_snapshot_payload`, `_degraded_dashboard_response`, `_get_trade_dashboard_cache`, `_put_dashboard_last_good`, `_snapshot_cache_key`, `_store_trade_dashboard_cache`, `_strip_degraded_snapshot_fields`, `monotonic`, `require_role`, `router.get`, `run_read_budget_bounded`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/home-overview — trade_home_overview

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:904`.
- Request / authorization / tenant contract: `request: Request, business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), compact: bool=Query(False, description='Omit heavy snapshot arrays when true.'), shell_bundle: bool=Query(False, description='When true, attach home_shell (subcategories, suppliers, items) from the same snapshot compute (no extra queries).'), max_span_days: int | None=Query(None, ge=1, le=400, description='When set, reject ranges longer than this (inclusive calendar days).')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Depends`, `HTTPException`, `Query`, `_apply_trade_dashboard_compact`, `_attach_analytics_panel_blocks`, `_compute_trade_dashboard_snapshot_payload`, `_degraded_dashboard_response`, `_get_trade_dashboard_cache`, `_put_dashboard_last_good`, `_snapshot_cache_key`, `_store_trade_dashboard_cache`, `_strip_degraded_snapshot_fields`, `build_home_operational_bundle`, `compute_inventory_summary`, `json_response_with_etag`, `monotonic`, `out.get`, `router.get`, `run_read_budget_bounded`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-summary — trade_purchase_summary

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:996`.
- Request / authorization / tenant contract: `request: Request, business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))], date_from: date | None=Query(None, alias='from'), date_to: date | None=Query(None, alias='to'), supplier_id: uuid.UUID | None=Query(None)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(await execute_with_retry(lambda: db.execute(q_inner))).mappings`, `(await execute_with_retry(lambda: db.execute(q_inner))).mappings().one`, `(await execute_with_retry(lambda: db.execute(roll_q_inner))).mappings`, `(await execute_with_retry(lambda: db.execute(roll_q_inner))).mappings().one`, `Depends`, `Query`, `_degraded_summary_response`, `_get_trade_summary_cache`, `_put_summary_last_good`, `_store_trade_summary_cache`, `_strip_degraded_snapshot_fields`, `and_`, `conditions_inner.append`, `date_from.isoformat`, `date_to.isoformat`, `db.execute`, `execute_with_retry`, `func.coalesce`, `func.coalesce(func.sum(TradePurchaseLine.qty), 0.0).label`, `func.coalesce(func.sum(amt_inner), 0.0).label`, `func.coalesce(func.sum(bag_expr_i), 0.0).label`, `func.coalesce(func.sum(box_expr_i), 0.0).label`, `func.coalesce(func.sum(kg_expr_i), 0.0).label`, `func.coalesce(func.sum(tin_expr_i), 0.0).label`, `func.count`, `func.count(func.distinct(TradePurchase.id)).label`, `func.distinct`, `func.sum`, `json_response_with_etag`, `monotonic`, `require_role`, `router.get`, `run_read_budget_bounded`, `select`, `select(func.coalesce(func.sum(bag_expr_i), 0.0).label('total_bags'), func.coalesce(func.sum(box_expr_i), 0.0).label('total_boxes'), func.coalesce(func.sum(tin_expr_i), 0.0).label('total_tins'), func.coalesce(func.sum(kg_expr_i), 0.0).label('total_kg')).select_from`, `select(func.coalesce(func.sum(bag_expr_i), 0.0).label('total_bags'), func.coalesce(func.sum(box_expr_i), 0.0).label('total_boxes'), func.coalesce(func.sum(tin_expr_i), 0.0).label('total_tins'), func.coalesce(func.sum(kg_expr_i), 0.0).label('total_kg')).select_from(TradePurchaseLine).join`, `select(func.coalesce(func.sum(bag_expr_i), 0.0).label('total_bags'), func.coalesce(func.sum(box_expr_i), 0.0).label('total_boxes'), func.coalesce(func.sum(tin_expr_i), 0.0).label('total_tins'), func.coalesce(func.sum(kg_expr_i), 0.0).label('total_kg')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(func.count(func.distinct(TradePurchase.id)).label('deals'), func.coalesce(func.sum(amt_inner), 0.0).label('total_purchase'), func.coalesce(func.sum(TradePurchaseLine.qty), 0.0).label('total_qty')).select_from`, `select(func.count(func.distinct(TradePurchase.id)).label('deals'), func.coalesce(func.sum(amt_inner), 0.0).label('total_purchase'), func.coalesce(func.sum(TradePurchaseLine.qty), 0.0).label('total_qty')).select_from(TradePurchaseLine).join`, `select(func.count(func.distinct(TradePurchase.id)).label('deals'), func.coalesce(func.sum(amt_inner), 0.0).label('total_purchase'), func.coalesce(func.sum(TradePurchaseLine.qty), 0.0).label('total_qty')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `tq.trade_line_amount_expr`, `tq.trade_line_qty_bags_expr`, `tq.trade_line_qty_boxes_expr`, `tq.trade_line_qty_tins_expr`, `tq.trade_line_weight_expr`, `tq.trade_purchase_status_in_reports`, `trade_read_cache_generation`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-daily-profit — trade_daily_profit_series

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1106`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Depends`, `HTTPException`, `Query`, `_trade_purchase_date_filter`, `d.isoformat`, `db.execute`, `execute_with_retry`, `func.coalesce`, `func.sum`, `r.all`, `router.get`, `select`, `select(TradePurchase.purchase_date, func.coalesce(func.sum(profit_e), 0.0)).select_from`, `select(TradePurchase.purchase_date, func.coalesce(func.sum(profit_e), 0.0)).select_from(TradePurchaseLine).join`, `select(TradePurchase.purchase_date, func.coalesce(func.sum(profit_e), 0.0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where`, `select(TradePurchase.purchase_date, func.coalesce(func.sum(profit_e), 0.0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where(bf).group_by`, `select(TradePurchase.purchase_date, func.coalesce(func.sum(profit_e), 0.0)).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).where(bf).group_by(TradePurchase.purchase_date).order_by`, `tq.trade_line_profit_expr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-items — trade_items_breakdown

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1132`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), category_id: list[uuid.UUID] | None=Query(None), subcategory_id: list[uuid.UUID] | None=Query(None), supplier_id: list[uuid.UUID] | None=Query(None), sort: str=Query('highestValue'), order: str=Query('desc'), limit: int=Query(500, ge=1, le=2000), offset: int=Query(0, ge=0)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_fetch_trade_items_breakdown_rows`, `order.lower`, `r.get`, `router.get`, `rows.sort`, `{'highestValue': lambda r: float(r.get('total_purchase') or r.get('total_amount') or 0), 'highestQty': lambda r: float(r.get('total_qty') or 0), 'az': lambda r: str(r.get('item_name') or r.get('name') or '').lower(), 'latest': lambda r: str(r.get('last_purchase_date') or '')}.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-suppliers — trade_suppliers_breakdown

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1168`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), limit: int=Query(100, ge=1, le=500)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_trade_suppliers_rows`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-categories — trade_categories_breakdown

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1182`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), limit: int=Query(100, ge=1, le=500)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(await db.execute(q)).mappings`, `(await db.execute(q)).mappings().all`, `CatalogItem.deleted_at.is_`, `Depends`, `Query`, `_trade_line_amount_expr`, `_trade_purchase_date_filter`, `and_`, `cat_key.label`, `db.execute`, `func.coalesce`, `func.coalesce(func.sum(amt), 0).desc`, `func.coalesce(func.sum(amt), 0).label`, `func.coalesce(func.sum(profit_e), 0).label`, `func.count`, `func.count(TradePurchaseLine.id).label`, `func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label`, `func.distinct`, `func.sum`, `qty_sum.label`, `router.get`, `select`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from(TradePurchaseLine).join`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).outerjoin`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).outerjoin(CatalogItem, and_(CatalogItem.id == TradePurchaseLine.catalog_item_id, CatalogItem.deleted_at.is_(None))).outerjoin`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).outerjoin(CatalogItem, and_(CatalogItem.id == TradePurchaseLine.catalog_item_id, CatalogItem.deleted_at.is_(None))).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).outerjoin(CatalogItem, and_(CatalogItem.id == TradePurchaseLine.catalog_item_id, CatalogItem.deleted_at.is_(None))).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(bf).group_by`, `select(cat_key.label('category_name'), func.count(TradePurchaseLine.id).label('line_count'), func.count(func.distinct(TradePurchaseLine.catalog_item_id)).label('item_count'), func.coalesce(func.sum(amt), 0).label('total_purchase'), func.coalesce(func.sum(profit_e), 0).label('total_profit'), qty_sum.label('total_qty')).select_from(TradePurchaseLine).join(TradePurchase, TradePurchase.id == TradePurchaseLine.trade_purchase_id).outerjoin(CatalogItem, and_(CatalogItem.id == TradePurchaseLine.catalog_item_id, CatalogItem.deleted_at.is_(None))).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(bf).group_by(cat_key).order_by`, `tq.trade_line_profit_expr`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/trade-types — trade_types_breakdown

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1235`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), limit: int=Query(100, ge=1, le=500)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_fetch_trade_types_breakdown_rows`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/period-comparison — reports_period_comparison

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1295`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_purchase_totals_for_range`, `prior_end.isoformat`, `prior_start.isoformat`, `round`, `router.get`, `timedelta`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/movement-summary — reports_movement_summary

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1324`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), tz_offset_minutes: int=Query(0, ge=-720, le=840)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_utc_range_from_local_dates`, `d.isoformat`, `daily_r.all`, `db.execute`, `func.coalesce`, `func.count`, `func.date`, `func.sum`, `hasattr`, `r.all`, `router.get`, `select`, `select(StockAdjustmentLog.adjustment_type, func.count(), func.coalesce(func.sum(StockAdjustmentLog.new_qty - StockAdjustmentLog.old_qty), 0)).where`, `select(StockAdjustmentLog.adjustment_type, func.count(), func.coalesce(func.sum(StockAdjustmentLog.new_qty - StockAdjustmentLog.old_qty), 0)).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_at >= start_dt, StockAdjustmentLog.updated_at <= end_dt).group_by`, `select(func.date(StockAdjustmentLog.updated_at), func.count()).where`, `select(func.date(StockAdjustmentLog.updated_at), func.count()).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_at >= start_dt, StockAdjustmentLog.updated_at <= end_dt).group_by`, `select(func.date(StockAdjustmentLog.updated_at), func.count()).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_at >= start_dt, StockAdjustmentLog.updated_at <= end_dt).group_by(func.date(StockAdjustmentLog.updated_at)).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/activity-feed — reports_activity_feed

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1375`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), tz_offset_minutes: int=Query(0, ge=-720, le=840), limit: int=Query(50, ge=1, le=200), offset: int=Query(0, ge=0)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(await db.execute(pq)).all`, `Depends`, `Query`, `TradePurchase.purchase_date.desc`, `_trade_purchase_date_filter`, `_utc_range_from_local_dates`, `d.isoformat`, `daily_r.all`, `db.execute`, `func.count`, `func.date`, `hasattr`, `pd.isoformat`, `router.get`, `select`, `select(TradePurchase.id, TradePurchase.human_id, TradePurchase.purchase_date, TradePurchase.total_amount).where`, `select(TradePurchase.id, TradePurchase.human_id, TradePurchase.purchase_date, TradePurchase.total_amount).where(bf).order_by`, `select(TradePurchase.id, TradePurchase.human_id, TradePurchase.purchase_date, TradePurchase.total_amount).where(bf).order_by(TradePurchase.purchase_date.desc()).offset`, `select(TradePurchase.id, TradePurchase.human_id, TradePurchase.purchase_date, TradePurchase.total_amount).where(bf).order_by(TradePurchase.purchase_date.desc()).offset(offset).limit`, `select(func.date(StockAdjustmentLog.updated_at), func.count()).where`, `select(func.date(StockAdjustmentLog.updated_at), func.count()).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_at >= start_dt, StockAdjustmentLog.updated_at <= end_dt).group_by`, `select(func.date(StockAdjustmentLog.updated_at), func.count()).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_at >= start_dt, StockAdjustmentLog.updated_at <= end_dt).group_by(func.date(StockAdjustmentLog.updated_at)).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/reports/item/{catalog_item_id} — reports_item_bundle

- VERIFIED_CODE: `backend/app/routers/reports_trade.py:1466`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, catalog_item_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], date_from: date=Query(..., alias='from'), date_to: date=Query(..., alias='to'), limit: int=Query(50, ge=1, le=200), offset: int=Query(0, ge=0)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_500_INTERNAL_SERVER_ERROR.
- Implementation calls: `Depends`, `HTTPException`, `Query`, `_reports_item_bundle_body`, `logger.exception`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/search — unified_search

- VERIFIED_CODE: `backend/app/routers/search.py:425`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, _m: Annotated[Membership, Depends(require_membership)], db: Annotated[AsyncSession, Depends(get_db)], q: str=Query(..., min_length=1, max_length=200), supplier_id: uuid.UUID | None=Query(None, description='Boost catalog items tied to this supplier: last_supplier_id snapshot and catalog_item_id on trade purchases counted in reports.')`.
- Response: `UnifiedSearchOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Broker.id.in_`, `CatalogItem.deleted_at.is_`, `CatalogItem.hsn_code.isnot`, `CatalogItem.id.in_`, `CatalogItem.item_code.isnot`, `CatalogItem.type_id.isnot`, `Depends`, `Query`, `Supplier.gst_number.isnot`, `Supplier.id.in_`, `UnifiedSearchOut`, `_attach_last_party_names`, `_attach_last_purchase_human_ids`, `_hydrate_catalog_rows`, `_rank_catalog_items_for_query`, `_search_terms`, `_supplier_exists_in_business`, `_supplier_history_catalog_ids`, `and_`, `brr.all`, `catalog_items_has_type_id_column`, `catalog_rows.append`, `catalog_subcategories_out.append`, `ct.name.label`, `db.execute`, `execute_with_retry`, `f'{tname} {cname}'.strip`, `func.lower`, `func.lower(Broker.name).contains`, `func.lower(CatalogItem.hsn_code).contains`, `func.lower(CatalogItem.item_code).contains`, `func.lower(CatalogItem.name).contains`, `func.lower(CategoryType.name).contains`, `func.lower(ItemCategory.name).contains`, `func.lower(Supplier.gst_number).contains`, `func.lower(Supplier.name).contains`, `func.lower(ct.name).contains`, `func.lower(ic.name).contains`, `hr.all`, `hr_b.all`, `ic.name.label`, `ir.all`, `logger.debug`, `logger.exception`, `or_`, `pairs_br.all`, `pairs_r.all`, `q.strip`, `q.strip().lower`, `r.model_dump`, `r_types.all`, `rank_ids_by_token_sort`, `redact_catalog_items`, `redact_trade_purchase_dict`, `router.get`, `rsub.all`, `seen_type.add`, `select`, `select(Broker.id, Broker.name).where`, `select(Broker.id, Broker.name).where(Broker.business_id == business_id).limit`, `select(Broker.id, Broker.name).where(Broker.business_id == business_id, br_match).order_by`, `select(Broker.id, Broker.name).where(Broker.business_id == business_id, br_match).order_by(func.lower(Broker.name)).limit`, `select(CatalogItem.id, CatalogItem.name).where`, `select(CatalogItem.id, CatalogItem.name).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None)).limit`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), *extra_cols).join`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).where`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), item_name_cat_hsn).order_by`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), item_name_cat_hsn).order_by(func.lower(CatalogItem.name)).limit`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), ct.name.label('type_name'), *extra_cols).join`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), ct.name.label('type_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).outerjoin`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), ct.name.label('type_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).outerjoin(ct, ct.id == CatalogItem.type_id).where`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), ct.name.label('type_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).outerjoin(ct, ct.id == CatalogItem.type_id).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), item_name_cat_hsn).order_by`, `select(CatalogItem.id, CatalogItem.name, ic.name.label('category_name'), ct.name.label('type_name'), *extra_cols).join(ic, ic.id == CatalogItem.category_id).outerjoin(ct, ct.id == CatalogItem.type_id).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), item_name_cat_hsn).order_by(func.lower(CatalogItem.name)).limit`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(ItemCategory.business_id == business_id).limit`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(ItemCategory.business_id == business_id, or_(func.lower(CategoryType.name).contains(needle), func.lower(ItemCategory.name).contains(needle))).distinct`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(ItemCategory.business_id == business_id, or_(func.lower(CategoryType.name).contains(needle), func.lower(ItemCategory.name).contains(needle))).distinct().limit`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).select_from`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).select_from(CatalogItem).join`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).select_from(CatalogItem).join(CategoryType, and_(CategoryType.id == CatalogItem.type_id)).join`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).select_from(CatalogItem).join(CategoryType, and_(CategoryType.id == CatalogItem.type_id)).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where`, `select(CategoryType.id, CategoryType.name, ItemCategory.id, ItemCategory.name).select_from(CatalogItem).join(CategoryType, and_(CategoryType.id == CatalogItem.type_id)).join(ItemCategory, ItemCategory.id == CategoryType.category_id).where(CatalogItem.id.in_(ci_ids), CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), ItemCategory.business_id == business_id).distinct`, `select(Supplier.id, Supplier.name).where`, `select(Supplier.id, Supplier.name).where(Supplier.business_id == business_id).limit`, `select(Supplier.id, Supplier.name).where(Supplier.business_id == business_id, sup_match).order_by`, `select(Supplier.id, Supplier.name).where(Supplier.business_id == business_id, sup_match).order_by(func.lower(Supplier.name)).limit`, `set`, `should_redact_financials`, `sr.all`, `tps.list_trade_purchases`, `type_pairs.append`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /audit/feed — audit_feed

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:207`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], limit: int=Query(50, ge=1, le=200), on: date | None=Query(None)`.
- Response: `list[StockAdjustmentOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `recent_adjustments_all`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /audit/recent — recent_adjustments_all

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:217`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], limit: int=Query(5, ge=1, le=250), on: date | None=Query(None, description='Filter to calendar day (UTC) YYYY-MM-DD')`.
- Response: `list[StockAdjustmentOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `fetch_recent_adjustments`, `logger.info`, `monotonic`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /physical-counts/recent — physical_counts_recent

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:282`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], limit: int=Query(50, ge=1, le=250), date_from: date | None=Query(None, alias='from'), date_to: date | None=Query(None, alias='to')`.
- Response: `list[PhysicalStockCountRecentOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.id.in_`, `Depends`, `Query`, `_physical_count_feed_row`, `codes.get`, `datetime.combine`, `db.execute`, `desc`, `ir.all`, `logger.info`, `monotonic`, `names.get`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(CatalogItem.id, CatalogItem.name, CatalogItem.item_code).where`, `select(StockPhysicalCount).where`, `stmt.order_by`, `stmt.order_by(desc(StockPhysicalCount.counted_at)).limit`, `stmt.where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /physical-counts/by-item/{item_id} — physical_counts_for_item

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:339`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], limit: int=Query(50, ge=1, le=200)`.
- Response: `list[PhysicalStockCountRecentOut]`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `Query`, `_physical_count_feed_row`, `db.execute`, `desc`, `getattr`, `ir.scalar_one_or_none`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(CatalogItem).where`, `select(StockPhysicalCount).where`, `select(StockPhysicalCount).where(StockPhysicalCount.business_id == business_id, StockPhysicalCount.item_id == item_id).order_by`, `select(StockPhysicalCount).where(StockPhysicalCount.business_id == business_id, StockPhysicalCount.item_id == item_id).order_by(desc(StockPhysicalCount.counted_at)).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /variances/today — variances_today

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:377`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `list[StockVarianceOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Decimal`, `Depends`, `StockVarianceOut`, `date.fromisoformat`, `datetime.combine`, `datetime.now`, `datetime.now(timezone.utc).strftime`, `db.execute`, `desc`, `p.get`, `r.scalars`, `r.scalars().all`, `router.get`, `rows.append`, `seen.add`, `select`, `select(AppNotification).where`, `select(AppNotification).where(AppNotification.business_id == business_id, AppNotification.kind == 'stock_variance', AppNotification.created_at >= start).order_by`, `select(AppNotification).where(AppNotification.business_id == business_id, AppNotification.kind == 'stock_variance', AppNotification.created_at >= start).order_by(desc(AppNotification.created_at)).limit`, `set`, `uuid.UUID`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /audit/{item_id} — audit_for_item

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:415`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `list[StockAdjustmentOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `_adjustments_to_out`, `db.execute`, `desc`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(StockAdjustmentLog).where`, `select(StockAdjustmentLog).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id == item_id).order_by`, `select(StockAdjustmentLog).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id == item_id).order_by(desc(StockAdjustmentLog.updated_at)).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /staff-purchases — list_staff_purchase_logs

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:510`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], item_id: uuid.UUID | None=Query(None), limit: int=Query(100, ge=1, le=500)`.
- Response: `list[StaffPurchaseLogOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_staff_purchase_out`, `db.execute`, `desc`, `r.scalars`, `r.scalars().all`, `router.get`, `select`, `select(StaffPurchaseLog).where`, `stmt.order_by`, `stmt.order_by(desc(StaffPurchaseLog.created_at)).limit`, `stmt.where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /staff-purchases — create_staff_purchase_log

- VERIFIED_CODE: `backend/app/routers/stock/stock_adjustments.py:523`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: StaffPurchaseLogIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _membership: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StaffPurchaseLogOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Decimal`, `Depends`, `HTTPException`, `StaffPurchaseLog`, `_broker_snapshot`, `_staff_purchase_out`, `_supplier_snapshot`, `apply_stock_movement`, `body.broker_name.strip`, `body.notes.strip`, `body.supplier_name.strip`, `catalog_stock_unit`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `getattr`, `lr.scalar_one_or_none`, `publish_business_event`, `require_permission`, `router.post`, `select`, `select(StaffPurchaseLog).where`, `sh._user_display`, `uuid.uuid4`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /barcode/lookup — barcode_lookup

- VERIFIED_CODE: `backend/app/routers/stock/stock_barcode.py:156`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], code: str=Query(..., min_length=1)`.
- Response: `BarcodeLookupOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `BarcodeLookupOut`, `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `Query`, `_barcode_lookup_cache_get`, `_barcode_lookup_cache_set`, `_latest_purchase_by_item`, `catalog_reorder`, `catalog_stock_qty`, `code.strip`, `db.execute`, `getattr`, `lp_map.get`, `out.model_dump`, `phys_map.get`, `r.scalars`, `r.scalars().all`, `r2.scalars`, `r2.scalars().all`, `router.get`, `select`, `select(CatalogItem).where`, `sh._latest_physical_count_map`, `sh._supplier_names_bulk`, `sup_map.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /barcode/{item_id} — barcode_label

- VERIFIED_CODE: `backend/app/routers/stock/stock_barcode.py:258`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `BarcodeLabelOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `_barcode_label`, `db.execute`, `r.scalar_one_or_none`, `router.get`, `select`, `select(CatalogItem).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /barcode/batch — barcode_batch

- VERIFIED_CODE: `backend/app/routers/stock/stock_barcode.py:347`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: BarcodeBatchIn, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('barcode_print'))]`.
- Response: `BarcodeBatchOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `BarcodeBatchOut`, `CatalogItem.deleted_at.is_`, `CatalogItem.id.in_`, `Depends`, `_barcode_label_from_parts`, `_latest_purchase_by_item`, `cat_map.get`, `db.execute`, `items.get`, `labels.append`, `lp_map.get`, `r.scalars`, `r.scalars().all`, `require_permission`, `router.post`, `select`, `select(CatalogItem).where`, `sh._category_names_bulk`, `sh._supplier_names_bulk`, `sup_map.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /items/{item_id}/purchase-intelligence — get_item_purchase_intelligence

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:130`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `TradePurchase.status.notin_`, `db.execute`, `desc`, `diffs.append`, `max`, `range`, `round`, `router.get`, `rows_r.all`, `select`, `select(Supplier.id, Supplier.name).where`, `select(TradePurchaseLine.qty, TradePurchase.created_at, TradePurchase.supplier_id).join`, `select(TradePurchaseLine.qty, TradePurchase.created_at, TradePurchase.supplier_id).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).where`, `select(TradePurchaseLine.qty, TradePurchase.created_at, TradePurchase.supplier_id).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).where(TradePurchase.business_id == business_id, TradePurchaseLine.catalog_item_id == item_id, TradePurchase.status.notin_(('cancelled', 'deleted'))).order_by`, `select(TradePurchaseLine.qty, TradePurchase.created_at, TradePurchase.supplier_id).join(TradePurchase, TradePurchaseLine.trade_purchase_id == TradePurchase.id).where(TradePurchase.business_id == business_id, TradePurchaseLine.catalog_item_id == item_id, TradePurchase.status.notin_(('cancelled', 'deleted'))).order_by(desc(TradePurchase.created_at)).limit`, `sorted`, `sum`, `supp_r.first`, `supplier_counts.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /{item_id}/activity — stock_item_activity

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:276`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], membership: Annotated[Membership, Depends(require_membership)], limit: int=Query(50, ge=1, le=200), offset: int=Query(0, ge=0, le=5000), kind: str | None=Query(None, description='Comma-separated movement kinds filter (purchase,physical_count,damage,correction,sale,transfer,staff_purchase_log,staff_activity_log).')`.
- Response: `StockItemActivityOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `(kind or '').split`, `Depends`, `Query`, `StockActivityEventOut`, `StockItemActivityOut`, `StockMovement.movement_kind.in_`, `_activity_stock_item_header`, `_membership_role_map`, `_movement_out`, `_staff_activity_event`, `_staff_purchase_out`, `db.execute`, `desc`, `events.append`, `events.sort`, `getattr`, `k.strip`, `literal`, `m.movement_kind.replace`, `m.movement_kind.replace('_', ' ').title`, `movement_q.where`, `movement_r.scalars`, `movement_r.scalars().all`, `purchase_q.where`, `purchase_r.scalars`, `purchase_r.scalars().all`, `role_map.get`, `router.get`, `select`, `select(StaffActivityLog).where`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.item_id == item_id).order_by`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.item_id == item_id).order_by(desc(StaffActivityLog.created_at)).offset`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.item_id == item_id).order_by(desc(StaffActivityLog.created_at)).offset(offset).limit`, `select(StaffPurchaseLog).where`, `select(StaffPurchaseLog).where(StaffPurchaseLog.business_id == business_id, StaffPurchaseLog.item_id == item_id).order_by`, `select(StaffPurchaseLog).where(StaffPurchaseLog.business_id == business_id, StaffPurchaseLog.item_id == item_id).order_by(desc(StaffPurchaseLog.created_at)).offset`, `select(StaffPurchaseLog).where(StaffPurchaseLog.business_id == business_id, StaffPurchaseLog.item_id == item_id).order_by(desc(StaffPurchaseLog.created_at)).offset(offset).limit`, `select(StockMovement).where`, `select(StockMovement).where(StockMovement.business_id == business_id, StockMovement.item_id == item_id).order_by`, `select(StockMovement).where(StockMovement.business_id == business_id, StockMovement.item_id == item_id).order_by(desc(StockMovement.created_at)).offset`, `select(StockMovement).where(StockMovement.business_id == business_id, StockMovement.item_id == item_id).order_by(desc(StockMovement.created_at)).offset(offset).limit`, `staff_q.where`, `staff_r.scalars`, `staff_r.scalars().all`, `{'quick_purchase': 'Purchase quantity added', 'physical_count': 'Physical stock updated', 'delivery_receive': 'Purchase delivered to system', 'damage': 'Damage recorded', 'correction': 'System stock corrected', 'sale': 'Sale adjustment'}.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /{item_id}/intelligence — get_stock_intelligence

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:395`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], period_start: str | None=Query(None), period_end: str | None=Query(None)`.
- Response: `StockIntelligenceOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `Query`, `StockAdjustmentOut.model_validate`, `StockIntelligenceOut`, `abs`, `adj_r.scalars`, `adj_r.scalars().all`, `catalog_reorder`, `catalog_stock_qty`, `catalog_stock_unit`, `db.execute`, `desc`, `getattr`, `hasattr`, `ledger_map.get`, `m.get`, `p.model_copy`, `profile.as_dict`, `profile_from_catalog_item`, `r.one_or_none`, `router.get`, `select`, `select(CatalogItem, ItemCategory.name, CategoryType.name).outerjoin`, `select(CatalogItem, ItemCategory.name, CategoryType.name).outerjoin(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin`, `select(CatalogItem, ItemCategory.name, CategoryType.name).outerjoin(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin(CategoryType, CatalogItem.type_id == CategoryType.id).where`, `select(StockAdjustmentLog).where`, `select(StockAdjustmentLog).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id == item_id).order_by`, `select(StockAdjustmentLog).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id == item_id).order_by(desc(StockAdjustmentLog.updated_at)).limit`, `sh._ledger_variance_map`, `sh._needs_verification`, `sh._parse_period_dates`, `sh._period_purchased_map`, `sh._period_usage_map`, `sh._recent_purchases`, `sh._supplier_name`, `should_redact_financials`, `stock_qty_kg_equivalent`, `stock_status`, `um.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /item/{item_id}/summary — get_stock_item_summary

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:482`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `(await sh._latest_physical_count_map(db, business_id, [item_id])).get`, `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `catalog_stock_qty`, `db.execute`, `getattr`, `phys.counted_at.isoformat`, `r.scalar_one_or_none`, `router.get`, `select`, `select(CatalogItem).where`, `sh._latest_physical_count_map`, `updated_at.isoformat`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /{item_id}/bundle — stock_item_bundle

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:516`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], membership: Annotated[Membership, Depends(require_membership)], period_start: str | None=Query(None), period_end: str | None=Query(None)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `get_catalog_item`, `get_stock_intelligence`, `get_stock_item`, `router.get`, `stock_item_activity`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /{item_id} — get_stock_item

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:565`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], period_start: str | None=Query(None), period_end: str | None=Query(None)`.
- Response: `StockDetailOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `(await sh._latest_physical_count_map(db, business_id, [item_id])).get`, `(await sh._pending_order_meta_map(db, business_id, [item_id])).get`, `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `Query`, `StockDetailOut`, `abs`, `base.model_dump`, `catalog_stock_qty`, `catalog_stock_unit`, `db.execute`, `delivered_map.get`, `getattr`, `hasattr`, `ledger_map.get`, `p.model_copy`, `pending_lifetime_map.get`, `period_map.get`, `r.one_or_none`, `router.get`, `select`, `select(CatalogItem, ItemCategory.name, CategoryType.name).outerjoin`, `select(CatalogItem, ItemCategory.name, CategoryType.name).outerjoin(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin`, `select(CatalogItem, ItemCategory.name, CategoryType.name).outerjoin(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin(CategoryType, CatalogItem.type_id == CategoryType.id).where`, `sh._item_to_list_row`, `sh._last_trade_meta_map`, `sh._latest_physical_count_map`, `sh._ledger_variance_map`, `sh._lifetime_purchase_qty_maps`, `sh._needs_verification`, `sh._parse_period_dates`, `sh._pending_order_meta_map`, `sh._period_purchased_map`, `sh._period_usage_map`, `sh._recent_purchases`, `sh._supplier_name`, `should_redact_financials`, `trade_meta.get`, `usage_map.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/opening-stock — set_opening_stock

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:672`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, body: OpeningStockIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_role('owner', 'super_admin'))]`.
- Response: `StockDetailOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `(body.reason or '').strip`, `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `apply_stock_movement`, `body.notes.strip`, `body.reason.strip`, `datetime.now`, `db.commit`, `db.execute`, `db.refresh`, `get_stock_item`, `getattr`, `publish_business_event`, `r.scalar_one_or_none`, `require_role`, `router.post`, `select`, `select(CatalogItem).where`, `sh._user_display`, `uuid.uuid4`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/physical-count — record_physical_stock_count

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:749`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, body: PhysicalStockCountIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _membership: Annotated[Membership, Depends(require_permission('stock_edit'))], force: bool=Query(False, description='No-op for this route: physical count is observation-only and does not use stock_version optimistic locking.')`.
- Response: `PhysicalStockCountOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `(await sh._period_purchased_map(db, business_id, [item_id], ps, pe)).get`, `(body.idempotency_key or '').strip`, `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `Query`, `StockPhysicalCount`, `_physical_count_out`, `body.notes.strip`, `catalog_stock_qty`, `catalog_stock_unit`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `dup_r.scalar_one_or_none`, `log_staff_activity_best_effort`, `publish_business_event`, `r.scalar_one_or_none`, `require_permission`, `router.post`, `select`, `select(CatalogItem).where`, `select(StockPhysicalCount).where`, `select(StockPhysicalCount).where(StockPhysicalCount.business_id == business_id, StockPhysicalCount.idempotency_key == idem).limit`, `sh._parse_period_dates`, `sh._period_purchased_map`, `sh._user_display`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/physical-update — update_physical_stock

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:854`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, body: StockPhysicalUpdateIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], membership: Annotated[Membership, Depends(require_permission('stock_edit'))], force: bool=Query(False, description='Skip optimistic version check after explicit user warning (staff/owner).')`.
- Response: `StockPhysicalUpdateOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `(await sh._period_purchased_map(db, business_id, [item_id], ps, pe)).get`, `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `Query`, `StockPhysicalCount`, `StockPhysicalUpdateOut`, `_movement_out`, `apply_stock_movement_with_retry`, `body.notes.strip`, `catalog_stock_unit`, `db.add`, `db.commit`, `db.execute`, `db.refresh`, `get_stock_item`, `item_r.scalar_one_or_none`, `maybe_notify_stock_variance`, `publish_business_event`, `require_permission`, `router.post`, `select`, `select(CatalogItem).where`, `sh._parse_period_dates`, `sh._period_purchased_map`, `sh._user_display`, `{'verification': 'physical_count', 'damaged': 'damage', 'correction': 'correction', 'sale': 'sale'}.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/verify-count — verify_stock_count

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:972`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, body: StockVerifyCountIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], membership: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StockDetailOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `(body.idempotency_key or '').strip`, `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `apply_audit_line_to_stock`, `body.reason.strip`, `catalog_stock_qty`, `db.commit`, `db.execute`, `db.refresh`, `dup_r.scalar_one_or_none`, `get_stock_item`, `log_staff_activity_best_effort`, `maybe_notify_stock_variance`, `r.scalar_one_or_none`, `require_permission`, `router.post`, `select`, `select(CatalogItem).options`, `select(CatalogItem).options(selectinload(CatalogItem.category)).where`, `select(StockMovement).where`, `select(StockMovement).where(StockMovement.business_id == business_id, StockMovement.idempotency_key == idem).limit`, `selectinload`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /{item_id} — patch_stock_item

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:1050`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, body: StockPatchIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _membership: Annotated[Membership, Depends(require_permission('stock_edit'))], force: bool=Query(False, description='Skip optimistic version check after explicit user warning (staff/owner).')`.
- Response: `StockDetailOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `(_membership.role or '').strip`, `(_membership.role or '').strip().lower`, `CatalogItem.deleted_at.is_`, `Decimal`, `Depends`, `HTTPException`, `Query`, `apply_stock_movement_with_retry`, `bool`, `catalog_stock_unit`, `db.commit`, `db.execute`, `get_stock_item`, `getattr`, `item_r.scalar_one_or_none`, `maybe_notify_staff_system_stock_edit`, `maybe_notify_stock_variance`, `publish_business_event`, `require_permission`, `router.patch`, `select`, `select(CatalogItem).where`, `sh._user_display`, `{'verification': 'physical_count', 'damaged': 'damage', 'correction': 'correction', 'sale': 'sale'}.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/undo-last — undo_last_stock_change

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:1158`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _membership: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StockDetailOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `apply_stock_movement`, `datetime.now`, `db.commit`, `db.execute`, `desc`, `get_stock_item`, `item_r.scalar_one_or_none`, `publish_business_event`, `r.scalar_one_or_none`, `require_permission`, `router.post`, `select`, `select(CatalogItem).where`, `select(StockAdjustmentLog).where`, `select(StockAdjustmentLog).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id == item_id, StockAdjustmentLog.updated_by == user.id, StockAdjustmentLog.updated_at >= cutoff).order_by`, `select(StockAdjustmentLog).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.item_id == item_id, StockAdjustmentLog.updated_by == user.id, StockAdjustmentLog.updated_at >= cutoff).order_by(desc(StockAdjustmentLog.updated_at)).limit`, `timedelta`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/notify-owner — notify_owner_about_item

- VERIFIED_CODE: `backend/app/routers/stock/stock_detail.py:1238`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], alert: str=Query('reorder', pattern='^(reorder|missing_barcode)$')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `AppNotification`, `AppNotification.dedupe_key.in_`, `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `Membership.role.in_`, `Query`, `catalog_reorder`, `catalog_stock_qty`, `datetime.now`, `datetime.now(timezone.utc).strftime`, `db.add`, `db.commit`, `db.execute`, `er.all`, `mems.all`, `publish_notification_changed`, `r.scalar_one_or_none`, `router.post`, `select`, `select(AppNotification.dedupe_key).where`, `select(CatalogItem).where`, `select(Membership.user_id, Membership.role).where`, `set`, `sh._user_display`, `uuid.uuid4`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /list — list_stock

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:318`.
- Request / authorization / tenant contract: `request: Request, business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=2000), q: str=Query(''), category: str=Query(''), subcategory: str=Query(''), status: StatusFilter=Query('all'), sort: SortBy=Query('name'), include_period: bool=Query(False), period_start: str | None=Query(None), period_end: str | None=Query(None), date_from: str | None=Query(None, description='Alias for period_start (YYYY-MM-DD)'), date_to: str | None=Query(None, description='Alias for period_end (YYYY-MM-DD)'), include_today: bool=Query(True), purchased_in_period: bool=Query(False), missing_barcode: bool=Query(False), missing_item_code: bool=Query(False), reorder_only: bool=Query(False), unit: str=Query(''), include_ledger: bool=Query(False, description='All-time ledger variance (expensive). Enable only when needed.')`.
- Response: `StockListOut`.
- Explicit error branches: status.HTTP_500_INTERNAL_SERVER_ERROR.
- Implementation calls: `Depends`, `HTTPException`, `JSONResponse`, `Query`, `Response`, `_list_stock_page`, `get_cached`, `hashlib.md5`, `hashlib.md5(body).hexdigest`, `json.dumps`, `json.dumps(cached_payload, sort_keys=True, default=str).encode`, `json.dumps(payload, sort_keys=True, default=str).encode`, `logger.exception`, `out.model_dump`, `request.headers.get`, `router.get`, `set_cached`, `stock_list_cache_key`, `stock_list_ttl_s`, `trade_read_cache_generation`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /shell-bundle — stock_shell_bundle

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:427`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=200), q: str=Query(''), category: str=Query(''), subcategory: str=Query(''), status: StatusFilter=Query('all'), sort: SortBy=Query('name'), include_period: bool=Query(False), period_start: str | None=Query(None), period_end: str | None=Query(None), date_from: str | None=Query(None), date_to: str | None=Query(None), include_today: bool=Query(True), purchased_in_period: bool=Query(False), missing_barcode: bool=Query(False), missing_item_code: bool=Query(False), reorder_only: bool=Query(False), unit: str=Query(''), audit_limit: int=Query(12, ge=1, le=50), include_ledger: bool=Query(False, description='Ledger variance is off by default on shell-bundle (list browse).')`.
- Response: `StockShellBundleOut`.
- Explicit error branches: status.HTTP_500_INTERNAL_SERVER_ERROR.
- Implementation calls: `Depends`, `HTTPException`, `Query`, `StockShellBundleOut`, `StockShellBundleOut(list=list_out, status_counts=status_counts, delivery_counts=delivery_counts, audit_recent=audit_recent).model_dump`, `StockShellBundleOut.model_validate`, `_compute_delivery_indicator_counts`, `_list_stock_page`, `compute_stock_alerts_summary`, `fetch_recent_adjustments`, `get_cached`, `logger.exception`, `logger.info`, `monotonic`, `router.get`, `set_cached`, `stock_shell_bundle_cache_key`, `stock_shell_bundle_ttl_s`, `trade_read_cache_generation`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /delivery-indicator-counts — delivery_indicator_counts

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:635`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], q: str=Query(''), category: str=Query(''), subcategory: str=Query(''), status: StatusFilter=Query('all'), sort: SortBy=Query('name'), include_period: bool=Query(False), period_start: str | None=Query(None), period_end: str | None=Query(None), date_from: str | None=Query(None), date_to: str | None=Query(None), missing_barcode: bool=Query(False), missing_item_code: bool=Query(False), reorder_only: bool=Query(False), unit: str=Query('')`.
- Response: `StockDeliveryIndicatorCountsOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_compute_delivery_indicator_counts`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /list/compact — list_stock_compact

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:692`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=2000), q: str=Query(''), category: str=Query(''), subcategory: str=Query(''), status: StatusFilter=Query('all'), sort: SortBy=Query('name'), include_period: bool=Query(False), period_start: str | None=Query(None), period_end: str | None=Query(None), include_today: bool=Query(True), purchased_in_period: bool=Query(False)`.
- Response: `StockListCompactOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `StockListCompactOut`, `_list_stock_page`, `_stock_row_to_minimal`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /search — search_stock

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:733`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=2000), q: str=Query(''), category: str=Query(''), subcategory: str=Query(''), status: StatusFilter=Query('all'), sort: SortBy=Query('name')`.
- Response: `StockListOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `_list_stock_page`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /alerts/summary — stock_alerts_summary

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:759`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `StockAlertsSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `compute_stock_alerts_summary`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /warehouse/alerts-summary — warehouse_alerts_summary

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:836`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `WarehouseAlertsSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `compute_stock_alerts_summary`, `router.get`, `warehouse_alerts_from_stock`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /low-stock/summary — low_stock_operations_summary

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:956`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], q: str=Query(''), category: str=Query(''), subcategory: str=Query(''), period_start: str | None=Query(None), period_end: str | None=Query(None)`.
- Response: `LowStockOpsSummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Decimal`, `Depends`, `LowStockOpsSummaryOut`, `Query`, `_days_between`, `_fetch_low_stock_candidates`, `compute_low_stock_priority`, `it.stock_status.lower`, `item_is_disputed`, `merged.values`, `open_dispute_item_ids`, `rejected_audit_item_ids`, `router.get`, `sh._parse_period_dates`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /low-stock/operations — low_stock_operations

- VERIFIED_CODE: `backend/app/routers/stock/stock_list.py:1041`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=2000), q: str=Query(''), filter: LowStockOpsFilter=Query('all'), category: str=Query(''), subcategory: str=Query(''), supplier_id: uuid.UUID | None=Query(None), sort: LowStockOpsSort=Query('priority'), period_start: str | None=Query(None), period_end: str | None=Query(None)`.
- Response: `LowStockOpsOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `' '.join`, `(x.name or '').lower`, `CatalogItem.deleted_at.is_`, `CatalogItem.id.in_`, `Decimal`, `Depends`, `LowStockOpsOut`, `LowStockOpsSummaryOut`, `Query`, `_days_between`, `_enrich_low_stock_ops_rows`, `_fetch_low_stock_candidates`, `compute_low_stock_priority`, `db.execute`, `it.stock_status.lower`, `item_is_disputed`, `max`, `merged.values`, `min`, `open_dispute_item_ids`, `passing.append`, `passing.sort`, `q.strip`, `q.strip().lower`, `rejected_audit_item_ids`, `router.get`, `select`, `select(CatalogItem.id, CatalogItem.last_supplier_id).where`, `sh._parse_period_dates`, `sum`, `supplier_by_item.get`, `supplier_rows.all`, `usage_vals.sort`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /opening/setup — list_opening_stock_setup

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:265`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=200), q: str=Query(''), status: OpeningSetupStatus=Query('all'), stock_status: StatusFilter=Query('all'), category: str=Query(''), subcategory: str=Query(''), missing_barcode: bool=Query(False), missing_item_code: bool=Query(False), supplier_id: uuid.UUID | None=Query(None), unit: str=Query(''), updated_today: bool=Query(False), updated_by: str=Query('')`.
- Response: `OpeningStockSetupOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `OpeningStockSetupOut`, `Query`, `_opening_setup_item_row`, `_opening_setup_summary`, `_query_opening_setup_items`, `items.append`, `router.get`, `sh._supplier_names_bulk`, `sup_map.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /inventory-summary — stock_inventory_summary

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:315`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], membership: Annotated[Membership, Depends(require_membership)]`.
- Response: `InventorySummaryOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `InventorySummaryOut`, `compute_inventory_summary`, `router.get`, `should_redact_financials`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /totals — stock_totals

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:361`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], period_start: str | None=Query(None), period_end: str | None=Query(None)`.
- Response: `StockTotalsOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `Query`, `StockTotalsOut`, `_stock_totals_purchased_in_period`, `case`, `date.fromisoformat`, `db.execute`, `func.coalesce`, `func.count`, `func.sum`, `hasattr`, `r.one`, `router.get`, `select`, `select(func.count(CatalogItem.id), func.coalesce(func.sum(case((CatalogItem.default_unit == 'bag', CatalogItem.current_stock), else_=0)), 0), func.coalesce(func.sum(case((CatalogItem.default_unit == 'bag', CatalogItem.current_stock * func.coalesce(CatalogItem.default_kg_per_bag, 0)), (CatalogItem.default_unit == 'kg', CatalogItem.current_stock), else_=0)), 0), func.coalesce(func.sum(case((CatalogItem.default_unit == 'box', CatalogItem.current_stock), else_=0)), 0), func.coalesce(func.sum(case((CatalogItem.default_unit == 'tin', CatalogItem.current_stock), else_=0)), 0)).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /reorder — list_reorder_entries

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:441`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], membership: Annotated[Membership, Depends(require_membership)], status: str='pending'`.
- Response: `ReorderListOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `(await db.execute(q)).all`, `(status or 'pending').strip`, `(status or 'pending').strip().lower`, `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `ReorderListEntry.created_at.desc`, `ReorderListEntryOut`, `ReorderListOut`, `catalog_reorder`, `catalog_stock_qty`, `db.execute`, `items.append`, `q.where`, `router.get`, `select`, `select(ReorderListEntry, CatalogItem).join`, `select(ReorderListEntry, CatalogItem).join(CatalogItem, CatalogItem.id == ReorderListEntry.item_id).where`, `select(ReorderListEntry, CatalogItem).join(CatalogItem, CatalogItem.id == ReorderListEntry.item_id).where(ReorderListEntry.business_id == business_id, CatalogItem.deleted_at.is_(None)).order_by`, `sh._recent_purchases`, `sh._supplier_names_bulk`, `should_redact_financials`, `sup_map.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /reorder/{entry_id} — patch_reorder_entry

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:493`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, entry_id: uuid.UUID, body: ReorderListPatchIn, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `ReorderListEntryOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `ReorderListEntryOut`, `catalog_reorder`, `catalog_stock_qty`, `datetime.now`, `db.commit`, `db.execute`, `db.refresh`, `r.first`, `router.patch`, `select`, `select(ReorderListEntry, CatalogItem).join`, `select(ReorderListEntry, CatalogItem).join(CatalogItem, CatalogItem.id == ReorderListEntry.item_id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /reorder/{entry_id} — delete_reorder_entry

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:530`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, entry_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `db.commit`, `db.delete`, `db.execute`, `r.scalar_one_or_none`, `router.delete`, `select`, `select(ReorderListEntry).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /opening/missing — missing_opening_stock

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:548`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], limit: int=Query(100, ge=1, le=500)`.
- Response: `OpeningStockMissingOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `CatalogItem.name.asc`, `CatalogItem.opening_stock_set_at.is_`, `Depends`, `OpeningStockMissingOut`, `Query`, `count_r.scalar_one`, `db.execute`, `func.count`, `r.all`, `router.get`, `select`, `select(CatalogItem, ItemCategory.name, CategoryType.name).join`, `select(CatalogItem, ItemCategory.name, CategoryType.name).join(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin`, `select(CatalogItem, ItemCategory.name, CategoryType.name).join(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin(CategoryType, CatalogItem.type_id == CategoryType.id).where`, `select(CatalogItem, ItemCategory.name, CategoryType.name).join(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin(CategoryType, CatalogItem.type_id == CategoryType.id).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), CatalogItem.opening_stock_set_at.is_(None)).order_by`, `select(CatalogItem, ItemCategory.name, CategoryType.name).join(ItemCategory, CatalogItem.category_id == ItemCategory.id).outerjoin(CategoryType, CatalogItem.type_id == CategoryType.id).where(CatalogItem.business_id == business_id, CatalogItem.deleted_at.is_(None), CatalogItem.opening_stock_set_at.is_(None)).order_by(CatalogItem.name.asc()).limit`, `select(func.count(CatalogItem.id)).where`, `sh._item_to_list_row`, `sh._supplier_names_bulk`, `sup_map.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/quick-purchase — create_item_quick_purchase

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:585`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, body: QuickPurchaseIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], membership: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `QuickPurchaseOut`.
- Explicit error branches: status.HTTP_500_INTERNAL_SERVER_ERROR.
- Implementation calls: `Depends`, `HTTPException`, `QuickPurchaseOut`, `StaffPurchaseLogIn`, `_movement_out`, `create_staff_purchase_log`, `db.execute`, `get_stock_item`, `movement_r.scalar_one_or_none`, `require_permission`, `router.post`, `select`, `select(StockMovement).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /{item_id}/reorder — add_item_to_reorder_list

- VERIFIED_CODE: `backend/app/routers/stock/stock_ops.py:615`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, item_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `CatalogItem.deleted_at.is_`, `Depends`, `HTTPException`, `ReorderListEntry`, `datetime.now`, `db.add`, `db.commit`, `db.execute`, `ex.scalar_one_or_none`, `r.scalar_one_or_none`, `router.post`, `select`, `select(CatalogItem).where`, `select(ReorderListEntry).where`, `select(ReorderListEntry).where(ReorderListEntry.business_id == business_id, ReorderListEntry.item_id == item_id, ReorderListEntry.status == 'pending').limit`, `sh._user_display`, `uuid.uuid4`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/stock-audits — create_stock_audit

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:46`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_in: StockAuditCreate, db: Annotated[AsyncSession, Depends(get_db)], current_user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StockAuditOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `StockAudit`, `_audit_to_out`, `date.today`, `db.add`, `db.commit`, `db.flush`, `get_audit_for_business`, `require_permission`, `router.post`, `upsert_audit_line`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/stock-audits/active — get_active_stock_audit

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:81`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], current_user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `StockAuditOut | None`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `StockAudit.created_at.desc`, `StockAudit.status.in_`, `_audit_to_out`, `db.execute`, `r.scalar_one_or_none`, `router.get`, `select`, `select(StockAudit).where`, `select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.auditor_id == current_user.id, StockAudit.status.in_(('draft', 'pending_review'))).order_by`, `select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.auditor_id == current_user.id, StockAudit.status.in_(('draft', 'pending_review'))).order_by(StockAudit.created_at.desc()).limit`, `select(StockAudit).where(StockAudit.business_id == business_id, StockAudit.auditor_id == current_user.id, StockAudit.status.in_(('draft', 'pending_review'))).order_by(StockAudit.created_at.desc()).limit(1).options`, `selectinload`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/stock-audits — list_stock_audits

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:103`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], skip: int=0, limit: int=100`.
- Response: `list[StockAuditOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `StockAudit.created_at.desc`, `_audit_to_out`, `db.execute`, `result.scalars`, `result.scalars().all`, `router.get`, `select`, `select(StockAudit).where`, `select(StockAudit).where(StockAudit.business_id == business_id).order_by`, `select(StockAudit).where(StockAudit.business_id == business_id).order_by(StockAudit.created_at.desc()).offset`, `select(StockAudit).where(StockAudit.business_id == business_id).order_by(StockAudit.created_at.desc()).offset(skip).limit`, `select(StockAudit).where(StockAudit.business_id == business_id).order_by(StockAudit.created_at.desc()).offset(skip).limit(limit).options`, `selectinload`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/stock-audits/kpis — stock_audit_kpis

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:122`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `StockAuditKpisOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `StockAuditKpisOut`, `date.today`, `db.scalar`, `func.count`, `router.get`, `select`, `select(func.count(StockAudit.id)).where`, `select(func.count(StockAuditItem.id)).join`, `select(func.count(StockAuditItem.id)).join(StockAudit, StockAudit.id == StockAuditItem.audit_id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/stock-audits/pending-lines — list_pending_audit_lines_for_item

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:168`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], item_id: uuid.UUID`.
- Response: `list[StockAuditPendingLineOut]`.
- Explicit error branches: status.HTTP_403_FORBIDDEN.
- Implementation calls: `Depends`, `HTTPException`, `StockAudit.created_at.desc`, `StockAuditItem.id.desc`, `StockAuditPendingLineOut`, `db.execute`, `out.append`, `r.all`, `router.get`, `select`, `select(StockAuditItem, StockAudit.audit_date).join`, `select(StockAuditItem, StockAudit.audit_date).join(StockAudit, StockAudit.id == StockAuditItem.audit_id).where`, `select(StockAuditItem, StockAudit.audit_date).join(StockAudit, StockAudit.id == StockAuditItem.audit_id).where(StockAudit.business_id == business_id, StockAuditItem.item_id == item_id, StockAuditItem.line_status == 'pending_approval').order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/stock-audits/{audit_id} — get_stock_audit

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:213`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `StockAuditOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `_audit_to_out`, `get_audit_for_business`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PUT /v1/businesses/{business_id}/stock-audits/{audit_id} — update_stock_audit

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:223`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_id: uuid.UUID, audit_in: StockAuditUpdate, db: Annotated[AsyncSession, Depends(get_db)], current_user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StockAuditOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `Depends`, `HTTPException`, `_audit_to_out`, `any`, `db.commit`, `db.flush`, `db_audit.items.clear`, `get_audit_for_business`, `require_permission`, `router.put`, `upsert_audit_line`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/stock-audits/{audit_id}/lines — upsert_stock_audit_line

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:273`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_id: uuid.UUID, body: StockAuditLineUpsert, db: Annotated[AsyncSession, Depends(get_db)], current_user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StockAuditOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `_audit_to_out`, `db.commit`, `get_audit_for_business`, `require_permission`, `router.post`, `upsert_audit_line`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/stock-audits/{audit_id}/complete — complete_stock_audit_endpoint

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:299`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], current_user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `StockAuditOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `_audit_to_out`, `complete_stock_audit`, `db.commit`, `get_audit_for_business`, `require_permission`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/stock-audits/{audit_id}/lines/{line_id}/approve — approve_stock_audit_line

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:315`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_id: uuid.UUID, line_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], current_user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `StockAuditOut`.
- Explicit error branches: status.HTTP_403_FORBIDDEN.
- Implementation calls: `Depends`, `HTTPException`, `_audit_to_out`, `approve_audit_line`, `db.commit`, `get_audit_for_business`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/stock-audits/{audit_id} — delete_stock_audit

- VERIFIED_CODE: `backend/app/routers/stock_audits.py:334`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, audit_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `Depends`, `HTTPException`, `db.commit`, `db.delete`, `get_audit_for_business`, `require_permission`, `router.delete`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/draft — read_trade_draft

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:235`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `TradeDraftOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `execute_with_retry`, `router.get`, `tps.get_draft`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PUT /v1/businesses/{business_id}/trade-purchases/draft — upsert_trade_draft

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:248`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: TradeDraftUpsertRequest, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `TradeDraftOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `router.put`, `tps.upsert_draft`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/trade-purchases/draft — delete_trade_draft

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:259`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_204_NO_CONTENT.
- Implementation calls: `Depends`, `Response`, `router.delete`, `tps.delete_draft`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/preview-lines — preview_trade_purchase_lines

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:270`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], body: dict[str, Any]=Body(...)`.
- Response: `TradePurchasePreviewOut`.
- Explicit error branches: status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Body`, `Depends`, `HTTPException`, `build_trade_purchase_preview`, `coerce_raw_to_trade_purchase_create`, `router.post`, `tps.collect_trade_purchase_preview_errors`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/validate — validate_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:286`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], body: dict[str, Any]=Body(...)`.
- Response: `TradePurchaseValidateOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Body`, `Depends`, `build_trade_purchase_validate`, `coerce_raw_to_trade_purchase_create`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/check-duplicate — check_trade_duplicate

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:299`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: TradeDuplicateCheckRequest, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `TradeDuplicateCheckResponse`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `router.post`, `tps.check_duplicate`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/next-human-id — next_trade_human_id

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:311`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `TradeNextHumanIdOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `TradeNextHumanIdOut`, `execute_with_retry`, `router.get`, `tps.next_human_id`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases — list_trade_purchases

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:323`.
- Request / authorization / tenant contract: `request: Request, business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], include_lines: bool=Query(False, description='When false (default), omit line payloads for lighter list responses.'), limit: int=Query(20, ge=1, le=2000), offset: int=Query(0, ge=0, le=10000), status: str | None=Query(None, description="Payment: draft|pending|due_soon|overdue|paid; delivery: dispatched|arrived|staff_verifying|stock_committed|…; omit or 'all' = no filter"), q: str | None=Query(None, max_length=200), supplier_id: uuid.UUID | None=Query(None), broker_id: uuid.UUID | None=Query(None), catalog_item_id: uuid.UUID | None=Query(None, description='Only purchases that include a line for this catalog item'), purchase_from: date | None=Query(None, description='Inclusive lower bound on purchase_date (calendar date)'), purchase_to: date | None=Query(None, description='Inclusive upper bound on purchase_date (calendar date)')`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_500_INTERNAL_SERVER_ERROR, status.HTTP_503_SERVICE_UNAVAILABLE.
- Implementation calls: `(q or '').strip`, `Depends`, `HTTPException`, `Query`, `_log.debug`, `_log.exception`, `_log.warning`, `_normalize_trade_list_status`, `_purchase_list_from_cache`, `_purchase_list_response`, `_purchase_list_to_cache`, `execute_with_retry`, `get_cached`, `is_sa_infrastructure_failure`, `max`, `min`, `purchase_from.isoformat`, `purchase_list_cache_key`, `purchase_list_ttl_s`, `purchase_to.isoformat`, `request.headers.get`, `request.headers.get('x-request-id', '').strip`, `router.get`, `set_cached`, `tps.list_trade_purchases`, `trade_read_cache_generation`, `type`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/last-defaults — last_trade_purchase_defaults

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:454`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)], catalog_item_id: uuid.UUID=Query(...), supplier_id: uuid.UUID | None=Query(None), broker_id: uuid.UUID | None=Query(None)`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `execute_with_retry`, `router.get`, `tps.last_purchase_defaults`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases — create_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:476`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: TradePurchaseCreateRequest, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('purchase_create'))], idempotency_key: str | None=Header(None, alias='Idempotency-Key')`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_409_CONFLICT, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `(idempotency_key or '').strip`, `Depends`, `HTTPException`, `Header`, `_catalog_item_ids_from_create`, `_publish_purchase_changed`, `require_permission`, `router.post`, `tps.create_trade_purchase`, `tps.get_trade_purchase`, `tps.lookup_idempotency_purchase_id`, `tps.remember_idempotency_purchase_id`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/payment — patch_trade_purchase_payment

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:524`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: TradePurchasePaymentPatch, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('purchase_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Depends`, `HTTPException`, `_publish_purchase_changed`, `require_permission`, `router.patch`, `tps.patch_trade_purchase_payment`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/delivery-pipeline — get_trade_purchase_delivery_pipeline

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:546`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `TradePurchaseDeliveryPipelineOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `router.get`, `tps.get_trade_purchase_delivery_pipeline`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/trade-purchases/{purchase_id}/delivery — patch_trade_purchase_delivery

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:556`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: TradePurchaseDeliveryPatch, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_with_event`, `require_permission`, `router.patch`, `tps.patch_trade_purchase_delivery`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/dispatch — dispatch_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:577`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: TradePurchaseDispatchIn, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_with_event`, `require_role`, `router.post`, `tps.dispatch_trade_purchase`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/arrive — arrive_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:597`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: TradePurchaseArriveIn, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_with_event`, `require_permission`, `router.post`, `tps.arrive_trade_purchase`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/commit-stock — commit_trade_purchase_delivery

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:617`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'admin', 'super_admin'))], _perm: Annotated[None, Depends(require_permission('stock_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_with_event`, `_stock_version_conflict_http`, `require_permission`, `require_role`, `router.post`, `tps.commit_trade_purchase_delivery`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/auto-commit — auto_commit_trade_purchase_delivery

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:649`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'admin', 'super_admin'))], _perm: Annotated[None, Depends(require_permission('stock_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_with_event`, `require_permission`, `require_role`, `router.post`, `tps.try_auto_commit_trade_purchase_delivery`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/verify — verify_trade_purchase_delivery

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:669`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: TradePurchaseVerifyIn, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_with_event`, `_stock_version_conflict_http`, `require_permission`, `router.post`, `tps.verify_trade_purchase_delivery`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/mark-paid — mark_trade_purchase_paid

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:695`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('purchase_edit'))], body: TradeMarkPaidRequest=TradeMarkPaidRequest()`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `TradeMarkPaidRequest`, `_publish_purchase_changed`, `require_permission`, `router.post`, `tps.mark_trade_purchase_paid`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/cancel — cancel_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:715`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('purchase_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_publish_purchase_changed`, `require_permission`, `router.post`, `tps.cancel_trade_purchase`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PUT /v1/businesses/{business_id}/trade-purchases/{purchase_id} — update_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:734`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: TradePurchaseUpdateRequest, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('purchase_edit'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT, status.HTTP_422_UNPROCESSABLE_ENTITY.
- Implementation calls: `Depends`, `HTTPException`, `_catalog_item_ids_from_update`, `_publish_purchase_changed`, `require_permission`, `router.put`, `tps.update_trade_purchase`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/trade-purchases/{purchase_id} — delete_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:778`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_204_NO_CONTENT, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `Response`, `require_role`, `router.delete`, `tps.delete_trade_purchase`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/{purchase_id} — get_trade_purchase

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:793`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_purchase_detail_response`, `execute_with_retry`, `router.get`, `tps.get_trade_purchase`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports — create_purchase_damage_report

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:813`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: PurchaseDamageReportIn, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_permission('stock_edit'))]`.
- Response: `PurchaseDamageReportOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `(user.name or user.email or '').strip`, `Depends`, `HTTPException`, `PurchaseDamageReportOut`, `pds.create_damage_report`, `pds.damage_report_to_out`, `require_permission`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/damage-reports — list_purchase_damage_reports

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:853`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `list[PurchaseDamageReportOut]`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `PurchaseDamageReportOut`, `out.append`, `pds.damage_report_to_out`, `pds.list_damage_reports`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle-events — list_purchase_lifecycle_events

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:877`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `list[PurchaseLifecycleEventOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `router.get`, `tps.list_purchase_lifecycle_events`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/trade-purchases/{purchase_id}/lifecycle — transition_purchase_lifecycle

- VERIFIED_CODE: `backend/app/routers/trade_purchases.py:888`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, purchase_id: uuid.UUID, body: PurchaseLifecycleTransitionIn, user: Annotated[User, Depends(get_current_user)], db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))]`.
- Response: `TradePurchaseOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_publish_purchase_changed`, `_purchase_detail_response`, `require_role`, `router.post`, `tps.transition_purchase_lifecycle`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/users — create_user

- VERIFIED_CODE: `backend/app/routers/users.py:300`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: UserCreateIn, db: Annotated[AsyncSession, Depends(get_db)], actor: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))], current_user: Annotated[User, Depends(get_current_user)]`.
- Response: `UserCreateOut`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_403_FORBIDDEN, status.HTTP_409_CONFLICT.
- Implementation calls: `Depends`, `HTTPException`, `Membership`, `User`, `UserCreateOut`, `_active_user_filter`, `_phone_digits`, `_user_row`, `allocate_username`, `body.full_name.strip`, `body.notes.strip`, `body.password.strip`, `body.phone.strip`, `db.add`, `db.commit`, `db.execute`, `db.flush`, `db.refresh`, `effective_permissions`, `ex.first`, `generate_readable_password`, `hash_password`, `log_user_lifecycle`, `require_role`, `router.post`, `select`, `select(User.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users — list_users

- VERIFIED_CODE: `backend/app/routers/users.py:373`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'manager', 'super_admin'))], include_inactive: bool=Query(False)`.
- Response: `list[UserListOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `User.is_active.is_`, `_active_user_filter`, `_list_user_rows`, `clauses.append`, `db.execute`, `r.all`, `require_role`, `router.get`, `select`, `select(User, Membership).join`, `select(User, Membership).join(Membership, Membership.user_id == User.id).where`, `select(User, Membership).join(Membership, Membership.user_id == User.id).where(*clauses).order_by`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/active-sessions — active_sessions

- VERIFIED_CODE: `backend/app/routers/users.py:392`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))]`.
- Response: `list[UserListOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `User.is_active.is_`, `User.last_active_at.isnot`, `_active_user_filter`, `_list_user_rows`, `datetime.now`, `db.execute`, `r.all`, `require_role`, `router.get`, `select`, `select(User, Membership).join`, `select(User, Membership).join(Membership, Membership.user_id == User.id).where`, `timedelta`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/users/bulk — bulk_users

- VERIFIED_CODE: `backend/app/routers/users.py:429`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: UserBulkIn, db: Annotated[AsyncSession, Depends(get_db)], actor: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))], current_user: Annotated[User, Depends(get_current_user)]`.
- Response: `UserBulkOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `ROLE_DEFAULTS.get`, `UserBulkOut`, `_guard_actor_target`, `_load_user_membership`, `_revoke_user_tokens`, `datetime.now`, `db.commit`, `failed.append`, `log_user_lifecycle`, `require_role`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id} — get_user

- VERIFIED_CODE: `backend/app/routers/users.py:499`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'manager', 'super_admin'))]`.
- Response: `UserProfileOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_load_user_membership`, `_user_row`, `require_role`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/users/{user_id} — patch_user

- VERIFIED_CODE: `backend/app/routers/users.py:513`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, body: UserPatchIn, db: Annotated[AsyncSession, Depends(get_db)], actor: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))], current_user: Annotated[User, Depends(get_current_user)]`.
- Response: `UserListOut`.
- Explicit error branches: status.HTTP_403_FORBIDDEN, status.HTTP_404_NOT_FOUND, status.HTTP_409_CONFLICT.
- Implementation calls: `Depends`, `HTTPException`, `ROLE_DEFAULTS.get`, `_active_user_filter`, `_guard_actor_target`, `_load_user_membership`, `_revoke_user_tokens`, `_user_row`, `body.full_name.strip`, `body.notes.strip`, `body.phone.strip`, `db.commit`, `db.execute`, `db.refresh`, `ex.first`, `log_user_lifecycle`, `require_role`, `router.patch`, `select`, `select(User.id).where`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### DELETE /v1/businesses/{business_id}/users/{user_id} — delete_user

- VERIFIED_CODE: `backend/app/routers/users.py:574`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], actor: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))], current_user: Annotated[User, Depends(get_current_user)]`.
- Response: `untyped response; inspect handler`.
- Explicit error branches: status.HTTP_400_BAD_REQUEST, status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_guard_actor_target`, `_load_user_membership`, `_revoke_user_tokens`, `datetime.now`, `db.commit`, `log_user_lifecycle`, `require_role`, `router.delete`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/users/{user_id}/reset-password — reset_password

- VERIFIED_CODE: `backend/app/routers/users.py:602`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], actor: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))], current_user: Annotated[User, Depends(get_current_user)]`.
- Response: `ResetPasswordOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `ResetPasswordOut`, `_guard_actor_target`, `_load_user_membership`, `_revoke_user_tokens`, `db.commit`, `generate_readable_password`, `hash_password`, `log_password_reset`, `require_role`, `router.post`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id}/credentials — user_credentials

- VERIFIED_CODE: `backend/app/routers/users.py:625`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))]`.
- Response: `dict`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `_load_user_membership`, `require_role`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id}/created-items — user_created_items

- VERIFIED_CODE: `backend/app/routers/users.py:644`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'manager', 'super_admin'))], limit: int=Query(50, ge=1, le=200)`.
- Response: `list[CreatedItemOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `CatalogItem.deleted_at.is_`, `CreatedItemOut`, `Depends`, `Query`, `db.execute`, `desc`, `out.append`, `r.all`, `require_role`, `router.get`, `select`, `select(CatalogItem, ItemCategory.name).outerjoin`, `select(CatalogItem, ItemCategory.name).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where`, `select(CatalogItem, ItemCategory.name).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(CatalogItem.business_id == business_id, CatalogItem.created_by_user_id == user_id, CatalogItem.deleted_at.is_(None)).order_by`, `select(CatalogItem, ItemCategory.name).outerjoin(ItemCategory, ItemCategory.id == CatalogItem.category_id).where(CatalogItem.business_id == business_id, CatalogItem.created_by_user_id == user_id, CatalogItem.deleted_at.is_(None)).order_by(desc(CatalogItem.created_at)).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id}/stock-adjustments — user_stock_adjustments

- VERIFIED_CODE: `backend/app/routers/users.py:678`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'manager', 'super_admin'))], limit: int=Query(50, ge=1, le=200)`.
- Response: `list[StockAdjustmentOut]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `StockAdjustmentOut`, `db.execute`, `desc`, `out.append`, `r.all`, `require_role`, `router.get`, `select`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin(CatalogItem, CatalogItem.id == StockAdjustmentLog.item_id).where`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin(CatalogItem, CatalogItem.id == StockAdjustmentLog.item_id).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_by == user_id).order_by`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin(CatalogItem, CatalogItem.id == StockAdjustmentLog.item_id).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_by == user_id).order_by(desc(StockAdjustmentLog.updated_at)).limit`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id}/purchases — user_purchases

- VERIFIED_CODE: `backend/app/routers/users.py:713`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'manager', 'super_admin'))], limit: int=Query(50, ge=1, le=100)`.
- Response: `list[UserPurchaseBrief]`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `Query`, `UserPurchaseBrief`, `datetime.combine`, `datetime.min.time`, `db.execute`, `desc`, `func.count`, `line_count_col.label`, `out.append`, `r.all`, `require_role`, `router.get`, `select`, `select(TradePurchase, Supplier.name, line_count_col.label('item_count')).outerjoin`, `select(TradePurchase, Supplier.name, line_count_col.label('item_count')).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).where`, `select(TradePurchase, Supplier.name, line_count_col.label('item_count')).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).where(TradePurchase.business_id == business_id, TradePurchase.user_id == user_id).order_by`, `select(TradePurchase, Supplier.name, line_count_col.label('item_count')).outerjoin(Supplier, Supplier.id == TradePurchase.supplier_id).where(TradePurchase.business_id == business_id, TradePurchase.user_id == user_id).order_by(desc(TradePurchase.created_at)).limit`, `select(func.count(TradePurchaseLine.id)).where`, `select(func.count(TradePurchaseLine.id)).where(TradePurchaseLine.trade_purchase_id == TradePurchase.id).correlate`, `select(func.count(TradePurchaseLine.id)).where(TradePurchaseLine.trade_purchase_id == TradePurchase.id).correlate(TradePurchase).scalar_subquery`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id}/ledger — user_ledger

- VERIFIED_CODE: `backend/app/routers/users.py:761`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'manager', 'super_admin'))], limit: int=Query(80, ge=1, le=200), grouped: bool=Query(False)`.
- Response: `list[LedgerEntryOut] | LedgerGroupedOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `Depends`, `LedgerEntryOut`, `LedgerGroupedOut`, `Query`, `act.scalars`, `act.scalars().all`, `datetime.now`, `db.execute`, `desc`, `entries.append`, `entries.sort`, `now.replace`, `out.this_week.append`, `out.today.append`, `out.yesterday.append`, `require_role`, `router.get`, `select`, `select(StaffActivityLog).where`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.user_id == user_id).order_by`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.user_id == user_id).order_by(desc(StaffActivityLog.created_at)).limit`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin(CatalogItem, CatalogItem.id == StockAdjustmentLog.item_id).where`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin(CatalogItem, CatalogItem.id == StockAdjustmentLog.item_id).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_by == user_id).order_by`, `select(StockAdjustmentLog, CatalogItem.name).outerjoin(CatalogItem, CatalogItem.id == StockAdjustmentLog.item_id).where(StockAdjustmentLog.business_id == business_id, StockAdjustmentLog.updated_by == user_id).order_by(desc(StockAdjustmentLog.updated_at)).limit`, `stock.all`, `timedelta`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/users/{user_id}/permissions — get_permissions

- VERIFIED_CODE: `backend/app/routers/users.py:833`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], _m: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))]`.
- Response: `PermissionsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `PermissionsOut`, `_load_user_membership`, `membership_permissions`, `require_role`, `router.get`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### PATCH /v1/businesses/{business_id}/users/{user_id}/permissions — patch_permissions

- VERIFIED_CODE: `backend/app/routers/users.py:848`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, user_id: uuid.UUID, body: PermissionsPatchIn, db: Annotated[AsyncSession, Depends(get_db)], actor: Annotated[Membership, Depends(require_role('owner', 'admin', 'super_admin'))], current_user: Annotated[User, Depends(get_current_user)]`.
- Response: `PermissionsOut`.
- Explicit error branches: status.HTTP_404_NOT_FOUND.
- Implementation calls: `Depends`, `HTTPException`, `PermissionsOut`, `_guard_actor_target`, `_load_user_membership`, `bool`, `db.commit`, `membership_permissions`, `require_role`, `router.patch`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### POST /v1/businesses/{business_id}/activity-log — post_activity

- VERIFIED_CODE: `backend/app/routers/users.py:876`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, body: ActivityLogIn, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)]`.
- Response: `ActivityLogOut`.
- Explicit error branches: None declared in handler; dependency/global/service errors apply.
- Implementation calls: `ActivityLogOut.model_validate`, `Depends`, `StaffActivityLog`, `activity_router.post`, `datetime.now`, `db.add`, `db.commit`, `db.refresh`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

### GET /v1/businesses/{business_id}/activity-log — list_activity

- VERIFIED_CODE: `backend/app/routers/users.py:901`.
- Request / authorization / tenant contract: `business_id: uuid.UUID, db: Annotated[AsyncSession, Depends(get_db)], user: Annotated[User, Depends(get_current_user)], _m: Annotated[Membership, Depends(require_membership)], user_id: uuid.UUID | None=None, period: str=Query('today'), days: int | None=Query(None, ge=1, le=90), page: int=Query(1, ge=1), per_page: int=Query(50, ge=1, le=200)`.
- Response: `list[ActivityLogOut]`.
- Explicit error branches: status.HTTP_403_FORBIDDEN.
- Implementation calls: `(_m.role or '').strip`, `(_m.role or '').strip().lower`, `ActivityLogOut.model_validate`, `Depends`, `HTTPException`, `Query`, `activity_router.get`, `datetime.now`, `db.execute`, `desc`, `now.replace`, `r.scalars`, `r.scalars().all`, `select`, `select(StaffActivityLog).where`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.user_id == uid, StaffActivityLog.created_at >= start).order_by`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.user_id == uid, StaffActivityLog.created_at >= start).order_by(desc(StaffActivityLog.created_at)).offset`, `select(StaffActivityLog).where(StaffActivityLog.business_id == business_id, StaffActivityLog.user_id == uid, StaffActivityLog.created_at >= start).order_by(desc(StaffActivityLog.created_at)).offset((page - 1) * per_page).limit`, `timedelta`.
- Runtime / test result: UNKNOWN/BLOCKED (reference not launched).

## Database model catalogue

| Model | Tables | Source |
|---|---|---|
| AdminAuditLog | admin_audit_logs | `backend/app/models/admin_audit_log.py:12` |
| ApiUsageLog | api_usage_logs | `backend/app/models/api_usage_log.py:12` |
| Base | inherited / no direct table | `backend/app/models/base.py:4` |
| Business | businesses | `backend/app/models/business.py:10` |
| BusinessGoal | business_goals | `backend/app/models/business_goal.py:16` |
| ItemCategory | item_categories | `backend/app/models/catalog.py:15` |
| CategoryType | category_types | `backend/app/models/catalog.py:30` |
| CatalogItem | catalog_items | `backend/app/models/catalog.py:47` |
| CatalogItemDefaultSupplier | catalog_item_default_suppliers | `backend/app/models/catalog.py:153` |
| CatalogItemDefaultBroker | catalog_item_default_brokers | `backend/app/models/catalog.py:172` |
| CatalogVariant | catalog_variants | `backend/app/models/catalog.py:191` |
| Broker | brokers | `backend/app/models/contacts.py:15` |
| Supplier | suppliers | `backend/app/models/contacts.py:38` |
| Membership | memberships | `backend/app/models/membership.py:12` |
| AppNotification | notifications | `backend/app/models/notification.py:16` |
| DailyUsageLog | daily_usage_logs | `backend/app/models/operations.py:15` |
| StaffChecklistTemplate | staff_checklist_templates | `backend/app/models/operations.py:40` |
| StaffChecklistCompletion | staff_checklist_completions | `backend/app/models/operations.py:56` |
| BackupLog | backup_logs | `backend/app/models/owner_ops.py:29` |
| ProviderCredential | provider_credentials | `backend/app/models/owner_ops.py:50` |
| StaffTask | staff_tasks | `backend/app/models/owner_ops.py:70` |
| AiUsageLog | ai_usage_logs | `backend/app/models/owner_ops.py:103` |
| WhatsAppDeliveryLog | whatsapp_delivery_logs | `backend/app/models/owner_ops.py:128` |
| PasswordResetToken | password_reset_tokens | `backend/app/models/password_reset.py:21` |
| PurchaseDamageReport | purchase_damage_reports | `backend/app/models/purchase_damage_report.py:15` |
| PurchaseLifecycleEvent | purchase_lifecycle_events | `backend/app/models/purchase_lifecycle_event.py:16` |
| ReorderListEntry | reorder_list | `backend/app/models/reorder_list.py:11` |
| ReportSavedView | report_saved_views | `backend/app/models/report_saved_view.py:15` |
| StaffPurchaseLog | staff_purchase_logs | `backend/app/models/staff_purchase_log.py:15` |
| StockAdjustmentLog | stock_adjustment_log | `backend/app/models/stock_adjustment.py:16` |
| StockAudit | stock_audits | `backend/app/models/stock_audit.py:11` |
| StockAuditItem | stock_audit_items | `backend/app/models/stock_audit.py:43` |
| StockDisputeCase | stock_dispute_cases | `backend/app/models/stock_dispute_case.py:15` |
| StockMovement | stock_movements | `backend/app/models/stock_movement.py:15` |
| StockPhysicalCount | stock_physical_counts | `backend/app/models/stock_physical_count.py:15` |
| SupplierItemDefault | supplier_item_defaults | `backend/app/models/supplier_item_default.py:17` |
| BrokerSupplierLink | broker_supplier_m2m | `backend/app/models/trade_purchase.py:18` |
| TradePurchase | trade_purchases | `backend/app/models/trade_purchase.py:32` |
| TradePurchaseLine | trade_purchase_lines | `backend/app/models/trade_purchase.py:96` |
| TradePurchaseDraft | trade_purchase_drafts | `backend/app/models/trade_purchase.py:154` |
| MasterUnit | master_units | `backend/app/models/unit_intelligence.py:19` |
| ItemPackagingProfile | item_packaging_profiles | `backend/app/models/unit_intelligence.py:34` |
| OcrItemAlias | ocr_item_aliases | `backend/app/models/unit_intelligence.py:60` |
| SmartUnitRule | smart_unit_rules | `backend/app/models/unit_intelligence.py:82` |
| ItemLearningHistory | item_learning_history | `backend/app/models/unit_intelligence.py:100` |
| UnitConfidenceLog | unit_confidence_logs | `backend/app/models/unit_intelligence.py:117` |
| AiItemProfile | ai_item_profiles | `backend/app/models/unit_intelligence.py:133` |
| SmartPackageRule | smart_package_rules | `backend/app/models/unit_intelligence.py:148` |
| User | users | `backend/app/models/user.py:12` |
| UserSession | user_sessions | `backend/app/models/user_session.py:16` |
| StaffActivityLog | staff_activity_log | `backend/app/models/user_session.py:32` |
| WebhookEventLog | webhook_event_logs | `backend/app/models/webhook_event_log.py:11` |

## Important negative/deferred evidence

- OCR endpoint is a text preview, not a vision implementation: `media.py` decodes base64 bytes to UTF-8; confidence is fixed 0.35/0.55. Catalog/supplier correction learning is not wired. SQL structures alone do not verify a learning flow.
- `ocr_item_aliases` exists in migration 019; `ocr_correction_events` exists in supplemental SQL 020, with an OcrItemAlias ORM model and SQLite bootstrap, but no correction-learning service/API consumer located.
- WhatsApp `whatsapp_po_delivery.py` sends PDF via Graph v19.0, three attempts, 60-second timeout, sent-record lookup, pending_manual fallback. This is not concurrent-safe exactly-once delivery; no webhook/signature/delivery-status handler found. Lifecycle approved automatically calls it in reference, contrary to this user’s explicit operational-action requirement; do not translate that auto-send.
- Config names verified: ENABLE_AI, ENABLE_AI_EXTRACTION, ENABLE_OCR, OCR_PROVIDER, OCR_API_KEY, ENABLE_VOICE, ENABLE_WHATSAPP_PO_DELIVERY, WHATSAPP_PHONE_NUMBER_ID, DIALOG360_API_KEY, DIALOG360_PHONE_NUMBER_ID; names derive Pydantic settings fields. Per-business credential types: whatsapp_api_key, whatsapp_staff_number, whatsapp_phone_number_id. No credentials are introduced.
- Natural-language intent helper/provider failover code exists; no active intent route found in this checkout. Target AI intent is an existing extension to preserve.
- Restore commit returns 501 intentionally (`exports.py::restore_commit`).
- Reference realtime docstring says stub, but implementation streams business queues; source behavior wins over docstring. Target SignalR/PWA/idempotency/BackgroundService are README claims with no active implementations found.
- Historical Dart map/sheet bugs are not applicable as code fixes in React; preserve their loading/error/mobile lessons. Barcode timing in TASKS is mocked, not live API/device performance.

## Target implementation checkpoint (does not relabel initial source evidence)

The initial category statuses above remain the pre-edit baseline. The status document/matrix track computed line discount/tax, variant CRUD/default weight, manual owner payment/balance/due states, catalog archive, business selection/logout and session isolation. Reference trade purchase models/schemas do not contain a variant_id field; its variant delete handler checks legacy archived entry lines. That legacy dependency is not invented as a new purchase schema. The October 2 migration reconciles existing target variant configuration to the reference 512-character width, rebuilding only the derived normalization column/index around the widening.

The full financial source also rounds rate/money/percent/total to two decimals and quantity/weight to three in decimal_precision.py; target four-decimal historical storage is retained and full precision parity is explicitly unresolved. Reference payment patch is cumulative/capped, while mark-paid helpers also provide increments. Reference compute_status prioritizes paid, then overdue/due-soon, then partial. Target represents those payment states separately from delivery lifecycle, as required by the existing architecture, and derives balances on the backend.
