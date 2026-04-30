# Project Context

## 2026-04-30 — v0.8 Milestone Role Assignment

**Role:** CI/DevOps & Test Reporting Lead (v0.8.0 Tier 1)  
**Effort:** 3–4 hours  
**Timeline:** v0.8.0 (2–3 weeks)  

**Tier 1 (ship-blockers):**
- #272 — Fix Playwright badge/reports in CI (2-3h) — unblock test visibility in README + CI status page

**Support:**
- #319 — E2E pipeline setup (1-2h) — coordinate with Wash on CI workflow

**Sync point:** #272 must complete before v0.8.0 release. Coordinate with Wash (E2E) and Kaylee (test results). No production Docker deployment needed for this release.

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### Aspire 13.2 Dynamic Port Assignment (2026-03-26)
- Aspire 13.2 assigns dynamic ports to project resources, ignoring the project's own `launchSettings.json` ports
- The E2E test pipeline (`build-and-test.ps1`) starts the AppHost in `--testonly=true` mode and polls `http://localhost:5020`
- Without `WithHttpEndpoint(port: 5020)` in the AppHost, the web frontend starts on a random port and the health check loop never succeeds
- The effective timeout was ~10.5 minutes (90 retries × ~7s per retry due to 5s HTTP timeout + 2s sleep), not the intended 3 minutes
- Fix: Added `WithHttpEndpoint(port: 5020, name: "http")` when `testOnly` is true, reduced HTTP timeout to 2s, added stderr capture and diagnostic logging on failure
- The PR is from a fork (`csharpfritz/SharpSite`), so fork-PR limitations (no secrets access) also apply but are not the root cause here

### v0.7 Release Blocker: OpenTelemetry CVE (2026-03-31)
- After merging PR #352 (.NET 10 + Aspire 13.2 upgrade), the build fails with NuGet security warnings-as-errors
- OpenTelemetry.* 1.15.0 has three moderate CVEs (GHSA-g94r-2vxg-569j, GHSA-mr8r-92fq-pj8p, GHSA-q834-8qmm-v933)
- Directory.Packages.props pins 1.15.0 as the "floor required by Aspire 13.2"
- TreatWarningsAsErrors is enabled, so security warnings → build errors → CI fails
- Root cause: Aspire 13.2 dependency chain requires 1.15.0, but that version has known CVEs
- **Action:** River must decide: (a) upgrade OpenTelemetry to newer 1.x if compatible, (b) suppress NU1902 warnings and document CVE risk, or (c) revert .NET 10 upgrade pending upstream fix
- **Release Impact:** v0.7 cannot ship until this is resolved

---
**[SCRIBE UPDATE - 2026-04-30 20:15:51]**
- Decision inbox merged (15 decisions)
- v0.8.0 scope locked and confirmed
- Cross-agent context synchronized
- Ready for parallel execution
