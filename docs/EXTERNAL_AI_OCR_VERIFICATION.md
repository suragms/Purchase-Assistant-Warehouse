# External AI and OCR Verification — 2026-10-03

## Exact status

### Purchase-intent AI

Target integration is implemented for text purchase intent through OpenRouter, Gemini, Groq and OpenAI adapters, with encrypted active-business keys, optional server fallback, ordered failover, normalized failures, request timeout, and an authenticated/rate-limited endpoint. The user must review candidates; the service cannot create or commit a purchase. Automated tests use mocked handlers and fake providers.

**Unverified externally:** credential acceptance, DNS/TLS/network egress from the deployment, model availability, quota/rate behavior, provider billing, real response shape/quality, end-to-end latency under account load, and the actual production key-ring setup. No credentials were supplied or accessed. No secret is requested or included in this repository.

### OCR / image / voice

There is no current main-app OCR/image/voice provider integration or API. Current reference revision drops the old scan tables in `reference-repo/backend/alembic/versions/066_drop_scan_and_whatsapp.py` and explicitly removes `purchase_scan_traces` and `catalog_aliases` in `reference-repo/backend/sql/066_drop_scan_and_whatsapp.sql`. Older parity documentation is stale for this feature. No external OCR call can be verified because there is no live target code path or provider contract.

### WhatsApp

The main app currently has allowlisted WhatsApp credential fields only. It has no outbound send, opt-in/consent, webhook, delivery state, retry, template or external delivery API. Current reference history also removes WhatsApp fields and has no active delivery router at this revision. Credential presence alone is not delivery integration.

## Safe automated evidence

- Provider request serialization and authorization header handling are tested with an in-process handler.
- Business credential resolution, server fallback, missing-key skip, failover, malformed provider output, timeout and normalized errors are covered with test doubles where the relevant tests exist.
- API-key values are returned neither by settings responses nor logs. Provider exception bodies and request prompts are not logged.
- Current business authorization precedes purchase-intent parsing; supplier and catalog context comes from tenant-scoped application services.
- After the Phase 2 missing-credential fix, a provider with an empty key is skipped before HTTP dispatch. This is code-path validation, not account validation.

## Staging verification needed for AI

An operator with an approved non-production provider account should configure a key through the owner settings UI or deployment secret store, then verify: save/masked status; restart/key-ring decrypt; exact supported model; one valid parse with synthetic data; invalid key; missing key; quota/rate response; timeout; malformed upstream response; sequential fallback; no secret in app/proxy/provider logs; correct authorization and tenant context; actual data-retention/region terms; and spend limits. Revoke/rotate the test key at the end. Record provider/model/date and result without recording the key or real purchase prompt.

## Staging verification needed before OCR or WhatsApp work

First approve the feature contract and data-minimization rules. OCR needs supported file/media formats, size limits, consent, retention/deletion, extraction schema, confidence/error behavior and a manual correction path. WhatsApp needs opt-in evidence, templates, recipient selection, webhook signature verification, delivery idempotency, retries, quiet hours and audit/retention behavior. Implement and test those contracts before using credentials. Do not enter any credentials into this report.

## Readiness

Purchase-intent AI is **automated-code-path verified; external-service verification blocked**. OCR/image/voice is **not implemented and externally untestable**. WhatsApp is **not implemented and externally untestable**. No mocked result counts as live delivery evidence.
