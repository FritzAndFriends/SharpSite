# Squad Decisions

## Active Decisions

### Security P0: Remove TypeNameHandling.Auto RCE Vector (2026-03-26)
**Status:** Pending  
**Owner:** Mal  
**Issue:** `ApplicationState.cs` (lines 130-134, 212-216) uses `TypeNameHandling.Auto` with Newtonsoft.Json — a well-documented Remote Code Execution deserialization vulnerability. Attacker can modify `plugins/applicationState.json` to inject arbitrary type instantiation payloads.  
**Action:** Replace with `System.Text.Json` or implement custom `SerializationBinder` with type whitelist. Estimated effort: 2 hours.  
**Blocker:** Blocks production readiness of plugin system.

### Security P0: Implement Assembly Validation for Plugin Loading (2026-03-26)
**Status:** Pending  
**Owner:** Mal  
**Issue:** Plugin DLLs in the plugins directory are loaded and executed with zero integrity verification, code signing, or assembly name validation. A malicious plugin has unrestricted access to database, filesystem, environment variables, network, and all application services.  
**Action:** Add assembly name validation against manifest ID. Long-term: implement plugin signing certificate chain.  
**Blocker:** Blocks production readiness of plugin system.

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

- **#349** → River: Implement assembly validation & signing for plugin loading (Effort: 4-6 hours Phase 1)
  - Phase 1: Assembly name validation + SHA-256 hash verification
  - Phase 2: Permission manifest sandboxing
  - Phase 3: Code signing certificate chain
  - Blocker: Plugin system production readiness

**Tier 2: Critical Security Issues**
- **#347** → River: ZIP bomb vulnerability in plugin extraction (Effort: 2-3 hours)
  - Action: Add max total size (100MB), per-file cap (50MB), compression ratio check (100:1), path normalization

**Tier 3: Correctness Bugs**
- **#348** → River: Thread-unsafe static service collection in `PluginManager` (Effort: 3-4 hours)
  - Action: Add `lock` or `ReaderWriterLockSlim` around mutations; use `ConcurrentDictionary`

**Tier 4: Security Hardening**
- **#350** → Simon: Forced password reset after initial admin seed (Effort: 3-4 hours)
  - Scope: Add `MustChangePassword` flag, redirect to reset on first login, warn in Production if default creds active
  - Note: Keep `Admin123!` as dev default; mandatory reset in production

**Tier 5: Validation Work**
- **#351** → Wash: .NET 10 + Aspire 13.2 upgrade validation (Effort: 2-3 hours)
  - Status: Build complete (0 errors), unit tests pass (47/47)
  - Pending: E2E/Playwright, Aspire AppHost, CI SDK updates
  - Tracking: PR #352

### User Directive: Admin Seed Credentials (2026-03-26T15:37Z)
**By:** Jeffrey T. Fritz (via Copilot)  
**Decision:** Keep `Admin123!` as the default developer experience. Add mechanism to force password reset after initial install in production mode. Do NOT remove seed credentials — they are a feature, not a bug.

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
