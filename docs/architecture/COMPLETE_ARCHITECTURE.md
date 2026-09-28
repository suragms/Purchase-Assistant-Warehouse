# Complete Architecture

## Overview
The Purchase Assistant is a multi-tenant, cloud-ready Warehouse/ERP system designed to manage products, purchasing, stock logic, operations, and reporting. 

## Stack
- **Frontend**: React 18+, TypeScript, Vite, Tailwind CSS, React Router v6, TanStack Query, Zustand, Axios, React Hook Form, Zod, Recharts, Lucide React.
- **Backend**: ASP.NET Core Web API (Targeting latest stable LTS, e.g., .NET 8/9/10), C#, Entity Framework Core.
- **Database**: PostgreSQL (Relational Data), Redis (Caching & SignalR Backplane).

## High-Level Architecture
We use a Clean Architecture / Modular Monolith approach for the backend and a feature-based architecture for the frontend.

### Backend Structure
- **PurchaseAssistant.Api**: Controllers, Middleware, API setup, SignalR Hubs.
- **PurchaseAssistant.Application**: Use cases, CQRS handlers (or services), DTOs, interfaces.
- **PurchaseAssistant.Domain**: Core entities, value objects, domain exceptions.
- **PurchaseAssistant.Infrastructure**: EF Core DbContext, Migrations, Repositories, 3rd party providers (AI, Redis).
- **PurchaseAssistant.Contracts**: Request/Response models used by both API and external clients.

### Frontend Structure (Feature-Based)
```
/src
  /api          (Axios instances, interceptors)
  /auth         (Auth context, token rotation)
  /components   (Shared UI components: buttons, tables, modals)
  /features     (Domain modules: purchases, catalog, stock)
  /hooks        (Global hooks)
  /layouts      (App shell, sidebar, topbar)
  /pages        (Entry points for routes)
  /stores       (Zustand stores for global UI state)
  /types        (TypeScript declarations)
  /utils        (Formatting, pure functions)
```

## Resilience & Performance
- **Idempotency**: All purchase creations require an `Idempotency-Key` header.
- **Concurrency**: Stock operations use standard HTTP 409 + Row Versioning.
- **Caching**: Output caching via Redis for dashboard snapshots and slow queries.
- **Realtime**: SignalR invalidates specific TanStack Query keys via event payloads (e.g., `stock.changed`).

## Deployment
Docker-compose definitions including `api`, `postgres`, `redis`, and optionally `frontend`. CI/CD targets Vercel for frontend and Azure/Render for backend.
