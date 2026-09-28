# API Specification

## 1. REST Conventions
Base URL: `/api/v1`

### Generic Standard Response
All responses are wrapped in a standard envelope.

#### Success
```json
{
  "data": { ... },
  "meta": {
     "page": 1,
     "pageSize": 50,
     "totalCount": 200,
     "totalPages": 4
  }
}
```

#### Error (ASP.NET ProblemDetails mapped)
```json
{
  "error": {
    "code": "STOCK_VERSION_CONFLICT",
    "message": "Stock changed by another user.",
    "details": {
       "field": ["validation error detail"]
    },
    "requestId": "req-12345"
  }
}
```

## 2. Protection Mechanisms
- **Idempotency**: Requests like `POST /api/v1/purchases` require the `Idempotency-Key` header.
- **Rate Limiting**: Applied via `.NET RateLimiting` middleware.
- **Authorization**: Extracted from JWT claim `role` and `permissions`.

## 3. Core Endpoints Overview (Samples)

### Auth
- `POST /auth/login` -> Returns Access Token & Sets HttpOnly Refresh Token
- `POST /auth/refresh` -> Single-flight token rotation
- `POST /auth/logout`

### Catalog
- `GET /catalog/items?page=1&search=ABC`
- `POST /catalog/items`
- `PATCH /catalog/items/{id}` (Includes `RowVersion` for concurrency)

### Stock
- `GET /stock/audits`
- `POST /stock/physical` (Payload: ItemId, NewQty, Reason)
- `POST /stock/system/adjust` (Payload: ItemId, Difference, Reason)

### Purchases
- `POST /purchases` (Draft creation)
- `GET /purchases/duplicates/check` (Payload: SupplierId, Lines hash)
- `POST /purchases/{id}/commit` (Moves purchase to StockCommitted, triggers stock adjustment logic).

## 4. SignalR Events
Event Name: `ReceiveEvent`
Payload:
```json
{
  "entity": "stock|purchase|notification",
  "action": "updated|created",
  "id": "uuid-here"
}
```