# Complete Architecture

## 1. High-Level Architecture
The system follows a strict Client-Server architecture separated into a React Frontend and an ASP.NET Core Web API Backend, communicating over HTTPS using REST and SignalR.

## 2. Backend Architecture - ASP.NET Core (.NET 10)
Uses Clean Architecture (Onion) with feature-based slicing where applicable.

### 2.1 Backend Layers
* **PurchaseAssistant.Domain:** Core business models, interfaces, Domain Exceptions, Enums. No external dependencies.
* **PurchaseAssistant.Application:** CQRS patterns (MediatR style or standard Service interfaces), DTOs, FluentValidation rules, Business logic (interfaces).
* **PurchaseAssistant.Infrastructure:** Entity Framework Core DbContext, PostgreSQL repositories, SignalR Hubs, BackgroundServices, JwtService, Serilog config, Caching layer (Redis/MemoryFallback).
* **PurchaseAssistant.Web (API):** Controllers, Middleware (Global Exception, JWT Auth, Idempotency, Rate Limiting), Swagger setup.
* **PurchaseAssistant.Contracts:** Shared response models, standardized HTTP wrapper types.

### 2.2 Security & Resilience
* **Tenancy:** `BusinessId` isolated via EF Core Global Query Filters.
* **Authentication:** JWT access (15min) + RefreshToken (HttpOnly cookie, 7 days, single-flight rotation).
* **Resilience:** Polly for internal retries, global exception handler returning standard ProblemDetails.
* **Concurrency:** `RowVersion` on sensitive records (Stock, Purchases).

## 3. Frontend Architecture - React (Vite)
Uses a Feature-Sliced Design to prevent horizontal monoliths.

### 3.1 Frontend Folders
* `/api`: Axios instance with Interceptors for auto-refresh.
* `/auth`: Authentication context and guards.
* `/components`: Generic/Shared UI (Buttons, Inputs, Modals) using Tailwind & Lucide.
* `/features`: Domain slices (e.g., `/features/purchases`, `/features/catalog`). Each feature contains its own hooks, components, schemas, and types.
* `/layouts`: Application Shell, Sidebar, AuthLayout.
* `/pages`: Route entry points assembling features.
* `/router`: React Router configurations.
* `/stores`: Zustand stores for UI state (e.g., Sidebar expansion, Theme).
* `/theme`: Tailwind config overrides, CSS globals.

### 3.2 State Management
* **Server State:** TanStack Query (`useQuery`, `useMutation`). Strict query key factory.
* **Client State:** Zustand (only for UI preferences).
* **Form State:** React Hook Form + Zod resolvers.

## 4. Real-time & Background Jobs
* **SignalR:** Pushes `stock.changed`, `purchase.changed` events directly to clients to invalidate TanStack query caches.
* **Hosted Services:** C# `BackgroundService` for checking low stock alerts globally and scheduling reporting snapshots.