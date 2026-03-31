# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-26 — Security Context from Mal's Analysis

River should be aware of two critical P0 security issues identified on spike_DatabasePlugin branch:

1. **RCE via Insecure Deserialization** — `ApplicationState.cs` uses `TypeNameHandling.Auto` with Newtonsoft.Json. Attackers can modify `plugins/applicationState.json` to instantiate arbitrary types. **Fix:** Replace with `System.Text.Json` or implement custom whitelist-based `SerializationBinder`.

2. **No Assembly Validation** — Plugin DLLs loaded with zero integrity/signing checks. Any DLL in plugins directory runs with full app permissions. **Fix:** Validate assembly name against manifest ID at minimum.

These are blocking issues for production readiness. While .NET 10 upgrade is in progress, ensure P0 fixes are addressed in parallel or immediately after build stabilization.

Reference: `.squad/decisions/inbox/mal-plugin-analysis.md` (now merged to decisions.md)

### 2026-03-26 — .NET 10 Build Fix: SharpSite.Security.Postgres

Fixed remaining build error in `RegisterPostgresSecurityServices.cs:33`. The `SharpSite.Abstractions.Security.IEmailSender` interface is **non-generic** (it bakes in `ISharpSiteUser` in its method signatures), so DI registration must use `AddScoped<AbsSecurity.IEmailSender, PgEmailSender>()` — not the generic `IEmailSender<ISharpSiteUser>` form. The PgUserManager nullable reference (CS8603) and IdentityError mapping (CS1503) errors were already resolved prior to this fix.

### 2026-03-26 — .NET 10 + Aspire 13.2 Upgrade

**Completed the full .NET 10 + Aspire 13.2 upgrade.** Key learnings:

- **TargetFramework centralized**: Moved from per-csproj `<TargetFramework>net9.0</TargetFramework>` to a single entry in `Directory.Build.props`. All 19 csproj files now inherit it.
- **C# 14 breaks interface conversions**: `explicit operator` to/from interfaces is banned. Replaced with `FromInterface()` static methods on `PgSharpSiteUser`.
- **Type ambiguity in .NET 10**: `Microsoft.AspNetCore.Identity` types (`SignInResult`, `IdentityResult`, `AuthenticationScheme`) now conflict with `SharpSite.Abstractions.Security` custom types. Fixed using `using Abstractions = SharpSite.Abstractions.Security;` aliasing throughout Security.Postgres files.
- **Aspire 13.2 requires higher package floors**: EF Core 10.0.5, Extensions.Hosting 10.0.5, OpenTelemetry 1.15.0 minimum. The `Microsoft.Extensions.ServiceDiscovery` package is NOT an Aspire package (max version is 10.4.0, not 13.2.0).
- **Package pruning (NU1510)**: .NET 10 flags `Microsoft.Extensions.Localization`, `Microsoft.Extensions.Caching.Memory`, and `System.Text.Json` as unnecessary direct references — they're in the shared framework now.
- **Pre-existing issues in UI.Security and plugin**: Both projects have incomplete interfaces (missing methods like `CreateAsync`, `DeleteAsync` on `IUserManager`). Not caused by the upgrade.
- **All 47 unit tests pass on .NET 10.**

### 2026-03-26 — Security P0: RCE Fix — Replaced Newtonsoft.Json TypeNameHandling.Auto with System.Text.Json

**Issue:** #346 — `TypeNameHandling.Auto` in Newtonsoft.Json is a documented RCE deserialization vector. Four usages existed across `ApplicationState.cs` and `SharpsiteConfigurationExtensions.cs`.

**Fix applied:**
- Replaced all Newtonsoft.Json serialization with `System.Text.Json` across `ApplicationState.cs`, `ApplicationStateModel.cs`, `SharpsiteConfigurationExtensions.cs`, and `PluginConfigUI.razor`.
- Created `ConfigurationSectionJsonConverter.cs` — a custom `JsonConverter<ISharpSiteConfigurationSection>` that handles polymorphic serialization safely. It writes a `$type` discriminator but only resolves types that implement `ISharpSiteConfigurationSection` from loaded assemblies. Attackers cannot instantiate arbitrary types.
- The converter handles legacy Newtonsoft-style assembly-qualified type names for backwards compatibility with existing `applicationState.json` files.
- Removed `Newtonsoft.Json` PackageReference from both `SharpSite.Abstractions.csproj` and `SharpSite.Web.csproj`.
- Updated test file `WhenFileExists.cs` to use `System.Text.Json`.
- All 47 unit tests pass. Build is clean.

**Pattern to remember:** For any future polymorphic serialization in the plugin system, use the `ConfigurationSectionJsonConverter` pattern: validate the resolved type implements the expected interface before instantiation. Never allow arbitrary type resolution from JSON input.
