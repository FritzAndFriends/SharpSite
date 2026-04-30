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

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### Forced Password Reset (Issue #350) — 2026-03-31
- ASP.NET Core Identity user claims (AspNetUserClaims table) are included in the auth cookie automatically by `SignInManager.PasswordSignInAsync`. This makes claims ideal for request-level checks without hitting the DB.
- The `MustChangePassword` feature uses a claim-based approach: add claim on seed → check in Login.razor + middleware → remove claim + RefreshSignInAsync after password change.
- Account pages under `Account/Pages/` inherit `@attribute [ExcludeFromInteractiveRouting]` from _Imports.razor, making them SSR-only. The `Account/_Imports.razor` imports `Account.Shared` namespace, giving access to `StatusMessage` component.
- `IdentityRedirectManager.RedirectTo()` throws a `NavigationException` handled by Blazor SSR framework as a redirect — it's `[DoesNotReturn]`.
- The admin seed happens in `RegisterPostgresSecurityServices.ConfigureHttpApp()`, called from `StartApi.cs` — not during `Program.cs` startup directly.
- E2E tests (Playwright) require a running Aspire stack and fail locally without it — this is expected.
- **Cross-agent coordination:** River's #346–#349 security fixes are parallel, non-blocking work on the plugin system. No dependencies on this feature.
