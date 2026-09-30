# REFERENCE AI/ML EXTRACTION

## 1. Summary
- **Traditional ML Models:** NO VERIFIED TRADITIONAL ML MODEL FOUND.
- **LLM/Generative AI:** Integrated via `llm_failover.py` and `llm_intent.py`. Used for OCR parsing and purchase intent extraction.
- **OCR/Document Intelligence:** Supported via OpenAI Vision v2/v3 (see `purchase_scan_service` referenced in `ocr_parser.py`).
- **Voice AI:** No voice-related functionality verified.
- **WhatsApp:** Supported for Purchase Order (PO) delivery via WhatsApp Cloud API (Meta Graph).

## 2. AI Providers
- **OpenRouter:** Used for Tier 1 inference (DeepSeek, Kimi, Xiaomi).
- **Gemini (Google):** Tier 2 provider.
- **Groq:** Tier 2 provider.
- **OpenAI:** Tier 2 provider.

*Security Model:* Uses `provider_credentials` service to resolve keys from owner-defined settings (`db`) or deployment env. Never logs keys.

## 3. WhatsApp Integration
- **Functionality:** `deliver_po_whatsapp` (in `whatsapp_po_delivery.py`)
- **Capability:** Generates PO PDF and sends via WhatsApp Cloud API (Meta Graph).
- **Permissions:** Owner/Manager context.

## 4. AI Safety Rules
Rules enforced in code:
- AI does not finalize purchases (Draft -> ... -> Backend Totals -> Confirm).
- AI parsing results always validated against catalog/DB.
- Structured output (JSON) is parsed, and low-confidence/missing-field results trigger escalation or heuristic fallback.
- No AI-based authority for stock, price, tax, totals, profit, permissions, or workflow state.
