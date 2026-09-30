# AI Provider Architecture

This document describes the structured AI provider infrastructure for the Warehouse Purchase Assistant.

## Overview
The system uses a pluggable provider pattern to support multiple AI backends with automated failover capability.

## Components
- `IAIProvider`: Interface for all AI services.
- `IAIProviderFactory`: Factory pattern for provider resolution.
- `AIRoutingService`: Service responsible for request orchestration, failover, and health checks.
- `AiOptions`: Configuration for AI settings.

## Failover Strategy
The system follows a defined fallback order:
1. OpenRouter
2. Gemini
3. Groq
4. OpenAI
5. Stub

If a primary provider fails (non-retryable or exhausting retry attempts), the routing service automatically cascades to the next configured provider, ensuring service reliability.

## DI Integration
Providers and services are registered in `Program.cs` utilizing named/typed `HttpClient` instances registered via `AddHttpClient<T>`.
