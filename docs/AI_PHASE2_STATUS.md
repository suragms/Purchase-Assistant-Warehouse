# AI Phase 2 Status

| Capability | Status | Evidence |
|---|---|---|
| AI abstraction | IMPLEMENTED | VERIFIED_CODE |
| OpenAI provider | IMPLEMENTED | VERIFIED_CODE |
| Gemini provider | MISSING | VERIFIED_CODE |
| Groq provider | MISSING | VERIFIED_CODE |
| OpenRouter provider | MISSING | VERIFIED_CODE |
| Stub provider | IMPLEMENTED | VERIFIED_CODE |
| Provider factory | MISSING | VERIFIED_CODE |
| Provider routing | PARTIAL | VERIFIED_CODE |
| Failover | PARTIAL | VERIFIED_CODE |
| Timeout | IMPLEMENTED | VERIFIED_CODE |
| Retry handling | IMPLEMENTED | VERIFIED_CODE |
| Error normalization | IMPLEMENTED | VERIFIED_CODE |
| AI disabled mode | MISSING | VERIFIED_CODE |
| Credential security | IMPLEMENTED | VERIFIED_CODE |
| Tenant isolation | IMPLEMENTED | VERIFIED_CODE |
| Reports authorization | IMPLEMENTED | VERIFIED_CODE |
| Backend tests | IMPLEMENTED | VERIFIED_TEST |
| Frontend regression | NOT APPLICABLE | VERIFIED_CODE |

## Notes
- `IAIProvider` and `AIRoutingService` are implemented, but concrete providers for Gemini, Groq, and OpenRouter are missing.
- `AIRoutingService` is implemented but not registered in DI in `Program.cs` and not integrated with any controllers/services.
- Provider failover is partially implemented in `AIRoutingService` but relies on missing provider implementations.
- AI disabled mode is not yet implemented.
