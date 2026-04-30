# Project Context

## 2026-04-30 — v0.8 Milestone Role Assignment

**Role:** Plugin Ecosystem Lead (v0.8.0 → v0.8.x)  
**Effort:** 35–40 hours  
**Timeline:** v0.8.0 (6 weeks), v0.8.1–0.8.3 (4–6 weeks)  

**Tier 1 (concurrent):**
- #309 — Delete a page (4-6h) — data layer + API + UI
- #299 — Delete a post (2-3h) — similar scope

**Tier 3 (after Tier 1-2):**
- #166 — Plugin packager tool (8-10h) — CLI to build .sspkg files
- #336 — Upload plugins without install (4-6h) — staging area
- #334 — Choose DB plugin during install (6-8h) — installer backend + setup
- #254 — Move Postgres to plugin (6-8h) — refactor + migration tests

**Key risks:** Plugin packager complexity (mitigate: spike week 1), DB plugin refactor data loss risk (dry-run first).

**Sync point:** v0.8.0 ship decision gates all Tier 3 work. Coordinate with Simon (installer UX) and Zoe (CI).

### 2026-05-01 — v0.8 Tier 2 Backend Work Queued: Delete Pages/Posts Clarification Pending

**Status:** QUEUED pending Tier 1 completion + clarification  
**Issues:** #309 (Delete page), #299 (Delete post)  
**Effort:** 6–9 hours combined (Tier 1 Phase 2 work)  
**Critical Gap:** Delete semantics undefined — hard delete or soft (archive with recovery)?  

**Impact on River:**
- Cannot scope #309/#299 schema/data layer changes until delete semantics clarified
- Hard delete is simpler (2–3h); soft delete with archive complicates schema by 2–3x (4–6h)

**Dependencies:**
- Tier 1 completion (Wash's #319, #351, #272 + Simon's #350) gates start
- User clarification on delete semantics required (Q5 in Tier 2 phase)

**Next Action:** User provides delete semantics; Mal updates #309/#299 with acceptance criteria. River can then scope schema/API work post-Tier 1.

### 2026-04-30 — River Trash Delete Backend Contract Documented (Proposed)

**Status:** Proposed  
**Issues:** #309 (Delete page), #299 (Delete post)

**Backend Contract Finalized:**
- Pattern: Soft delete (archive + recovery) via `ITrashableContent` + repository extensions
- Methods: `GetDeleted*()`, `Restore*()`, `PermanentlyDelete*()`
- Normal queries exclude trashed records by default
- Deterministic behavior: delete → soft archive; restore → un-archive; purge → permanent removal

**Schema Impact:**
- Soft delete requires `IsDeleted` flag + optional `DeletedAt` timestamp on pages/posts tables
- Complexity: ~4–6 hours for dual-repository implementation + migration tests
- Risk mitigation: Dry-run migration on test DB first

**Simon UI Contract:**
- Delete button: calls `DeletePage(int id)` / `DeletePost(string slug)` (soft delete)
- Trash UI (future): reads `GetDeletedPages()` / `GetDeletedPosts()`
- Restore actions: calls `RestorePage(int id)` / `RestorePost(string slug)`
- Purge actions: calls `PermanentlyDeletePage(int id)` / `PermanentlyDeletePost(string slug)`

**Impact on River:**
- Ready to spike schema + migration design once Tier 1 completes
- Test infrastructure (trash/restore patterns) ready; Kaylee can define test cases early
- Unblocks Simon's future Trash UI work (backend contract clear)

**Next:** Await Tier 1 completion + #309/#299 scope activation; coordinate with Kaylee on test harness design.



- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-05-03 — Trash Delete Pattern for Pages and Posts

Pages and posts now share a trash-state contract via `ITrashableContent` plus `TrashableContentExtensions` in `SharpSite.Abstractions`. The delete flow is soft delete only: repository `Delete*` methods mark `IsDeleted = true`, stamp `DeletedAt`, and all normal `GetPage/GetPages/GetPost/GetPosts` queries exclude trashed content by default.

Both `SharpSite.Data.Postgres` and `SharpSite.Plugins.Data.Postgres` expose the same recovery contract: `GetDeleted*`, `Restore*`, and `PermanentlyDelete*`. EF migrations `20260503101500_AddTrashStateToPagesAndPosts` in both projects add the shared `IsDeleted`/`DeletedAt` columns needed for recycle-bin behavior.

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

---
**[SCRIBE UPDATE - 2026-04-30 20:15:51]**
- Decision inbox merged (15 decisions)
- v0.8.0 scope locked and confirmed
- Cross-agent context synchronized
- Ready for parallel execution
