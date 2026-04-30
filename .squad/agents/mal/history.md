# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite  a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Core Context

**v0.8.0 Sprint (CURRENT - Locked & Ready)**
- **Status:**  Locked scope; all clarifications answered; squad ready for execution
- **8 Active Issues:** 3647h total (Tier 1: ship-blockers; Tier 2: core CMS)
- **Squad Assignments:**
  - Simon: #322 (setup), #323 (favicon), #320 (logo), #321 (logo-display), #328 (CSS)  5 issues, 1623h
  - River: #309 (page-delete), #299 (post-delete)  2 issues, 1216h
  - Wash: #319 (E2E tests)  1 issue, 810h
  - Zoe: Supporting (badges)  34h (waiting)
- **First Batch Ready:** #322, #323, #319, #309, #299 (zero blockers)
- **Critical Path:** Parallel execution; Tier 1 + Tier 2 concurrent
- **Execution:** Ready to launch now

**Clarifications Resolved (v0.8 Planning)**
- #208/#209 (file browser): Both ship in v0.8.0 
- #212 (autosave): Deferred to v0.8.1+ (scope creep) 
- #203/#117/#118 (plugin lifecycle): Deferred to v0.8.1+ 
- #313 (CSP header): Deferred to v0.8.1+ 
- #297 (home post display): Deferred 

**v0.7 Completion & v0.8 Planning (Pre-May 2026)**
- Triaged 73 open issues; identified 8 core items for v0.8
- v0.7 blockers resolved: Security P0s (#346 RCE, #347 ZIP bomb, #348 threading) + build stabilization
- Plugin architecture assessed; three security gaps addressed in v0.7 fixes
- GitHub Issues re-enabled by owner (was disabled, blocking triage)
- v0.8 milestone applied to 8 active issues; v0.7 stragglers triaged/deferred

## Recent Decisions

*All decisions merged to .squad/decisions.md (canonical source)*

**2026-05-03:** v0.8.0 squad assignment locked. 8 issues routed per expertise; sequencing confirmed; no blockers.

**2026-05-01:** File browser clarification (#208/#209); plugin lifecycle deferral (#203/#117/#118); autosave deferral (#212).

**2026-04-30:** GitHub issues enabled; milestone triage complete; 10 v0.7 items deferred to backlog.

---

**[SCRIBE UPDATE - 2026-04-30 20:18:02]**
- Decision inbox merged (15 decisions into decisions.md)
- v0.8.0 scope confirmed locked
- Cross-agent context synchronized
- Ready for parallel execution
- Agent history files updated across team
