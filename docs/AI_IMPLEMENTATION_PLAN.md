# AI Implementation Plan

## 1. Verified Reference Features to Implement
- **Generative AI/LLM Integration:** Tiered provider failover (OpenRouter, Gemini, Groq, OpenAI) for purchase extraction.
- **OCR/Vision:** Purchase bill text parsing using multimodal AI processing (OpenAI Vision).
- **Purchase Intent Parsing:** Natural language purchase drafting (drafts only, backend-validated).
- **Confidence-based Escalation:** Handle low-confidence AI extraction with escalation or heuristic fallback.
- **Structured Output Validation:** Prevent AI-hallucinated financial data from persisting.
- **WhatsApp PO Delivery:** Generate PO PDF and deliver via WhatsApp Cloud API.

## 2. Target Implementation Architecture
- **Provider Abstraction:** `IAIProvider` in `PurchaseAssistant.Application` with concrete implementations in `PurchaseAssistant.Infrastructure` (using `HttpClient`).
- **Orchestration:** `AIRoutingService` for tiered failover and confidence-based escalation.
- **OCR/Intent:** API Endpoints in `PurchaseAssistant.Web` (e.g., `/ocr/extract`, `/purchase/draft-from-text`) validating output.
- **Logging/Monitoring:** Audit table in Postgres for AI costs and usage.
- **WhatsApp:** `IWhatsAppService` in `PurchaseAssistant.Application` with `HttpClient` implementation.

## 3. Implementation Order
1.  **Phase 2: Provider Abstraction & Infrastructure**
    - `IAIProvider`, factories, secrets/configuration.
2.  **Phase 4/5: OCR & Intent Parsing Infrastructure**
    - Parsing services, schemas.
3.  **Phase 11: WhatsApp Integration**
    - PDF generation (verify reference code usage), API delivery.
4.  **Phase 9: AI Assistant UI**

## 4. Security Requirements
- Keys encrypted at rest, never logged, configurable per tenant.
- Strict authorization policies (Owner/Manager).
- All AI inputs treated as untrusted.

## 5. Tests
- Integration tests for failover logic, OCR parsing success/failure scenarios, and WhatsApp API integration (mocked).
