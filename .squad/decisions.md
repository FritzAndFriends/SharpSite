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

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
