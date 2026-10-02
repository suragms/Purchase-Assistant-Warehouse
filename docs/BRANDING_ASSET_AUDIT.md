# Warehouse Assistant branding and asset audit

Business: **Harisree Agency**. Audited on 2 October 2026; updated for the favicon and authentication-link continuation.

## Asset inventory

The complete project was searched for brand, public, source assets, image/icon extensions, and filenames containing logo, brand, icon, background, bg, banner, splash, favicon, warehouse, harisree, login, signup, avatar, and image. Dependency folders, Git metadata, compiled output, and generated test output were excluded from the source inventory. The two existing reference checkouts were included. There are no project JPG/JPEG, GIF, ICO, AVIF, or BMP source assets, and no dedicated avatar or empty-state illustration assets.

Paths below are repository-relative. “Previous usage” records the state before this change.

| Asset | Path | Type / dimensions / bytes | Purpose | Previous usage | Recommended / implemented usage |
| --- | --- | --- | --- | --- | --- |
| Harisree logo | `brand/logo.webp` | WebP, 235 × 195, 4,370 | Application identity | Not referenced by React frontend | Reused by login, sidebar, mobile drawer/header, authentication/page loaders, and global error screen |
| Get-started background | `brand/getstarted_bg.webp` | WebP, 853 × 1844, 169,598 | Authentication background | Not referenced by React frontend | Reused as the decorative login background |
| Duplicate logo | `images/app_logo.webp` | WebP, 235 × 195, 4,370 | Legacy logo copy | Not referenced by React frontend | Retained; SHA-256 matches `brand/logo.webp`, so no extra copy/import needed |
| Former starter favicon | `frontend/public/favicon.svg` | SVG, 48 × 46, 9,522 | Vite browser tab icon | Referenced by index/offline HTML and v2 shell cache | Retained as an unused source asset; removed from all active production references and precaching |
| Active Harisree favicon | `frontend/public/favicon.png` | PNG, 64 × 64, 6,807 | Browser tab icon, including offline | Added during continuation | Exact copy of root `favicon.png`; active in index/offline HTML and v3 shell cache |
| Active Harisree PWA icon | `frontend/public/icon-192.png` | PNG, 192 × 192, 36,303 | PWA / Apple touch icon | Originally a 1,161-byte generic cube | Exact copy of `icons/Icon-192.png`; existing public URL preserved |
| Active Harisree PWA icon | `frontend/public/icon-512.png` | PNG, 512 × 512, 144,265 | PWA icon | Originally a 3,505-byte generic cube | Exact copy of `icons/Icon-512.png`; existing public URL preserved |
| Original Harisree favicon | `favicon.png` | PNG, 64 × 64, 6,807 | Original business favicon | Not referenced by React frontend | Preserved unchanged; now supplies the frontend public favicon |
| Original Harisree app icon | `icons/Icon-192.png` | PNG, 192 × 192, 36,303 | Application icon | Not referenced by React frontend | Preserved unchanged; now supplies the active frontend icon |
| Original Harisree app icon | `icons/Icon-512.png` | PNG, 512 × 512, 144,265 | Application icon | Not referenced by React frontend | Preserved unchanged; now supplies the active frontend icon |
| Original maskable icon | `icons/Icon-maskable-192.png` | PNG, 192 × 192, 30,455 | Reference maskable application icon | Not referenced by React frontend | Inspected and retained; transparent border means it is not advertised as a maskable icon by the current manifest |
| Original maskable icon | `icons/Icon-maskable-512.png` | PNG, 512 × 512, 123,585 | Reference maskable application icon | Not referenced by React frontend | Inspected and retained; same transparent-border issue |
| Template symbol sprite | `frontend/public/icons.svg` | SVG, 5,031 | Vite template symbols | Only the unused template `src/App.tsx` | Retained; excluded from active app branding |
| Template hero | `frontend/src/assets/hero.png` | PNG, 343 × 361, 13,057 | Vite template illustration | Only the unused template `src/App.tsx` | Retained; no new usage |
| React template logo | `frontend/src/assets/react.svg` | SVG, 4,126 | Framework template logo | Only the unused template `src/App.tsx` | Retained; no new usage |
| Vite template logo | `frontend/src/assets/vite.svg` | SVG, 8,709 | Framework template logo | Only the unused template `src/App.tsx` | Retained; no new usage |

Both `reference-repo/flutter_app/` and `backend/reference-repo/flutter_app/` contain the following reference assets. They are not part of the React application's production runtime and were left intact:

| Asset group | Paths under each reference Flutter app | Type / purpose | Previous usage | Recommended usage |
| --- | --- | --- | --- | --- |
| Original brand assets | `assets/brand/logo.webp`, `assets/brand/getstarted_bg.webp` | WebP / original identity and background | Reference Flutter app | Use the existing root `brand/` counterparts in React; retain originals |
| Duplicate app logo | `assets/images/app_logo.webp` | WebP / logo copy | Reference Flutter app | Retain without another React copy |
| Original favicon | `web/favicon.png` | PNG / browser icon | Reference Flutter app | Retain |
| Original web/PWA icons | `web/icons/Icon-192.png`, `Icon-512.png`, `Icon-maskable-192.png`, `Icon-maskable-512.png` | PNG / application icons | Reference Flutter app | Retain; React uses the requested frontend/public icons |
| Stock golden references | `test/golden/goldens/stock_desktop_1280.png`, `stock_desktop_1440.png` | PNG / two layout baselines | Reference tests | Retain; not application illustrations |
| Stock golden failure images | `test/golden/failures/stock_desktop_{1280,1440}_{isolatedDiff,maskedDiff,masterImage,testImage}.png` | PNG / eight test comparison outputs | Reference tests | Retain; not application illustrations |

Canonical root assets in `brand/`, `icons/`, `favicon.png`, and both reference checkouts are unchanged. The frontend's generic cube icon copies were replaced with exact copies of the existing Harisree PNGs. The new public favicon copy is required for static production hosting and offline precaching. No image was generated, downloaded, or recompressed.

## Implementation

- The visible identity is **Warehouse Assistant**, followed by **Harisree Agency**.
- Vite imports the original root WebP files directly. Production emits one hashed asset for each original; both output files are byte-for-byte identical to their sources. No `/brand/` URL is assumed to exist in `frontend/public`.
- Desktop login uses the existing warehouse background on the left and the existing form on the right at the existing `lg` breakpoint. The background uses `cover` with a fixed focal position and never stretches.
- Mobile retains the existing stacked heading/form, form spacing, inputs, and buttons. A 90% light overlay keeps the same background subtle and the text readable. Minimum viewport height allows the document to grow and scroll when space is reduced.
- Logo dimensions reserve space, meaningful alt text describes the business logo, and `object-contain` maintains aspect ratio. The background is decorative and hidden from assistive technology. Critical login images are not lazy-loaded.
- Mobile header now shows the logo and two lines of identity. The existing search trigger becomes an icon at mobile widths, retaining its accessible name and functionality. Drawer branding reserves space for the existing close control.
- Authentication and route-loading states reuse the logo and identity, without a new delay. The global error screen also shows the identity. Existing 404 and access-denied pages inherit it from the application shell.
- Browser title and manifest name, short name, description and PWA icon URLs retain the verified configuration. Index and offline HTML now use `/favicon.png` with `image/png` and `64x64`. The Apple touch icon remains `/icon-192.png` and now displays the Harisree crest. There is no `document.title` override in the frontend.
- The manifest retains the 192/512 icon URLs with `purpose: any`. The selected original PNGs are opaque RGB business artwork. The separate reference maskable files contain transparency at their borders, so those files are not added and the originals are not modified to manufacture replacements.
- Offline HTML keeps the requested title and identity. The service-worker shell cache is now v3, replacing v2's starter favicon and generic icons. It precaches `/favicon.png` and both Harisree icon PNGs. Existing update activation, network-required navigation, and API cache exclusions are preserved.
- The unsupported `href="#"` password action is replaced in the same form position by non-interactive text: “For sign-in help, contact your business owner.” No signup/reset workflow or authentication API logic was added.
- Existing empty states use small Lucide icons; no project illustration is intended there, so their presentation is preserved.

## Old branding classification

| Occurrence | Classification | Result |
| --- | --- | --- |
| `Connection required · Purchase Assistant` in offline HTML | User-facing browser title | Replaced with `Warehouse Assistant | Harisree Agency` |
| `AI Purchase Assistant` feature heading | User-facing feature label containing the former name | Renamed to `AI Purchase Helper`; component/file names and APIs preserved |
| AI feature label in unit and browser expectations | Test expectation | Updated to match the visible feature heading |
| `Purchase assistant and authoritative submit` test description | Internal test suite label | Preserved |
| `PurchaseAssistant` component/import filenames | Internal identifiers | Preserved |
| `purchase-assistant-shell-*` service-worker cache prefix | Internal cache identifier | Preserved; version only increased to invalidate outdated offline content |
| C# namespaces, assemblies, routes, database identifiers and reference checkout branding | Internal/backend/reference identifiers | Untouched |

There are no remaining occurrences of `Purchase Assistant`, `PurchaseAssiastant`, `HEXA Purchase Assistant`, or Vite/React starter identity in active frontend UI copy. `src/main.tsx` mounts `AppRouter`; the unused template `src/App.tsx` and its SVGs are not imported into the production app. Vite environment names, test suite labels, build tooling and repository/package names are internal and were not renamed. The old public SVG remains on disk without an active reference.

## Files changed across branding work

| File | Change |
| --- | --- |
| `frontend/src/components/BrandIdentity.tsx` (new) | Shared original logo, two-line identity and loading UI |
| `frontend/src/pages/auth/Login.tsx` | Original logo/background, desktop asset placement, mobile overlay, autofill hints; unsupported password link replaced by owner-help text |
| `frontend/src/layouts/AppShell.tsx` | Sidebar/drawer logo, mobile header identity, page-loading identity |
| `frontend/src/auth/AuthProvider.tsx` | Branded loading presentation; authentication logic unchanged |
| `frontend/src/router/index.tsx` | Branded route-loading fallback; routes unchanged |
| `frontend/src/components/RouteErrorBoundary.tsx` | Branded error presentation; error/reload logic unchanged |
| `frontend/src/components/AI/PurchaseAssistant.tsx` | Visible feature heading only |
| `frontend/index.html` | Harisree PNG favicon reference |
| `frontend/public/offline.html` | Browser title, application name, business subtitle and Harisree PNG favicon reference |
| `frontend/public/sw.js` | v3 offline cache and Harisree favicon/icon precaching |
| `frontend/public/manifest.webmanifest` | Harisree icons declared with standard `any` purpose |
| `frontend/public/favicon.png` (new) | Exact copy of the existing 64 × 64 business favicon |
| `frontend/public/icon-192.png` | Exact copy of the existing 192 × 192 Harisree icon |
| `frontend/public/icon-512.png` | Exact copy of the existing 512 × 512 Harisree icon |
| `frontend/src/components/users/PermissionEditor.tsx` | Removed a pre-existing unused `ShieldAlert` import blocking the production build; no behavior change |
| `frontend/src/tests/PurchaseAssistant.test.tsx` | Updated label and corrected a pre-existing stale exact payload expectation to include current freight/commission/unit fields |
| `frontend/e2e/phase3.spec.ts` | Updated visible AI helper label expectation and corrected a pre-existing damage-report mock to return the API's list shape |
| `frontend/e2e/branding.spec.ts` (new) | Production responsive branding, validation, scrolling, navigation and PWA/offline checks; exact source bytes, v2 cache removal and absence of dead auth links verified |
| `docs/BRANDING_ASSET_AUDIT.md` (new) | Asset inventory and final audit report |

## Authentication support audit

- Current React routes contain only login; no public signup, forgot-password, or reset-password page exists.
- Current C# `AuthController` has no public registration endpoint. Account creation is an authenticated, permission-controlled operation in `UsersController` (`RequireUsersManage`). Signup is **NOT_SUPPORTED** in the current public authentication workflow.
- Current `AuthController.cs` lines 382–397 explicitly return HTTP 503 / `PASSWORD_RESET_UNAVAILABLE` for both forgot/reset endpoints because token issuing and secure delivery are not configured. Self-service password reset is **NOT_SUPPORTED**. DTOs and route names alone do not constitute an implemented workflow.
- The reference FastAPI repository disables public registration by default. Its Flutter router redirects `/signup` to login with an owner-only notice. Its production password help directs users to the business owner; the reference reset code is not the current C# implementation.
- The misleading login `#` action is removed. No dead signup/forgot/reset links remain in the current login UI. The replacement owner-help message does not promise automated email or password reset.

## Exact files changed in the continuation

1. `frontend/index.html`
2. `frontend/public/offline.html`
3. `frontend/public/sw.js`
4. `frontend/public/manifest.webmanifest`
5. `frontend/public/favicon.png` (new)
6. `frontend/public/icon-192.png`
7. `frontend/public/icon-512.png`
8. `frontend/src/pages/auth/Login.tsx`
9. `frontend/e2e/branding.spec.ts`
10. `docs/BRANDING_ASSET_AUDIT.md`

Other earlier branding changes in the working tree are preserved. This continuation does not edit Damage, Staff RBAC, Permission Editor, User Management, Stock, financial calculations, AI, OCR, realtime, app shell layout, or authentication logic.

## Verification

- `npm run build`: **PASS**. Runs the actual `tsc -b && vite build` command. Original logo and background resolve correctly in production output.
- `npm test`: **PASS**, 6 files and 57 tests on the continuation run. No unit tests were changed in this continuation.
- Previous branding work corrected a stale unit payload expectation and a damage-report browser fixture shape; those verified corrections were preserved and not revised here.
- `npm run test:e2e`: **PASS**, all 76 tests in 3.3 minutes on the continuation run. This includes the six requested mobile/desktop sizes, reduced keyboard viewport, login/session behavior, all existing regressions, exact Harisree asset bytes and offline cache checks. Test expectations changed only for the intended favicon/auth-link behavior; no regression assertion was removed.
- Responsive branding checks: **PASS**, all six required sizes: 390 × 844, 393 × 852, 412 × 915, 1366 × 768, 1440 × 900, 1920 × 1080.
- Production login, heading/subtitle, logo dimensions/loading, cover background loading/MIME, form/button visibility, validation, dashboard branding, search, drawer close control, and 404 checks pass at all requested sizes. No horizontal overflow.
- A 390 × 360 reduced viewport confirms that focused password input and the submit button remain reachable by scrolling. This simulates keyboard space; a physical device keyboard was not used.
- Static checks confirm PNG dimensions, opacity and SHA-256 identity with the three selected canonical assets. Browser verification checks production PNG MIME types, exact source bytes, manifest, v3 installation/removal of v2, and offline retrieval of all three brand icons alongside the navigation fallback.
- Screenshot review covers mobile login/header/drawer and desktop login/sidebar/dashboard. Outputs are in ignored `frontend/test-results/`.
- `git diff --check`: **PASS**.
- Backend build: not required; no backend or shared configuration changes.

## Status and remaining issues

| Requested category | Result | Notes |
| --- | --- | --- |
| BRANDING | PASS | Requested text identity on all existing application/authentication surfaces |
| ASSET AUDIT | PASS | Original assets and reference copies inventoried; canonical root sources unchanged |
| LOGIN | PASS | Original background and logo, validation and responsive checks pass |
| SIGNUP | NOT_SUPPORTED | Current app uses authenticated user provisioning; no public registration API/workflow |
| PASSWORD RESET | NOT_SUPPORTED | Backend forgot/reset routes explicitly return `PASSWORD_RESET_UNAVAILABLE`; dead link removed |
| DESKTOP | PASS | All requested desktop sizes, sidebar, login, dashboard and navigation checked |
| MOBILE | PASS | All requested mobile sizes, drawer, header and login checked; reduced keyboard viewport scrolls |
| PWA | PASS | Requested manifest/icons and offline behavior verified |
| PWA ICONS | PASS | Existing Harisree 192/512 PNGs active, original MIME/dimensions/source bytes verified |
| FAVICON | PASS | Existing Harisree 64 × 64 PNG active online/offline; Vite reference and precache removed |
| BUILD | PASS | Production build succeeds |
| UNIT TESTS | PASS | 57/57 tests in 6 files |
| BROWSER TESTS | PASS | 76/76 tests, including all 14 branding/responsiveness checks and 62 existing regressions |
| TESTS | PASS | 57 unit and 76 browser tests pass |

Signup and self-service password reset remain absent because the current architecture does not support them. No misleading authentication action remains. Session expiry redirects to the branded login page. Desktop collapsed-sidebar mode remains absent; existing navigation behavior is preserved. The active PWA and browser icons now show the same Harisree identity as the existing logo. Physical device keyboards were not used; reduced viewport checks cover keyboard space.
