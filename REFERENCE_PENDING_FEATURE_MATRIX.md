# REFERENCE PENDING FEATURE MATRIX

| Feature | Source | Status | Dependency | Port Required | Comment |
| :--- | :--- | :--- | :--- | :--- | :--- |
| LLM-based OCR | `ocr_parser.py` | Implemented | OpenAI/Gemini/Groq | Yes | Essential for rapid PO entry. |
| WhatsApp Delivery | `whatsapp_po_delivery.py` | Implemented | WhatsApp Cloud API | Yes | PO delivery automation. |
| AI Usage Logging | `llm_failover.py` | Implemented | DB (AiUsageLog) | Yes | Essential for cost tracking. |
