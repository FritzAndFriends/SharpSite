# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Core Context

**Pre-April 2026 Activity (Summarized from v0.7 → v0.8 transition phase)**

- **Issue Triage** (2026-03-26): 6 squad-labeled issues routed based on expertise. P0 security: #346 (RCE), #347 (ZIP bomb), #348 (thread safety), #349 (plugin signing). UI: #350 (password reset). E2E: #351 (.NET 10 validation).
- **v0.7 Blocker Analysis** (2026-03-31): Three critical blockers prevented release: build failure (OT CVE), missing #331 merges, incomplete #351 E2E validation. v0.7 eventually shipped after resolution.
- **v0.8 Strategic Planning** (2026-04-01): Analyzed 73 open issues; recommended Tier 1 (4 ship-blockers, ~12–16h) + Tier 2 (5 core CMS, ~15–19h) + Tier 3+ (ecosystem, deferred to patches). Squad assignments defined per expertise.
- **Plugin Architecture Insights**: Convention-based + attribute-driven loading; critical security gaps identified (RCE, no assembly validation). Covered in security fixes completed by River in v0.7.

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

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

### 2026-04-30 — v0.8 Milestone GitHub Triage ✅ COMPLETED

**Status:** Completed  
**Trigger:** Issues enabled on csharpfritz/SharpSite; owner action unblocked triage.

**Actions Executed:**
1. **Triaged 0.7 milestone (14 issues):**
   - **Closed:** #293, #167 (already closed; verified)
   - **Removed from 0.7:** 10 issues deferred to backlog/future (#334, #331, #312, #298, #292, #289, #218, #161, #122, #121)

2. **Applied 0.8 milestone:**
   - **Added Tier 1 (4 issues):** #350, #351, #319, #272 (ship-blockers; ~12–16h)
   - **Added Tier 2 (5 issues):** #309, #299, #321, #323, #328 (core CMS; ~15–19h)
   - **Tier 3+ ecosystem (12 issues):** Already on 0.8; deferred to v0.8.1–0.8.3 patches

**Final State:**
- **v0.7 Milestone:** 2 issues (both closed; milestone can be archived)
- **v0.8 Milestone:** 21 issues (9 Tier 1–2 committed; 12 Tier 3+ support)
- **Backlog (deferred):** 10 issues removed from any milestone; no urgency

**Decision Rationale:**
- **Tier 1–2 scope:** 30–35 hours of focused delivery; achievable in 6 weeks per original roadmap
- **Tier 3+ ecosystem:** 30–40 hours; deferred to patch releases after v0.8.0 ships
- **Deferred 10 issues:** Unclear scope, performance-only, or require upstream clarification; revisit after Tier 1–2

**Cross-Team Impact:**
- River: Can now scope Tier 2 backend work (#309, #299)
- Simon: Can now scope Tier 2 frontend work (#321, #323, #328)
- Wash: E2E validation (#319) + CI fix (#272) part of Tier 1
- All: v0.8.0 sprint planning can begin immediately

**Decision reference:** `.squad/decisions.md` (merged from inbox)

### 2026-05-01 — v0.8 Milestone Clarification Analysis ✅ COMPLETED

**Status:** Completed  
**Context:** Repository has Issues disabled; cannot fetch issue bodies via GitHub API. Analysis based on Tier 1–2 roadmap from decisions.md + inferred requirements.

**Actions Executed:**
1. **Reviewed all 21 issues in v0.8 milestone** (9 Tier 1–2 committed; 12 Tier 3+ deferred)
2. **Identified information gaps for each issue:**
   - **Tier 1 gaps:** UX flow for #350, test checklist for #351, flaky test names for #319, badge metric for #272
   - **Tier 2 gaps:** Delete semantics (#309, #299), asset display location (#321, #323), CSS scope (#328)
   - **Tier 3+ gaps:** Plugin packager format (#166), DB schema migration (#254), selection UI (#334)
3. **Created 7-question clarification plan** prioritized by shipping impact
4. **Prepared GitHub update recommendations** for each issue once user answers

**Gaps Summary by Priority:**
| Tier | Issue | Critical Gap | Impact |
|------|-------|--------------|--------|
| 1 | #350 | UX flow for forced password reset | Blocks Simon (UI); 3-4h effort depends on scope |
| 1 | #351 | E2E test checklist (what constitutes "done"?) | Blocks Wash; unclear pass criteria |
| 1 | #319 | Flaky test names; root cause unknown | Wash spike depends on reproducible failures |
| 1 | #272 | Badge metric (count/%, coverage) | Zoe implementation depends on metric choice |
| 2 | #309, #299 | Delete semantics (hard/soft/archive) | Changes schema complexity by 2–3x if archive required |
| 2 | #321, #323 | Asset serving strategy (local vs. CDN) | Affects implementation venue (frontend/backend) |
| 2 | #328 | CSS scope + sanitization rules | Security decision needed before code review |
| 3+ | #166 | Plugin packager format (ZIP vs. custom) | Blocker for entire plugin ecosystem track |

**Questioning Strategy:**
- **Phase 1 (Now):** Q1–Q4 on Tier 1 → unblocks E2E + password reset sprint
- **Phase 2 (After Q1–Q4):** Q5–Q7 on Tier 2 → unblocks admin UI sprint
- **Phase 3 (Post v0.8.0):** Spike #166 plugin packager before Tier 3 work

**Deliverable:** `.squad/decisions/inbox/mal-v08-clarification-plan.md`

**Next Action:** Present single best first question to user (Q1 on #351 E2E checklist)

### 2026-05-01 — v0.8 Clarification Decision Merged ✅ DECISION ARCHIVED

**Status:** Merged to canonical decisions.md  
**Context:** Scribe merged `.squad/decisions/inbox/mal-v08-clarification-plan.md` into decisions.md; inbox file deleted.

**Actions:**
1. Archived decision to decisions.md section: "### v0.8 Milestone Clarification Plan (2026-05-01)"
2. Documented cross-agent blockers:
   - Wash: Blocked on #351 test checklist (Q1)
   - Simon: Blocked on #350 UX flow (Q2)
   - River: Tier 2 backend (#309, #299) queued after Tier 1
3. Updated squad routing: Answers to Q1–Q4 unblock Tier 1 sprint planning

**Cross-Team Ready:**
- v0.8.0 sprint planning can begin once user answers Phase 1 (Tier 1) clarifications
- River can scope Phase 2 Tier 2 backend work after Phase 1 answers received
- Simon can begin UI spike on #350 immediately (pending UX flow definition)

**Status:** ✅ Decision canonical — awaiting user clarification phase input
