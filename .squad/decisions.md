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

### User Directive: Use gh CLI for GitHub Operations (2026-04-30)
**By:** Copilot  
**Decision:** Use the `gh` command line tool for GitHub operations in squad workflows.

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

### v0.8 GitHub Milestone Assignment (2026-04-30) ✅ COMPLETED
**Status:** Completed  
**Owner:** Mal  
**Decision:** Triaged v0.7 milestone on GitHub and applied v0.8 scope per strategic roadmap using `gh` CLI.

**Actions:**
- **Closed:** #331, #348 (marked as completed in v0.7)
- **Removed from v0.7:** 7 stale issues deferred to backlog (#334, #312, #298, #292, #289, #218, #161)
- **Assigned to v0.8:**
  - **Tier 1 (Ship-blockers):** #350, #351, #319, #272
  - **Tier 2 (Core CMS):** #309, #299, #321, #323, #328

**Final State:**
- v0.7 Milestone: 2 issues (both closed: #293, #167)
- v0.8 Milestone: 21 issues (9 Tier 1–2 committed + 12 Tier 3+ ecosystem already present)

**Verification:**
```
gh issue list --repo csharpfritz/SharpSite --milestone "0.7 - Installation and Startup" --state all
→ 2 issues total (both CLOSED)

gh issue list --repo csharpfritz/SharpSite --milestone "0.8 - The Version after this" --state all
→ 21 issues total
```

**Reference:** `.squad/decisions/inbox/mal-v08-milestone-github.md` (archived to history)

### v0.8 Milestone Clarification Plan (2026-05-01)
**Status:** Proposed (awaiting user answers)  
**Owner:** Mal  
**Decision:** Analyzed v0.8 milestone (21 issues, 3 tiers) and identified implementation detail gaps blocking sprint work. Created 7-question clarification framework prioritized by shipping impact.

**Tier 1 Ship-Blockers (4 questions, ~12–16h):**
1. **#351 E2E Validation** — Which test scenarios = "done"? (AppHost health + page load + workflows?)
2. **#350 Forced Password Reset** — Trigger on first login or after N warnings?
3. **#319 Flaky Tests** — Can you name 1–2 consistently failing Playwright tests?
4. **#272 Badge Metric** — Show test count, pass %, or coverage %?

**Tier 2 Core CMS (3 questions, ~15–19h):**
5. **#309, #299 Delete Semantics** — Hard delete or soft (archive with recovery)?
6. **#321, #323 Assets** — Display location, size limits, serving strategy (local vs. CDN)?
7. **#328 Custom CSS** — Site-wide or page-scoped? Sanitization rules?

**Tier 3+ (12 issues):** Deferred to v0.8.1–0.8.3 patches; spike #166 (plugin packager) first.

**Cross-Agent Blockers:**
- Wash: Blocked on #351 test checklist (#4 in Tier 1)
- Simon: Blocked on #350 UX flow (#2 in Tier 1)
- River: Tier 2 backend (#309, #299) queued after Tier 1 clarity
- All: v0.8.0 sprint planning ready once Phase 1 (Tier 1) answered

**Deliverable:** `.squad/decisions/inbox/mal-v08-clarification-plan.md`  
**Next Phase:** User provides answers; Mal updates GitHub issues per recommendations table

### User Directive: Upstream Repository as Source of Truth (2026-04-30T15:00:16Z)
**By:** User (via Copilot)  
**Decision:** Use the upstream repository milestone 0.8 (FritzAndFriends/SharpSite) as the canonical source of truth for v0.8 release planning.  
**Impact:** All clarification questions and scope decisions reference upstream issues, not local planning documents.

### Upstream v0.8 Milestone Review (2026-05-02) ✅ COMPLETED
**Status:** Completed analysis; clarification queue prioritized  
**Owner:** Mal  
**Directive:** User requested full review of upstream FritzAndFriends/SharpSite v0.8 milestone using upstream as source of truth.

**Upstream v0.8 Milestone State:**
- **Total open issues:** 18 (vs. 9 anticipated in local docs)
- **Tier 1 (Ship-blockers):** 5 issues — #319 (E2E OOB testing), #272 (Playwright badge), #322 (first admin user), #323 (favicon), #328 (custom CSS)
- **Tier 2 (Core CMS):** 5 issues — #309 (delete page), #299 (delete post), #320 (logo upload), #321 (logo display), #297 (post summary on home)
- **Tier 3+ (Ecosystem):** 8 issues — #313 (CSP header), #208 (admin file browser), #209 (editor file browser), #212 (autosave), #203 (plugin navlinks), #118 (remove plugin), #117 (upgrade plugin), #273 (README badges)

**Scope Mismatch Discovery:**
- **Local plan:** 9 issues (Tier 1: #350, #351 CLOSED upstream; Tier 2: 5 issues)
- **Upstream v0.8:** 18 open issues (all Tier 1–2 present + 11 additional ecosystem items)
- **Impact:** Scope discussion needed to confirm which of 11 new items belong in v0.8.0 (ship) vs v0.8.1+ (patch)

**Critical Gaps Identified (by Tier):**
| Tier | Issue | Gap | Impact |
|------|-------|-----|--------|
| 1 | #319 | No acceptance criteria defined | Wash blocked; unclear pass criteria |
| 1 | #272 | Metric type undefined (count, %, coverage?) | Zoe blocked; CI reporting depends on choice |
| 1 | #322 | Scope unclear (installer vs seed logic?) | Simon blocked; password reset UX depends on answer |
| 1 | #328 | CSS scope & sanitization rules undefined | Simon blocked; security review depends on scope |
| 2 | #309, #299 | Delete semantics undefined (hard vs soft delete?) | River/Simon blocked; schema complexity 2–3x difference |
| 2 | #320, #321 | Logo feature scope unclear (image validation, formats, serving?) | Simon blocked; asset handling complexity |

**Clarification Queue (7 Questions by Priority):**

**PHASE 1 — Tier 1 Blockers (4 questions, unblocks E2E + UI sprint):**

**Q1: E2E Testing Acceptance Criteria (#319)**
- What must E2E validation achieve to be "done"?
  - AppHost health only?
  - AppHost + page load + key workflows (post create, save, publish)?
  - Login + admin UI + content workflows?
- **Impact:** Blocks Wash's sprint; unclear pass criteria
- **GitHub Action:** Update #319 body with acceptance criteria; assign to Wash

**Q2: Playwright Badge Metric (#272)**
- Which metric should the badge display?
  - Test count (e.g., "247 tests")?
  - Pass percentage (e.g., "94% passing")?
  - Coverage percentage?
  - All three?
- **Impact:** Blocks Zoe's CI work; changes reporting logic
- **GitHub Action:** Update #272 with metric choice; assign to Zoe

**Q3: First Admin User Creation (#322)**
- How should the first admin user be created?
  - Installer wizard step?
  - Seed logic with forced password reset on first login?
  - Both?
- **Impact:** Blocks Simon's UI work; affects installer flow
- **GitHub Action:** Clarify #322 scope; link to #350 if dependency

**Q4: Custom CSS Scope & Sanitization (#328)**
- Which CSS scope is needed for v0.8.0?
  - Site-wide only (global stylesheet)?
  - Per-page customization?
  - Both?
  - Sanitization: Allow all CSS or restrict XSS-prone properties?
- **Impact:** Blocks Simon's UI work; affects security review
- **GitHub Action:** Update #328 with scope; assign to Simon

**PHASE 2 — Tier 2 Core CMS (3 questions, depends on Phase 1):**

**Q5: Delete Semantics (#309, #299)**
- For delete page/post, which strategy?
  - Hard delete (purge immediately)?
  - Soft delete (archive, with recovery)?
  - Both (admin can choose)?
- **Impact:** Schema complexity; River backend design depends on answer
- **GitHub Action:** Update #309 + #299 bodies with semantics choice

**Q6: Logo Upload & Display (#320, #321)**
- Logo feature scope for v0.8.0?
  - Upload existing images only?
  - Or include image validation + serving strategy?
  - Supported formats (JPG, PNG, SVG)?
- **Impact:** Image handling complexity; affects #320 + #321 effort
- **GitHub Action:** Update #320 + #321 with feature boundary

**Q7: Tier 3 Deferral Decision**
- For v0.8.0, which belong in scope?
  - #313 (CSP): Ship-blocker or v0.8.1?
  - #208, #209 (File browser): v0.8.0 or v0.8.1?
  - #212 (Autosave): v0.8.0 or v0.8.1?
  - #203, #117, #118 (Plugin lifecycle): v0.8.0 or post-v0.8.0?
  - #273 (README badges): v0.8.0 or separate release?
- **Impact:** Determines v0.8.0 vs v0.8.1+ split; resourcing
- **GitHub Action:** Update each issue with milestone assignment (v0.8.0 vs v0.8.1)

**Cross-Team Blockers:**
| Role | Issue | Blocked Until | Impact |
|------|-------|---------------|--------|
| Wash | #319 | Q1 answer | Cannot write acceptance criteria or test plan |
| Simon | #322, #328 | Q2, Q4 answers | Cannot scope admin UI sprint (est. 3–4 issues) |
| River | #309, #299 | Q5 answer | Cannot design delete schema; mid-tier effort shift |
| Zoe | #272 | Q3 answer | Cannot implement badge + reporting |
| All | Tier 2–3 | Phase 1 complete | Cannot plan v0.8.0 → v0.8.1 split |

**Deliverables:**
- `.squad/decisions/inbox/mal-upstream-v08-question-queue.md` (clarification queue, 7 questions)
- `.squad/decisions/inbox/mal-upstream-v08-question.md` (scope mismatch analysis)

**Next Actions:**
1. Present Q1 (E2E acceptance criteria) to user now
2. Upon user answer, update #319 on GitHub + unblock Wash
3. Move through Phase 1 sequentially (Q2, Q3, Q4)
4. After Phase 1 complete, ask Phase 2 questions (#5–#7)
5. After all 7 answered, Mal applies milestone tags and team begins sprint planning

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

