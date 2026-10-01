# AI Purchase Intent - Phase 3

## Authority and execution path

AI assistance is optional. `PurchaseService` owns purchase validation and financial totals;
`StockService` owns stock movements. AI is not authoritative for stock, price, totals,
tax, profit, permissions, payment/delivery state, or purchase lifecycle state.

The verified source path is:

1. The user enters natural language in `frontend/src/components/AI/PurchaseAssistant.tsx`.
2. An explicit **Analyze Intent** click calls `purchaseIntentApi.parseIntent` through a
   TanStack mutation. The client posts only `{ prompt }` to
   `POST /api/v1/ai/purchase-intent/parse`.
3. `PurchaseIntentController` requires `RequirePurchaseCreate` and a business claim in
   the authenticated token. A business ID in the request body has no effect.
4. `PurchaseParsingService` bounds input, calls `IAIRoutingService`, and deserializes a
   structured candidate. Routing uses the existing provider factory/failover chain.
5. The parser resolves supplier and catalog data using services scoped by
   `AppDbContext` tenant query filters. It constructs a new validated candidate rather
   than returning the model's IDs, options, warnings, states, or financial fields.
6. The assistant displays supplier, items, ambiguity, missing information and quantity
   warnings. The user can edit quantities, choose a returned catalog option, or discard.
7. **Apply to purchase form** explicitly maps catalog ID, quantity and validated supplier
   into `PurchaseForm`. Item prices start at zero. No API writes occur here. Unresolved
   products remain blank; unresolved suppliers clear any previous supplier selection.
8. The user reviews supplier, item, quantity, price, tax, and notes in the existing form.
   Required unresolved fields prevent submission. Form totals are previews only.
9. A separate **Create Purchase Order** click calls the existing `purchaseApi.createPurchase`
   (`POST /api/v1/purchases`) and `PurchaseController.CreatePurchaseOrder`.
10. `PurchaseService.CreatePurchaseOrderAsync` revalidates supplier, broker and catalog
    IDs in the current business, validates numeric bounds, computes totals using its
    existing calculations, and saves a **Draft** with pending payment/delivery.
11. Navigation and the success toast occur only after API success. Existing purchase
    detail actions still own Confirm/Dispatch/Receive. Receiving invokes the existing
    `StockService.AdjustStockAsync` purchase-receipt behavior. Draft creation and lifecycle
    confirmation are distinct operations; applying the AI draft performs neither.

There is no `/ai/purchase/confirm` endpoint and no AI database mutation path.

## Resolution and validation

- Supplier resolution accepts one exact name match, or a tenant-validated supplier ID
  when no name is present. Missing, unknown, foreign, and duplicate names yield no
  selected supplier. Users choose a supplier in the existing form.
- Catalog resolution accepts a unique exact code/name match only when the search page
  is complete. Partial matches, multiple exact matches, and truncated search pages
  require a user choice. Unknown/blank products remain unresolved; model-supplied IDs
  and options never survive validation. Options come from the tenant's catalog.
- Repeated item searches are cached per parse. Supplier data is read once per parse;
  linked-item counts use one projected database query rather than one query per supplier.
- Quantities must be positive, at most `99999999999999`, and have at most four decimal
  places. The existing database uses numeric(18,4). Zero, negative, excessively large,
  malformed and over-precision quantities cannot be saved. Decimal quantities work.
- Purchase creation/update validate 1-200 items, nonnegative bounded price/tax, and a
  bounded purchase total. The existing PurchaseService calculation remains authoritative.
- Prompt length is 1-4000 non-whitespace characters; candidates have at most 200 items.
  Malformed/null model structures return a controlled error with no candidate items.

## Errors, retries and feedback

Both parse and submit have immediate in-flight guards plus disabled/loading buttons.
Parse mutations do not invalidate purchase or stock queries. Purchase queries are
invalidated only after a successful create/update.

A new form keeps the same client-generated `OrderNumber` through retries. The existing
unique `(BusinessId, OrderNumber)` database index prevents duplicate orders, including a
retry after a response is lost. A conflict asks the user to inspect the purchase list;
it does not claim success or automatically change the number. This protection covers
retries in the current mounted form, not re-entry in a new tab or after a reload.

AI-disabled, malformed-response, unavailable-provider and timeout states permit manual
entry. HTTP 400/401/403/409/429/500/502/503/504, connection failures and client timeouts
have controlled messages. Submit failures preserve edits and focus the error for visibility.
The parser gives provider execution a 45-second cancellation budget; the browser's parse
request times out at 60 seconds. Cancellation propagates through routing/providers.

Provider responses use explicit JSON access (the previous dynamic JsonElement access
failed at runtime). All providers now receive the structured-output system instructions.
Raw provider errors/content are not returned or logged by the parser/router; Gemini
credentials are sent in a header rather than the URL. General 500 responses no longer
include internal exception details.

## Configuration

`AI:Enabled=false` retains the normal manual workflow. Provider credentials remain in
server configuration, never the browser. Live provider accounts and current model
availability were not exercised; regression tests use mocked provider HTTP responses.
The existing provider defaults and routing priority were retained.

Configure `Jwt__SecretKey` (or `Jwt:SecretKey` through server configuration) with at least
32 bytes for non-development environments. The previous shared signing key embedded in
source was removed during the security audit. Development without a configured key uses
a random process-local key; restarting invalidates old access tokens. Tests use a test-only key.

## Regression evidence

See [AI_PHASE3_STATUS.md](AI_PHASE3_STATUS.md) for exact results and evidence classification.

- Backend: parser/service safety tests use tenant-filtered EF stores and mocked routing;
  endpoint tests use real JWT authentication/authorization in an ASP.NET test host;
  transport tests mock provider HTTP. Existing PostgreSQL integration tests cover
  purchase receiving, stock, dashboard and notifications.
- Frontend: component tests mock APIs and exercise review, edits, discard, explicit
  submit, errors, numeric validation, manual entry, pending guards and retry identity.
- Browser: Playwright exercises the actual application with mocked HTTP at 320, 375,
  390, 768, 1024 and 1440 pixels, checks both document and main-content overflow,
  ambiguous selection, scroll access, and errors. Reduced viewport height simulates
  available keyboard space; physical mobile keyboards were not tested.
- Browser smoke checks cover Dashboard, Reports, Purchases, Stock, Suppliers, Brokers,
  Users and Notifications. These checks verify rendering/navigation, not every operation
  against a live backend. The assistant introduces no dialogs.

Run from `backend`: `dotnet restore`, `dotnet build`, `dotnet test`.
Run from `frontend`: `npm install`, `npm run build`, `npm test`, `npm run test:e2e`.
The browser suite uses installed Microsoft Edge through Playwright. On this Windows
host, API error-path tests require event-log access and browser tests require permission
to shut down their temporary server processes.

Phase 4 OCR/Vision, WhatsApp, summaries, forecasting and traditional ML are outside this change.
