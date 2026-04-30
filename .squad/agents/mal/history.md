# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-26 — Issue Triage Results (6 squad-labeled issues)

**Routed to squads:**
- **River** (Backend): #346 (P0 RCE), #347 (ZIP bomb), #348 (thread safety), #349 (plugin signing)
- **Simon** (Frontend): #350 (forced password reset UI)
- **Wash** (E2E): #351 (.NET 10 validation)

**Priority ordering:**
1. #346: Security P0 RCE via TypeNameHandling.Auto → **blocks production**
2. #347, #349: Security vulnerabilities (plugin extraction, DLL validation) → **blocks production**
3. #348: Thread-safety bug (PluginManager static state) → **critical correctness**
4. #350: Security hardening (forced password reset) → **medium priority**
5. #351: .NET 10 validation → **mostly done, E2E pending**

All issues labeled with `squad:{member}` and triage comments added. Labels created: squad:river, squad:simon, squad:wash.

### 2026-03-26 — Plugin System Architecture Review (spike_DatabasePlugin branch)

- **Branch health: RED** — Build fails with 60 errors. Root cause is type ambiguity between `SharpSite.Abstractions.Security` types and `Microsoft.AspNetCore.Identity` types in `src/SharpSite.Security.Postgres/`. The security abstraction migration is incomplete.
- **Plugin system architecture** is convention-based + attribute-driven. Plugins are `.sspkg` ZIP files loaded via collectible `AssemblyLoadContext`. Services registered via `RegisterPluginAttribute` and reflection scanning in `PluginManager.RegisterWithServiceLocator()`.
- **Critical security finding:** `ApplicationState.cs` uses `TypeNameHandling.Auto` with Newtonsoft.Json — a known RCE deserialization vector (lines 130-134, 212-216). Must be fixed before production.
- **Critical security finding:** No assembly validation, code signing, or integrity checking on plugin DLLs. Any DLL in the plugins directory runs with full app permissions.
- **Plugin extension points:** FileStorage, DataStorage (Config, EfContext, PageRepo, PostRepo), Security (SignIn, UserManager, UserRepo, EmailSender). Mapped in `PluginTypeMapping.cs`.
- **Static mutable state** in `PluginManager` (`_ServiceDescriptors`, `_ServiceProvider`) is a thread safety concern.
- **`IRunAtStartup` interface** is defined but never invoked by the PluginManager — lifecycle hooks are dead code.
- **Postgres plugin `manifest.json` is empty** (0 bytes) — would fail validation through standard plugin loading.
- **Hardcoded default admin** (`admin@localhost` / `Admin123!`) in the Postgres plugin's `RegisterPluginServices.cs`.
- Full analysis written to `.squad/decisions/inbox/mal-plugin-analysis.md`.

### 2026-03-31 — v0.7 Release Readiness Assessment

**NO GO decision. Three critical blockers prevent immediate release:**

1. **Build Failure** — `dotnet build` fails with 16 NuGet errors. OpenTelemetry.* 1.15.0 transitive dependencies have known CVEs; `TreatWarningsAsErrors=true` treats them as errors. Aspire 13.2 pulls these transitives; unresolved until Aspire is updated or CVE warning is suppressed.

2. **Missing #331 Merges** — Out-of-box startup fixes (commits `92b0ede`, `187a620`) are on `fix/issue-331-ootb-startup` branch but NOT in `upstream/v0.7`. Must merge before release.

3. **E2E Validation Incomplete** — #351 (.NET 10 validation) tracked by PR #352. Unit tests pass, but E2E/Playwright/Aspire AppHost validation pending. Cannot ship untested upgrade path.

**Must-do before release:** 
- Resolve OT CVE handling (suppress warning or upgrade Aspire)
- Merge #331 branch into v0.7
- Complete #351 E2E validation

**Branch state:** `upstream/v0.7` = `cb669d3` (PR #352 .NET 10 upgrade). `origin/v0.7` on GitHub still at `4ec8909` (older). Not yet pushed.

### 2026-04-01 — v0.8 Milestone Planning & Issue Triage (73 open issues analyzed)

**Analysis completed.** Reviewed all 73 open upstream issues and created strategic v0.8 milestone plan.

**Key Findings:**
- **P0 Blockers:** 4 security issues (#346-#349) — #346 CLOSED in v0.7; #347-#349 marked complete but open in tracker (recommend close after v0.7 verification)
- **73 total issues:** 10 stale/needs-clarification; 58 viable future work across 7 priority tiers
- **Recommended v0.8 scope:** Tier 1 (ship-blockers: 4 issues, ~12-16h) + Tier 2 (core CMS: 5 issues, ~15-19h) = **~30 hours of focused delivery** by mid-Q2
- **Plugin ecosystem:** Defer to v0.8.1-0.8.3 patch releases (32-42h after Tier 1-2)

**v0.8 Roadmap:**
1. **Tier 1 (Ship-blockers):** Forced password reset (#350), E2E validation (#319, #351), test badge fix (#272) — 2-3 weeks
2. **Tier 2 (Core features):** Delete page/post (#309, #299), logo/favicon (#321, #323), custom CSS (#328) — 3-4 weeks
3. **Tier 3 (Plugin ecosystem):** Plugin packager (#166), upload-without-install (#336), DB plugin selection (#334), Postgres plugin refactor (#254) — 4-6 weeks
4. **Defer to v0.9+:** Accessibility (#133, #171), performance (#62, #217), future plugins (#189, #186), storefront (#120)

**Squad Assignments Proposed:**
- **River:** Plugin ecosystem backend (#166, #336, #334, #254) + core deletes (#309, #299) = 35-40h
- **Simon:** Admin UI (#321, #323, #328) + installer UX = 15-20h
- **Wash:** E2E validation (#319, #351) + test badge CI = 8-10h
- **Kaylee:** Plugin test coverage (#341) + test cases for Tier 2 = 5-7h
- **Zoe:** CI/test badge reporting (#272) = 3-4h
- **Book:** Plugin author docs (#339) = 4-6h

**Stale Issues (10) Identified:** #331, #301, #300, #298, #289, #283, #203, #161, #122, #121 — Need triage/clarification before work.

**Release Timeline:** v0.8.0 (6 weeks) + v0.8.x plugin iteration (4-6 weeks).

**Full plan written to:** `.squad/decisions/inbox/mal-v08-milestone.md`

### 2026-04-30 — v0.8 Milestone Planning Decision Merged

**Decision merged to squad decisions.md.** v0.8 roadmap is now canonical team guidance.

**Cross-team propagation:**
- **River:** High impact — plugin ecosystem lead role activates; 35-40h commitment ahead
- **Simon:** Medium impact — admin UI sprint following Tier 1 completion
- **Wash, Kaylee, Zoe, Book:** Supporting roles assigned; brief in upcoming sprint planning
- **All:** Watch for scope creep on Tier 4 items; Mal holds gating authority

**Stale issue triage pending:** #331, #301, #300, #298, #289, #283, #203, #161, #122, #121

### 2026-04-30 — Milestone Triage Blocker: Issues Disabled

**Attempted:** Triage v0.7 issues and assign v0.8 milestone tags on GitHub.  
**Blocked:** Repository `csharpfritz/SharpSite` has `hasIssuesEnabled: false`. GitHub API refuses all issue/milestone queries.

**Root cause:** Issues are disabled in repo Settings. This prevents:
- Listing milestones or issues by milestone
- Updating issue assignments
- Any issue/PR workflow automation

**Decision:** Escalated to Jeffrey (owner). v0.8 scope identification is complete (Tier 1 & Tier 2 listed in decisions.md); awaiting Issues enablement to apply GitHub milestone tags.

**Next owner action:** Enable Issues in repo Settings, confirm v0.7 milestone exists, then re-trigger Mal triage workflow.

### 2026-05-01 — Milestone Triage Decision Merged to decisions.md

**Decision merged.** Blocker documented in decisions.md with three escalation options:
- **Option A (Recommended):** Owner enables Issues in repo Settings
- **Option B:** Use Discussions if milestones migrated
- **Option C:** Clarify scope if v0.7 already handled

Awaiting owner action to unblock Mal's triage workflow.
