# AI Phase 3 Status

Assessment date: 2026-10-01 (Asia/Calcutta).

Final status: **PHASE 3 COMPLETE**, within the regression scope described below.

| Feature | Status | Evidence |
|---|---|---|
| Intent parsing | IMPLEMENTED | VERIFIED_TEST - PurchaseParsingServiceTests, Phase3SafetyTests |
| Structured candidate | IMPLEMENTED | VERIFIED_TEST - malformed/null structures and untrusted-field stripping |
| Supplier validation | IMPLEMENTED | VERIFIED_TEST - valid, missing, unknown, foreign and duplicate supplier cases |
| Catalog validation | IMPLEMENTED | VERIFIED_TEST - exact, partial, unknown, foreign and duplicate item cases |
| Ambiguity handling | IMPLEMENTED | VERIFIED_TEST, VERIFIED_RUNTIME - no silent selection; user resolves an option or form field |
| Review UI | IMPLEMENTED | VERIFIED_TEST, VERIFIED_RUNTIME - supplier, item, quantity, warnings, discard |
| Edit workflow | IMPLEMENTED | VERIFIED_TEST - editable quantity, supplier, product and financial inputs |
| Explicit confirmation | IMPLEMENTED | VERIFIED_TEST - apply is local; separate submit uses existing purchase endpoint |
| Financial safety | IMPLEMENTED | VERIFIED_TEST - AI fields discarded; service bounds and authoritative totals |
| Stock safety | IMPLEMENTED | VERIFIED_TEST - read-only parsing/drafting; existing PostgreSQL receiving regression |
| Tenant isolation | IMPLEMENTED | VERIFIED_TEST - tenant-filtered resolution and final foreign-ID rejection |
| Authorization | IMPLEMENTED | VERIFIED_TEST - real JWT endpoint checks for 401/403/200 and missing business claim |
| Mobile UI | IMPLEMENTED | VERIFIED_RUNTIME - 320/375/390/768px browser layout and reduced-height scrolling |
| Desktop UI | IMPLEMENTED | VERIFIED_RUNTIME - 1024/1440px browser review, errors, choices and explicit submit |
| Backend tests | IMPLEMENTED | VERIFIED_TEST - final counts recorded below |
| Frontend tests | IMPLEMENTED | VERIFIED_TEST - final counts recorded below |

## Changes required by the audit

No repository or ancestor `AGENTS.md` was present. All five available planning/reference documents and the actual purchase flow were inspected.

The previous completion claims were not accepted as evidence. Source tracing uncovered
unvalidated fields surviving supplier failures, automatic partial catalog selection,
missing numeric validation, missing error/ambiguity controls, and a supplier/broker
array-versus-page response mismatch. These were fixed without changing the lifecycle
or StockService. Duplicate clicks are guarded; retries retain the unique order number.

All four provider transports incorrectly accessed JsonElement as dynamic data; three
omitted system instructions. Targeted fixes and mocked HTTP tests cover the repaired
transport contract. Parser/router logs and errors are sanitized, internal 500 details
were removed, and the fixed token-signing secret was replaced with configuration (or
an ephemeral development key). No real AI credentials were used.

## Final verification

- Backend restore: PASS.
- Backend build: PASS.
- Backend tests: **76 total / 76 passed / 0 failed / 0 skipped** (63 unit/API/transport tests plus 13 PostgreSQL integration tests).
- Frontend install: PASS; dependency audit reported zero vulnerabilities.
- Frontend production build: PASS.
- Frontend component tests: **29 total / 29 passed / 0 failed / 0 skipped**, across 2 test files.
- Browser regressions: **14 total / 14 passed / 0 failed / 0 skipped** in Microsoft Edge.
- Git diff review: PASS; `git diff --check` reported no whitespace errors. StockService and lifecycle methods are unchanged.

## Evidence boundaries

Browser tests use the real React application and mocked HTTP responses. They cover
Dashboard, Reports, Purchases, Stock, Suppliers, Brokers, Users and Notifications at
the screen-smoke level; this is not full live-backend testing of every CRUD operation.
Existing database integration tests separately verify purchase receiving, stock,
dashboard and notifications. No physical phone or real software keyboard was available;
keyboard-space checks reduce the viewport and verify scrolling/access to controls.
Live AI model availability, provider billing and real credentials were not exercised.

The retry identity lasts for the mounted form. A new browser tab or page reload starts
a new form; users must inspect the purchase list after an uncertain save before starting
a fresh order. Production now requires `Jwt__SecretKey` (at least 32 bytes).

The exact execution path and safety decisions are documented in
[AI_PURCHASE_INTENT.md](AI_PURCHASE_INTENT.md). No OCR/Vision, WhatsApp, summaries,
forecasting, traditional ML, or alternative AI purchase-confirm endpoint was added.
