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

### 2026-03-31 — Security: ZIP Bomb & Path Traversal Protection in Plugin Extraction

**Issue:** #347 — `ExtractAndInstallPlugin` had zero safety checks: no size limits, no compression ratio validation, no path traversal prevention. A 42KB ZIP could decompress to petabytes.

**Fix applied:**
- Added `ValidateArchiveSecurity()` method called during `HandleUploadedPlugin` — validates all entries before extraction begins.
- **Max total extracted size:** 100MB (`MaxTotalExtractedSize`).
- **Max single file size:** 50MB (`MaxSingleFileSize`).
- **Compression ratio check:** Rejects entries with ratio > 100:1 (`MaxCompressionRatio`) — catches ZIP bombs that use highly compressible data.
- **Path traversal protection:** Normalizes backslashes to forward slashes, rejects any entry containing `..` in its FullName.
- **Defense-in-depth in ExtractAndInstallPlugin:** Duplicate path traversal check + `Path.GetFullPath()` containment validation ensuring resolved paths stay within `pluginLibFolder` or `pluginWwwRootFolder`.
- All rejections throw `PluginException` with structured logging.
- All 55 unit tests pass including 8 new ZIP security tests.

**Pattern to remember:** For ZIP extraction security, validate entries at upload time (fail fast) AND during extraction (defense-in-depth). Always check: size limits, compression ratios, and path containment. Use `Path.GetFullPath()` + directory prefix checks for containment.

### 2026-03-31 — Thread-Safety Fix: PluginManager & PluginAssemblyManager (Issue #348)

**Issue:** Static `_ServiceDescriptors` (IServiceCollection) and `_ServiceProvider` in `PluginManager` plus the `Dictionary` in `PluginAssemblyManager` were unguarded against concurrent access — race conditions on concurrent plugin loads or config changes.

**Fix applied:**
- **PluginAssemblyManager:** Replaced `Dictionary<string, PluginAssembly>` with `ConcurrentDictionary<string, PluginAssembly>`. Rewrote `AddAssembly` to use `AddOrUpdate` (atomic add-or-replace with old context unload). Rewrote `RemoveAssembly` to use `TryRemove`.
- **PluginManager:** Added `private static readonly object _ServiceLock` and wrapped all `_ServiceDescriptors` mutations and reads in `lock` blocks. Used `Interlocked.Exchange` for `_ServiceProvider` swaps to guarantee atomic visibility. Restructured the async `ConfigurationSectionChanged` handler to use two separate lock blocks (pre-await and post-await) since `lock` cannot span an `await`.
- **ApplicationState:** Changed `Plugins` from `Dictionary<string, PluginManifest>` to `ConcurrentDictionary<string, PluginManifest>`. Simplified `AddPlugin` to use the thread-safe indexer.
- All 67 unit tests pass (including Kaylee's 6 new thread-safety tests: 3 in ConcurrentAccessTests, 3 in ThreadSafetyTests).

**Pattern to remember:** For static mutable collections shared across instances, use `ConcurrentDictionary` for simple key-value stores and `lock` + `Interlocked.Exchange` for `IServiceCollection`/`IServiceProvider` pairs. Never hold a `lock` across an `await` — split into pre-await and post-await lock blocks instead.

### 2026-03-31 — Security P0: Assembly Validation for Plugin Loading (Issue #349, Phase 1)

**Issue:** Plugin DLLs loaded from disk with zero integrity verification — any `.dll` matching the manifest key ran with full app permissions.

**Fix applied:**
- Created `PluginAssemblyValidator` in `SharpSite.Plugins` with three capabilities:
  1. **Assembly name validation:** After loading, verifies `Assembly.GetName().Name` matches the manifest `Id` (case-insensitive). Rejects mismatches with `PluginException`.
  2. **SHA-256 hash verification:** Computes hash of the DLL file. On first install, stores in `plugins/_assembly-hashes.json`. On subsequent loads, verifies hash matches stored value. Detects tampering.
  3. **Hash registry:** Simple JSON file with `{ "manifestId": "sha256hex" }` entries. Thread-safe via lock around file I/O.
- Integrated into `PluginManager.SavePlugin()` (runtime install: store hash + validate name).
- Integrated into `PluginManager.LoadPluginsAtStartup()` (startup: verify hash + validate name, gracefully skip failed plugins).
- Registered `PluginAssemblyValidator` as singleton in DI via `PluginManagerExtensions`.
- Updated all test constructors (3 files) to pass the new validator dependency.
- All 67 unit tests pass. Build is clean.

**Pattern to remember:** For plugin integrity, validate at two points: (1) the file hash before loading bytes into memory, and (2) the assembly metadata after loading. Store hashes on first install and verify on every subsequent load. At startup, catch validation failures per-plugin and `continue` to avoid one bad plugin blocking all others.

**Cross-agent coordination:** Simon's #350 forced password reset is independent; no blocking dependencies.
