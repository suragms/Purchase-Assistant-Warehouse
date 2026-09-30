# AI Purchase Intent Parsing

## Overview
AI-driven purchase intent parsing generates structured purchase drafts from natural language requests.
Authoritative system: Backend services (PurchaseService, StockService).

## Architecture
- `IPurchaseParsingService`: Handles AI parsing and backend validation.
- `AIRoutingService`: Routes requests through tiered providers.
- `PurchaseIntentController`: Secure API endpoint.

## Workflow
1. Natural language prompt -> API.
2. `PurchaseParsingService` routes to LLM (structured output).
3. LLM returns `PurchaseIntentCandidateDto`.
4. `PurchaseParsingService` validates IDs (Supplier/CatalogItems) against live data.
5. Result returned to client.
6. User reviews, edits, confirms.
7. Confirmation calls `PurchaseService.CreatePurchaseOrderAsync`.

## Safety Constraints
- Financial/Stock Isolation: AI cannot compute values or mutate stock.
- Tenant Isolation: Scoped resolutions.
- No Auto-Confirmation.
