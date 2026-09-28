# API Specification

## Conventions
- **Base URL**: `/api/v1`
- **Response Format**:
  - Success: `{"data": T, "meta": {page: number, pageSize: number, total: number}}`
  - Error: `{"error": {"code": string, "message": string, "details": object, "requestId": string}}`
- **Authentication**: Bearer JWT tokens in `Authorization` header. Refresh token in `HttpOnly` secure cookie.
- **Idempotency**: All `POST`/`PUT` requests for sensitive transactions MUST support `Idempotency-Key` header.
- **Concurrency**: All update operations on stock or lifecycle MUST enforce concurrency checks (e.g., `If-Match` or `RowVersion`).

## API Modules (v1)
- `GET/POST /auth/*`
- `GET/POST/PUT/DELETE /catalog/*`
- `GET/POST/PUT/DELETE /purchases/*`
- `GET/POST/PUT /stock/*`
- `GET /reports/*`
- `GET/POST/PUT /suppliers/*`
- `GET/POST /notifications/*`
- `GET /health`
