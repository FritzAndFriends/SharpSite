# Project Context

## 2026-04-30 — v0.8 Milestone Role Assignment

**Role:** Admin UI Lead (v0.8.0 Tier 2)  
**Effort:** 15–20 hours  
**Timeline:** v0.8.0 (6 weeks)  

**Tier 2 (after Tier 1 ship-blockers):**
- #321 — Logo management in Admin UI (3-4h) — upload, preview, admin panel
- #323 — Set favicon (1-2h) — small UI + metadata
- #328 — Custom CSS in Admin (2-3h) — text editor in admin panel

**Support:**
- #334 — Choose DB plugin during install (UX portion) — installer flows

**Sync point:** Coordinate with River on installer UX for #334. Both Tier 1 and Tier 2 must complete before Tier 3 (plugin ecosystem) activates.

### 2026-05-01 — v0.8 Tier 1 Clarification Blocker: #350 Forced Password Reset UX

---
**[SCRIBE UPDATE - 2026-05-01 00:04:00Z — Simon #319 E2E Blocker: Public Post Route Fix]**
- **ASSIGNED:** Fix published post public URLs (CreatePostTests.CanCreateSaveAndPublishPost blocker)
- **Test Context:** E2E test in `/tests/SharpSite.E2E/Tests/CreatePostTests.cs`
- **Failure Point:** Post creation/save/publish flow works; live-site assertion fails with 404
- **Root Cause:** Route not mapped or URL generation logic missing for published post public access
- **Expected Fix:** Add/fix post publication route handler + ensure URL generation in UI resolves correctly
- **Scope:** Isolated route/URL generation fix; minimal scope, critical path impact
- **Validation:** Wash will rerun E2E test once route is fixed
- **Priority:** CRITICAL — unblocks E2E suite acceptance for v0.8.0

---

---

**[SCRIBE UPDATE - 2026-05-01 00:06:00Z — Simon Public Post Route Fix COMPLETED]**
- ✅ Fixed /post/{slug} public route blocker (#319)
- **Root Cause:** SSR page treated "not loaded yet" as "not found", triggering premature 404
- **Solution:** Thin route page + shared rendering component + explicit load-complete gate before PageNotFound
- **Validation:** Live post URL returns 200 | E2E test CreatePostTests.CanCreateSaveAndPublishPost PASSES
- **Impact:** Issue #319 first execution wave blocker RESOLVED; v0.8.0 Tier 1 unblocked
- **Next:** Wash revalidates full E2E suite against fixed route

---

**Status:** BLOCKED pending user clarification  
**Issue:** #350 (Forced password reset after initial admin seed) lacks UX flow definition.  
**Critical Gap:** Should password reset happen immediately on first login, or allow N logins before enforcing?  

**Impact on Simon:**
- Cannot scope #350 UI effort until UX flow is defined (3–4h estimate depends on scope)
- Cannot begin UI spike until redirect timing and warning placement are clarified

**Next Action:** User provides UX flow requirements; Mal updates #350 with acceptance criteria. Simon can then scope UI implementation.

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### Public Post SSR Routing (Issue #319 blocker) — 2026-05-01
- `DisplayPost`-style public SSR pages must not treat `null` model state as "not found" during the first render. With async repository lookups, Blazor can render once before the awaited load finishes, so a premature `PageNotFound` sets HTTP 404 even when the content exists.
- For SharpSite public content pages, keep an explicit load-complete flag and only render `PageNotFound` after the async fetch has finished. A dedicated `/post/{slug}` route component can stay thin and delegate to a shared page component for lookup/rendering.

### Setup Wizard + Site Assets (Issues #322, #320, #321, #323) — 2026-05-02
- The startup wizard now has a natural four-step flow: site name → site identity assets → database setup → first admin account. Step transitions are stored in `ApplicationState.StartupStep`, so saving state between steps matters.
- `ApplicationState` is the durable source of truth for site branding (`HasCustomLogo`, `HasCustomFavicon`), while the actual files live behind `/api/files/{filename}`. UI can stay simple if it only updates the stored filename and lets layout components resolve URLs.
- A single reusable Blazor asset component works well for both setup and admin maintenance as long as it owns preview/upload/remove behavior and parents own persistence.

### Forced Password Reset (Issue #350) — 2026-03-31
- ASP.NET Core Identity user claims (AspNetUserClaims table) are included in the auth cookie automatically by `SignInManager.PasswordSignInAsync`. This makes claims ideal for request-level checks without hitting the DB.
- The `MustChangePassword` feature uses a claim-based approach: add claim on seed → check in Login.razor + middleware → remove claim + RefreshSignInAsync after password change.
- Account pages under `Account/Pages/` inherit `@attribute [ExcludeFromInteractiveRouting]` from _Imports.razor, making them SSR-only. The `Account/_Imports.razor` imports `Account.Shared` namespace, giving access to `StatusMessage` component.
- `IdentityRedirectManager.RedirectTo()` throws a `NavigationException` handled by Blazor SSR framework as a redirect — it's `[DoesNotReturn]`.
- The admin seed happens in `RegisterPostgresSecurityServices.ConfigureHttpApp()`, called from `StartApi.cs` — not during `Program.cs` startup directly.
- E2E tests (Playwright) require a running Aspire stack and fail locally without it — this is expected.
- **Cross-agent coordination:** River's #346–#349 security fixes are parallel, non-blocking work on the plugin system. No dependencies on this feature.

---
**[SCRIBE UPDATE - 2026-04-30 20:15:51]**
- Decision inbox merged (15 decisions)
- v0.8.0 scope locked and confirmed
- Cross-agent context synchronized
- Ready for parallel execution

---
**[SCRIBE UPDATE - 2026-05-01 00:02:00Z — Kaylee Trash Backend Unit Tests Completed]**
- Kaylee completed repository-level unit test harness for trash/recycle bin flows (#309/#299)
- Tests validate: soft delete, restore, permanent purge, trashed-content exclusion
- All test projects passing (SharpSite.Tests.Web ✅, SharpSite.Tests.Plugins ✅)
- **CRITICAL FOR API ENDPOINTS:** `PgPageRepository.AddPage()` returns the caller's original `Page` instance, NOT the tracked `PgPage`. Generated database `Id` is NOT populated on the returned object. Any caller needing the persisted key MUST re-read after insertion.
- **Action for Simon:** When implementing delete/restore API endpoints, apply ID re-read pattern after AddPage calls to ensure generated keys are available for return payloads.
- River trash backend (#309/#299) contracts ready for integration into API endpoints
