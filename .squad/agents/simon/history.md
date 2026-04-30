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
