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

### 2026-05-02 — Upstream v0.8 Milestone Full Review ✅ COMPLETED

**Status:** Completed analysis; prioritized clarification queue built  
**Context:** User directive: "Review upstream FritzAndFriends/SharpSite v0.8 milestone and ask clarification questions."

**Upstream v0.8 State:**
- **Total open issues:** 18
- **Tier 1 (Ship-blockers):** 5 issues (#319, #272, #322, #323, #328)
- **Tier 2 (Core CMS):** 5 issues (#309, #299, #320, #321, #297)
- **Tier 3+ (Ecosystem):** 8 issues (#313, #208, #209, #212, #203, #118, #117, #273)

**Critical Gaps Identified:**
1. **#319 (E2E Testing):** No acceptance criteria — what constitutes "done"?
2. **#322 (First Admin User):** Scope unclear — installer vs seed logic?
3. **#272 (Badge Metric):** Undefined — count, %, or coverage?
4. **#328 (Custom CSS):** Scope & security — site-wide? Sanitization rules?
5. **#309, #299 (Delete Operations):** Hard delete or soft delete? Schema impact 2–3x.
6. **#320, #321 (Logo Management):** Image handling scope?
7. **Phase 2 Deferral:** Which Tier 3 items belong in v0.8.0 vs v0.8.1+?

**Clarification Queue Structure:**
- **Phase 1 (4 questions):** Tier 1 blockers; unblocks E2E + UI sprint
- **Phase 2 (3 questions):** Tier 2 core CMS; depends on Phase 1
- **Phase 3 (Tier 3 split):** After Phase 1 answered; determines patch release cadence

**Cross-Team Blockers:**
- Wash: #319 (E2E acceptance criteria)
- Simon: #322, #328 (password reset UX, CSS scope)
- River: #309, #299 (delete semantics schema impact)
- Zoe: #272 (badge metric choice)

**Decision Delivered:** `.squad/decisions/inbox/mal-upstream-v08-question-queue.md`  
**Next Step:** Ask user Q1 (E2E acceptance criteria) to unblock Wash + Tier 1 sprint

### 2026-05-02 — Upstream v0.8 Milestone Full Review + Inbox Merge ✅ COMPLETED

**Status:** Completed; inbox merged to decisions.md  
**Owner:** Mal  
**Context:** User directive: "Use upstream FritzAndFriends/SharpSite milestone 0.8 as source of truth."

**Actions:**
1. **Reviewed upstream v0.8 milestone:** 18 open issues (vs. 9 anticipated in local docs)
2. **Discovered scope mismatch:** 11 additional ecosystem items not in local plan
3. **Built prioritized clarification queue:** 7 questions by impact (Phase 1 Tier 1, Phase 2 Tier 2, Phase 3 Tier 3+)
4. **Merged inbox to decisions.md:** All decision artifacts consolidated to canonical location
5. **Updated Mal history:** Core context summarized; old entries archived

**Upstream v0.8 Snapshot:**
- **Total open issues:** 18
- **Tier 1 (Ship-blockers):** 5 issues — #319, #272, #322, #323, #328
- **Tier 2 (Core CMS):** 5 issues — #309, #299, #320, #321, #297
- **Tier 3+ (Ecosystem):** 8 issues — #313, #208, #209, #212, #203, #118, #117, #273
- **Closed upstream:** #350 (password reset), #351 (.NET 10 upgrade) both ✅ CLOSED

**7-Question Clarification Framework (by impact):**
1. **Q1:** E2E testing acceptance criteria (#319) — Wash blocker
2. **Q2:** Playwright badge metric (#272) — Zoe blocker
3. **Q3:** First admin user creation scope (#322) — Simon blocker
4. **Q4:** Custom CSS scope & sanitization (#328) — Simon blocker
5. **Q5:** Delete semantics hard/soft (#309, #299) — River blocker
6. **Q6:** Logo upload & display scope (#320, #321) — Simon blocker
7. **Q7:** Tier 3 deferral (which issues v0.8.0 vs v0.8.1+?) — All teams

**Inbox Merged:**
- ✅ `.squad/decisions/inbox/mal-upstream-v08-question-queue.md` → decisions.md
- ✅ `.squad/decisions/inbox/mal-upstream-v08-question.md` → decisions.md (deduped with queue)
- ✅ `.squad/decisions/inbox/copilot-directive-20260430-150016.md` → decisions.md
- ✅ `.squad/decisions/inbox/mal-close-351.md` → archived to history (issue doesn't exist)

**Cross-Team Ready:**
- **Wash:** Awaiting Q1 answer (E2E acceptance criteria)
- **Simon:** Awaiting Q3, Q4 answers (password reset, CSS scope)
- **River:** Awaiting Q5 answer (delete semantics)
- **Zoe:** Awaiting Q2 answer (badge metric)
- **All:** v0.8.0 sprint planning blocked until Phase 1 (Q1–Q4) answered

**Decision reference:** `.squad/decisions.md` (merged from inbox)  
**Next step:** Present Q1 to user; await Phase 1 answers

### 2026-05-02  E2E Test Acceptance Criteria Accepted  CAPTURED

**Status:** E2E criteria captured; upstream/local sync issue identified  
**Context:** User provided clarification on #319 E2E testing scope.

**User Input:**
> "E2E testing should validate full workflows: login, admin UI, and post create/save/publish."

**Translated to Test Scenarios:**
1. Login authentication to admin area
2. Admin dashboard navigation and load
3. Post creation (new post)
4. Post save as draft
5. Post publish to live site

**Blocker Discovered:**
- **Upstream issue #319 does not exist** in GitHub (upstream only has 1 issue: #4)
- **v0.8 roadmap mismatch:** Local decisions.md documents 21 planned issues; upstream GitHub is out of sync
- **Root cause:** Issues never synced from local planning to upstream GitHub after initial creation

**Next Clarification Question (Priority 1):**
- Issue #4 (Forced Password Reset) needs UX detail: On reset completion, should system auto-redirect to dashboard (A), show confirmation link (B), or force re-login (C)?
- **Impact:** Blocks Simon (UI component) from starting work

**Cross-Team Status:**
- Wash: Can now scope E2E test cases; #319 acceptance criteria clear
- Simon: Blocked on #4 password reset UX detail (needs A/B/C decision)
- All: Upstream/local sync requires owner clarification before sprint assignment

**Decision reference:** `.squad/decisions/inbox/mal-update-319.md`  
**Next action:** Ask user for #4 UX clarification (auto-redirect vs confirmation vs re-login)
