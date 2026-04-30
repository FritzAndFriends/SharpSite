# Squad Decisions

## Active Decisions

### Security P0: Remove TypeNameHandling.Auto RCE Vector (2026-03-26) ✅ COMPLETED
**Status:** Completed  
**Owner:** River  
**Issue:** `ApplicationState.cs` (lines 130-134, 212-216) uses `TypeNameHandling.Auto` with Newtonsoft.Json — a well-documented Remote Code Execution deserialization vulnerability. Attacker can modify `plugins/applicationState.json` to inject arbitrary type instantiation payloads.  
**Resolution:** Replaced all Newtonsoft.Json usage with System.Text.Json. Implemented ConfigurationSectionJsonConverter for safe polymorphic deserialization.  
**Completion:** 2026-03-31T13:47  
**Verification:** Build clean, 47 tests pass, Newtonsoft.Json fully removed.  
**Blocker Status:** CLEARED — Plugin system production readiness unblocked.

### Security P0: ZIP Bomb Protection for Plugin Extraction (2026-03-31) ✅ COMPLETED
**Status:** Completed  
**Owner:** River  
**Issue:** `#347` — Plugin ZIP extraction in `PluginManager.cs` had no safety limits. An attacker could upload a ZIP bomb (42KB compressed → petabytes uncompressed) to exhaust server disk. Path traversal via `../` sequences in entry names was also not explicitly blocked.  
**Resolution:** Added two-layer security validation:
- **Layer 1 (upload-time):** `ValidateArchiveSecurity` validates max total size (100MB), per-file cap (50MB), compression ratio (100:1), and rejects path traversal via `..`
- **Layer 2 (extraction-time):** Defense-in-depth validation during file writes using `Path.GetFullPath()` containment checks
**Completion:** 2026-03-31T13:55  
**Verification:** Build clean, 55 tests pass (8 ZIP security tests included)  
**Blocker Status:** CLEARED — Plugin ZIP extraction production ready.

### Security P0: Implement Assembly Validation for Plugin Loading (2026-03-26) ✅ COMPLETED
**Status:** Completed (Phase 1)  
**Owner:** River  
**Issue:** Plugin DLLs in the plugins directory are loaded and executed with zero integrity verification, code signing, or assembly name validation. A malicious plugin has unrestricted access to database, filesystem, environment variables, network, and all application services.  
**Resolution:** Implemented Phase 1 assembly validation with two checks:
- **Assembly Name Validation** — After loading a plugin DLL, the assembly's `GetName().Name` is verified against the manifest `Id`. A mismatch causes the plugin to be rejected and unloaded.
- **SHA-256 Hash Verification** — Before loading a DLL, its SHA-256 hash is computed. On first install, the hash is stored in `plugins/_assembly-hashes.json`. On every subsequent load (including startup), the hash is verified against the stored value. A mismatch indicates tampering.  
**Completion:** 2026-03-31T14:12  
**Verification:** Build clean, 67 tests pass.  
**Blocker Status:** CLEARED — Phase 1 assembly validation production ready. Phase 2 (permission manifests) and Phase 3 (code signing) are future work.

### Build Stabilization: Fix Type Ambiguities in Security.Postgres (2026-03-26)
**Status:** In Progress  
**Owner:** River  
**Issue:** 60 build errors across `src/SharpSite.Security.Postgres/` due to collision between `SharpSite.Abstractions.Security.*` types and `Microsoft.AspNetCore.Identity.*` types. Secondary: missing type definitions in `src/SharpSite.UI.Security/`.  
**Action:** Resolve ambiguous references via full namespace qualification or selective using imports. Estimated effort: 1-2 days.  
**Dependency:** Must complete before .NET 10 upgrade can be fully validated.

### .NET 10 + Aspire 13.2 Upgrade (2026-03-26)
**Status:** In Progress  
**Owner:** River  
**Progress:** 39 files upgraded, 51 build errors remain (Blazor BL0008 + C# type errors).  
**Dependency:** Blocked on build stabilization.

### Correctness Bug: Thread-unsafe Static Service Collection in PluginManager (2026-03-31) ✅ COMPLETED
**Status:** Completed  
**Owner:** River  
**Issue:** `#348` — Static mutable `IServiceCollection` and `IServiceProvider?` fields in `PluginManager`, and plain `Dictionary` in `PluginAssemblyManager`, shared across all instances with no synchronization. Concurrent plugin loads or configuration changes could corrupt collections mid-enumeration.  
**Resolution:** Three-layer fix:
- **PluginAssemblyManager:** Replaced `Dictionary` with `ConcurrentDictionary`; used atomic `AddOrUpdate` and `TryRemove` operations
- **PluginManager:** Added static `lock` object guarding all `_ServiceDescriptors` mutations and reads; used `Interlocked.Exchange` for atomic `_ServiceProvider` swaps; restructured async `ConfigurationSectionChanged` handler into snapshot-before-await and mutate-after-await phases
- **ApplicationState.Plugins:** Changed from `Dictionary` to `ConcurrentDictionary`
**Completion:** 2026-03-31T14:02  
**Verification:** Build clean (0 errors, 0 warnings), 67 tests pass (including Kaylee's 6 thread-safety tests). E2E 9 failures are pre-existing (require Aspire host).  
**Blocker Status:** CLEARED — Plugin system thread-safety now production-ready.

### Triage Priorities & Routing (2026-03-31)
**Status:** Completed  
**Owner:** Mal  
**Decision:** 6 squad-labeled GitHub issues routed to team members based on expertise, security criticality, and effort priority.

**Team Routing:**
- **River (Backend Dev):** #346, #347, #348, #349 — plugin system security + threading (4 issues)
- **Simon (Frontend Dev):** #350 — forced password reset UX
- **Wash (E2E Tester):** #351 — .NET 10 + Aspire validation

**Tier 1: Production Blockers (Security P0)**
- **#346** → River: RCE via `TypeNameHandling.Auto` in `ApplicationState.cs` (Effort: 2-4 hours)
  - Action: Replace Newtonsoft.Json polymorphic deserialization with System.Text.Json or strict `ISerializationBinder` whitelist
  - Blocker: Plugin system production readiness
  - **Status:** ✅ COMPLETED

- **#349** → River: Implement assembly validation & signing for plugin loading (Effort: 4-6 hours Phase 1)
  - Phase 1: Assembly name validation + SHA-256 hash verification
  - Phase 2: Permission manifest sandboxing
  - Phase 3: Code signing certificate chain
  - Blocker: Plugin system production readiness

**Tier 2: Critical Security Issues**
- **#347** → River: ZIP bomb vulnerability in plugin extraction (Effort: 2-3 hours)
  - Action: Add max total size (100MB), per-file cap (50MB), compression ratio check (100:1), path normalization
  - **Status:** ✅ COMPLETED

**Tier 3: Correctness Bugs**
- **#348** → River: Thread-unsafe static service collection in `PluginManager` (Effort: 3-4 hours)
  - Action: Add `lock` or `ReaderWriterLockSlim` around mutations; use `ConcurrentDictionary`
  - **Status:** ✅ COMPLETED

**Tier 4: Security Hardening**
- **#350** → Simon: Forced password reset after initial admin seed (Effort: 3-4 hours)
  - Scope: Add `MustChangePassword` flag, redirect to reset on first login, warn in Production if default creds active
  - Note: Keep `Admin123!` as dev default; mandatory reset in production

**Tier 5: Validation Work**
- **#351** → Wash: .NET 10 + Aspire 13.2 upgrade validation (Effort: 2-3 hours)
  - Status: Build complete (0 errors), unit tests pass (47/47)
  - Pending: E2E/Playwright, Aspire AppHost, CI SDK updates
  - Tracking: PR #352

### Security Testing Framework: Anticipatory Tests for #346-#348 (2026-03-31) ✅ COMPLETED
**Status:** Completed  
**Owner:** Kaylee (Tester)  
**Decision:** Wrote 21 anticipatory unit test cases across 4 test files to accelerate River's security fixes for Issues #346, #347, #348. Tests are written now; some verify the *fixed* behavior and will pass once implementations land.

**Test Structure:**
- **RCE Serialization (#346):** 7 tests validating System.Text.Json safe deserialization behavior
  - Location: `tests/SharpSite.Tests.Web/ApplicationState/Security/`
- **ZIP Bomb (#347):** 8 tests validating compression ratio limits, max sizes, path traversal rejection
  - Location: `tests/SharpSite.Tests.Web/PluginManager/Security/`
  - **Status:** Already passing with River's implementation
- **Thread Safety (#348):** 6 tests verifying concurrent access patterns
  - Location: Split across `Tests.Plugins` (PluginAssemblyManager) and `Tests.Web` (PluginManager, ApplicationState)

**Completion:** 2026-03-31T13:55  
**Verification:** Build clean, 55 tests pass (ZIP bomb tests already validated)  
**Cross-Agent Impact:** Tests remain pending River's completion of #346 and #348 fixes.



### User Directive: Admin Seed Credentials (2026-03-26T15:37Z)
**By:** Jeffrey T. Fritz (via Copilot)  
**Decision:** Keep `Admin123!` as the default developer experience. Add mechanism to force password reset after initial install in production mode. Do NOT remove seed credentials — they are a feature, not a bug.

### .NET 10 + Aspire 13.2 Package Versions (2026-03-26)
**Status:** Completed  
**Author:** River  
**Context:** Upgraded the entire SharpSite solution from .NET 9 / Aspire 9.1 to .NET 10 / Aspire 13.2.
**Decisions:**
| Package Family | Version | Rationale |
|---|---|---|
| Aspire.Hosting.*, Aspire.Npgsql.* | 13.2.0 | Target Aspire version |
| Aspire.AppHost.Sdk | 13.2.0 | Must match Aspire packages |
| Microsoft.EntityFrameworkCore.* | 10.0.5 | Floor required by Aspire 13.2 transitives |
| Microsoft.Extensions.Hosting.* | 10.0.5 | Floor required by Aspire 13.2 transitives |
| Microsoft.Extensions.Caching.Memory | 10.0.5 | Floor required by Aspire 13.2 transitives |
| Microsoft.Extensions.ServiceDiscovery | 10.4.0 | NOT an Aspire package; 13.2.0 doesn't exist |
| Microsoft.Extensions.Http.Resilience | 10.4.0 | Latest stable for .NET 10 |
| OpenTelemetry.* | 1.15.0 | Fixes CVE in 1.11.x; floor required by Aspire 13.2 |
| Newtonsoft.Json | 13.0.4 | ~~Floor required by Aspire 13.2~~ REMOVED in #346 |
| System.Text.Json | 10.0.0 | Kept in central management, removed from csproj (pruned) |
| ASP.NET Core packages | 10.0.0 | Standard .NET 10 RTM |

**TargetFramework:** Moved `<TargetFramework>net10.0</TargetFramework>` to `Directory.Build.props`. Future upgrades require one-line change.

**Packages Pruned (removed from csproj):**
- `Microsoft.Extensions.Localization` — part of .NET 10 shared framework
- `Microsoft.Extensions.Caching.Memory` — part of .NET 10 shared framework
- `System.Text.Json` — part of .NET 10 shared framework

### Pin Aspire Port for E2E Test Mode (2026-03-26)
**Status:** Completed  
**Author:** Zoe (CI/DevOps)  
**PR:** #352  
**Context:** Aspire 13.2 assigns dynamic ports; CI hangs waiting for port 5020.
**Decision:** 
- Add `WithHttpEndpoint(port: 5020, name: "http")` in AppHost when `testOnly` is true
- Improve `build-and-test.ps1` with stderr capture, progress logging, diagnostics
- Reduce HTTP health-check timeout from 5s to 2s
**Impact:** CI no longer hangs; failures produce diagnostic output; no production impact

### v0.8 Milestone Plan (2026-04-30) 🎯 PROPOSED
**Status:** Proposed  
**Owner:** Mal  
**Decision:** After analyzing 73 open GitHub issues, recommended a 2-phase v0.8 release strategy:
- **Phase 1 (v0.8.0):** 6 weeks — Tier 1 (ship-blockers: #350, #351, #319, #272) + Tier 2 (core CMS: #309, #299, #321, #323, #328) = ~50–60 hours
- **Phase 2 (v0.8.1–0.8.3):** 4–6 weeks — Tier 3 (plugin ecosystem: #166, #336, #334, #254, #341, #339) = ~30–40 hours
- **Defer to v0.9+:** Accessibility (#133, #171, #267), performance (#62, #217, #218), future plugins, storefront

**Squad Assignments:**
- **River (Backend):** Plugin ecosystem lead (#166, #336, #334, #254) + core deletes (#309, #299) = 35–40h
- **Simon (Frontend):** Admin UI lead (#321, #323, #328) + installer UX = 15–20h  
- **Wash (E2E):** Validation lead (#319, #351, #272) = 8–10h
- **Kaylee (Tester):** Plugin test coverage (#341) + test cases = 5–7h
- **Zoe (CI/DevOps):** Test reporting (#272) = 3–4h
- **Book (Docs):** Plugin author docs (#339) = 4–6h

**Identified Stale Issues (10):** #331, #301, #300, #298, #289, #283, #203, #161, #122, #121 — require triage/clarification before work.

**Key Risks & Mitigations:**
- Plugin packager complexity (#166): River to spike week 1; clarify format early
- DB plugin refactor (#254): Comprehensive migration tests; dry-run first
- E2E flakiness (#319): 2-week buffer; Wash to add retry logic
- Scope creep: Close Tier 4 issues; Mal approval required for exceptions

**Full Plan:** `.squad/decisions/inbox/mal-v08-milestone.md`

### User Directive: Central Package Management (2026-04-30)
**By:** Copilot  
**Decision:** Package version updates should be made in `Directory.Packages.props` (central package management), not in individual project files.

### v0.7 Release Completion (2026-04-30) ✅ COMPLETED
**By:** Jayne (CI/DevOps)  
**Decision:** v0.7 release executed end-to-end:
- Commit: `c35e5e06c5e17b3e0fbada8fb006d30dadcbf5e6` (PR #304 merge to main)
- Tag: Annotated `v0.7` created and pushed
- Release: Public on GitHub Releases with auto-generated changelog (v0.2...v0.7)
- **Key detail:** Used explicit `git push upstream refs/tags/v0.7` (not `git push upstream v0.7`) to avoid ambiguity with branch names

**Notes:** No production Docker deployment yet (code-only release); tag is public and discoverable.

### Commit Movement: Squad GitHub Actions Workflow Deletion (2026-04-30)
**By:** Jayne (CI/DevOps)  
**Decision:** Moved commit `683b828` ("Remove Squad GitHub Actions workflows") from `issue-331-content-root-paths` to `fix/issue-331-ootb-startup` via cherry-pick.
- **Source:** Remains on `issue-331-content-root-paths`
- **Destination:** New commit `eba624e` on `fix/issue-331-ootb-startup`, pushed to origin
- **Files deleted:** 4 Squad workflow files (squad-heartbeat.yml, squad-issue-assign.yml, squad-triage.yml, sync-squad-labels.yml)
- **Method:** Safe cherry-pick preserves remote history

### River — PR #354 Package Upgrades (2026-04-30)
**By:** River (Backend Dev)  
**Branch:** `issue-331-content-root-paths`  
**Decision:** Upgrade OpenTelemetry packages to resolve NuGet vulnerability findings in PR #354:
- `OpenTelemetry.Api` → 1.15.3 (explicit transitive pin; fixes GHSA-g94r-2vxg-569j)
- `OpenTelemetry.Exporter.OpenTelemetryProtocol` → 1.15.3 (fixes GHSA-mr8r-92fq-pj8p, GHSA-q834-8qmm-v933)
- `OpenTelemetry.Extensions.Hosting` → 1.15.3
- `OpenTelemetry.Instrumentation.AspNetCore` → 1.15.2 (1.15.3 not available on NuGet)
- `OpenTelemetry.Instrumentation.Http` → 1.15.1 (1.15.2+ not available)
- `OpenTelemetry.Instrumentation.Runtime` → 1.15.1 (1.15.2+ not available)

**Rationale:** Satisfies patched vulnerability floors while respecting actual NuGet availability; keeps change surgical.

**Validation:** Solution builds clean; unit tests pass; vulnerability audit passes; respects Aspire 13.2/.NET 10 alignment.

### Zoe — PR #354 CI Findings (2026-04-30)
**By:** Zoe (CI/DevOps)  
**PR:** #354  
**Decision:** Diagnosed CI failures in `.NET Build` and `Playwright Tests` jobs:
- **Root cause:** NuGet vulnerability warnings (OpenTelemetry 1.15.0 CVEs) promoted to errors via `TreatWarningsAsErrors=true` in Directory.Packages.props
- **Failing advisories:**
  - `GHSA-g94r-2vxg-569j` (OpenTelemetry.Api 1.15.0)
  - `GHSA-mr8r-92fq-pj8p` (OpenTelemetry.Exporter.OpenTelemetryProtocol 1.15.0)
  - `GHSA-q834-8qmm-v933` (OpenTelemetry.Exporter.OpenTelemetryProtocol 1.15.0)

**Fix:** Upgrade packages per River's decision (above). No CI workflow changes required.

**Note:** Do **not** set all OpenTelemetry packages to 1.15.3; NuGet lacks 1.15.3 for Instrumentation.* packages. The validated safe set (1.15.3 for Api/Exporter, 1.15.2 for AspNetCore, 1.15.1 for Http/Runtime) clears all vulnerabilities.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

