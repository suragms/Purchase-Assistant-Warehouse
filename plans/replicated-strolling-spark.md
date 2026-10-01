# Implementation Plan: Fix Staff User Management

## Context
The current warehouse ERP has issues with staff user management. Specifically, user creation/management requires verification against the reference implementation to ensure correct role-based access control and functional parity, especially for Staff users.

## Approach
1. **Audit Roles and Permissions**: Compare `backend/PurchaseAssistant.Domain/Constants/Permissions.cs` with `reference-repo/backend/app/services/permissions.py`.
2. **Investigate Authorization**: Analyze `UserService.RequireUserAdministrator` and `UserService.CreateUserAsync`. The requirement is to ensure correct authorization for creating/managing Staff users while maintaining security constraints (e.g., only Admin/Owner can create/manage users).
3. **Verify Staff Creation**: Test the user creation flow for Staff roles to pinpoint where it fails or behaves incorrectly.
4. **Fix**: Apply necessary fixes to `UserService.cs` and ensure the frontend `UsersPage.tsx` correctly reflects available actions based on backend permissions.
5. **Regression Testing**: Ensure all user management flows (Create, Update, Delete) are functional for authorized roles and blocked for unauthorized ones.
6. **Documentation**: Update parity documentation.

## Critical Files
- `backend/PurchaseAssistant.Infrastructure/Services/UserService.cs`
- `backend/PurchaseAssistant.Web/Controllers/UsersController.cs`
- `backend/PurchaseAssistant.Domain/Constants/Permissions.cs`
- `frontend/src/pages/users/UsersPage.tsx`

## Verification
- Run backend unit tests: `dotnet test backend/PurchaseAssistant.UnitTests/backend/PurchaseAssistant.UnitTests.csproj`
- Run backend integration tests: `dotnet test backend/PurchaseAssistant.IntegrationTests/backend/PurchaseAssistant.IntegrationTests.csproj`
- Run frontend tests: `npm test`
- Perform manual browser verification of the user management flow, specifically:
    - Attempt to create a Staff user as an Owner.
    - Attempt to create a Staff user as a Manager (verify it fails if disallowed).
    - Ensure newly created Staff users appear in the user list and can log in (if authorized).
    - Audit visibility of the Users menu.
