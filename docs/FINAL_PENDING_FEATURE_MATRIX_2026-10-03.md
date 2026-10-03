# Final pending feature matrix — 2026-10-03

Phase 3 execution record. Status is evidence-based; IMPLEMENT/FIX/INTEGRATE/TEST remain open until their verification is recorded. The Phase 2 documents are inputs, not proof of current completeness. Business membership is the warehouse boundary; no separate branch stock pool exists in either implementation.

| ID | Area / remaining work | Initial category | Verification / completion evidence |
|---|---|---|---|
| P01 | Forecast training, temporal validation, baseline comparison, versioned artifacts | IMPLEMENT | DailyUsageLog is the consumption label; purchases and adjustments are not demand. |
| P02 | Scoped inference, history, reorder, risk, anomalies and monitoring | IMPLEMENT | Never infer without validated data/artifact; no pooled tenant training. |
| P03 | ML frontend, loading/empty/error, export and responsive integration | INTEGRATE | New route must use server results and existing permissions. |
| P04 | ML data, leakage, repeatability, artifact failure and authorization tests | TEST | Include missing dates, zeros, duplicates, future timestamps and tenant mismatch. |
| P05 | Supplier history / optional historical price suggestion | IMPLEMENT | Existing SupplierItemPrice is disconnected; purchase records are authoritative. |
| P06 | Audit search, staff activity, filters and immutable records | IMPLEMENT | SecurityAuditLog lacks an application immutability guard and global read UI. |
| P07 | Mutation audit completeness | FIX | Catalog/contact/category/profile/settings/stock/operations writers need consistent transaction-bound provenance. |
| P08 | Notification event coverage and recipient permissions | FIX | Review stock, purchase, discrepancy, reconciliation, damage, staff and ML events; honor settings and dedupe. |
| P09 | Provider configuration, model selection, retry, timeout and circuit breaker | IMPLEMENT | Existing encrypted keys and failover remain; live account verification is separate. |
| P10 | Reports, exports, filtering, database totals and response states | FIX | Existing CSV/XLSX/PDF preserve server authority and financial restrictions. |
| P11 | Owner/staff route and direct API authorization matrix | TEST | Include current membership, explicit permissions, object substitution and selected-business boundary. |
| P12 | Purchase and stock lifecycle regression | TEST | Preserve preview tokens, charges, discounts, verification, receiving, payments and concurrency. |
| P13 | Low-stock enrichment and follow-up actions | INTEGRATE | Category/supplier/severity filters and reorder navigation. |
| P14 | Search/contact/duplicate query bounds and performance | FIX | Bound results and measure meaningful workloads, without caching mutable stock. |
| P15 | Desktop/tablet/mobile browser regression and lint/build gates | TEST | Preserve mobile design. Fixtures and live persistence evidence must be distinguished. |
| P16 | Complete PostgreSQL suite and migration rehearsal | TEST | Use the existing isolated wa_test_* harness; skipped cases are not passes. |
| P17 | Full reference/source placeholder and AI parity scan | TEST | Scan both reference copies; input placeholders, test doubles and explicit unavailable states are legitimate. |
| P18 | Owner/staff core CRUD, profile/password, settings, inventory and purchases | TEST | Verify existing implementation rather than replacing working modules. |
| P19 | Warehouse/branch model | COMPLETE | One logical warehouse per Business; profile editing and membership assignment are the supported operations. Multi-location transfers are outside this model. |
| P20 | Live AI provider/key-ring/quotas/output quality | EXTERNAL VERIFICATION | Requires an approved configured provider account and deployment environment. |
| P21 | Production backup restore, deployment, operational recovery | EXTERNAL VERIFICATION | Local synthetic rehearsal available; production-copy restore is gated and must not be enabled by bypassing it. |
| P22 | Physical device, installed PWA, native keyboard/camera | EXTERNAL VERIFICATION | Responsive browser evidence does not certify unavailable devices. |
| P23 | Account recovery delivery | BLOCKED | No email/SMS delivery adapter or configured provider contract. Existing password changes/session revocation remain available. |
| P24 | OCR/voice/WhatsApp | BLOCKED | Verify current executable reference code, not historical migration/table names. No live target provider contract or credentials supplied. |
| P25 | Production forecast artifact and accuracy | TEST | Inspect available history; test fixtures prove mechanics only. Train production artifacts only from eligible real records. |

## Scope rules

- No static prediction or dashboard values are acceptable as production data.
- Statistical risk scenarios are not calibrated stockout probabilities; absent outcome labels cannot support that claim.
- Missing consumption days are not zero demand. Explicit zero records are valid.
- Source findings and final commands/results will be appended as work completes.
