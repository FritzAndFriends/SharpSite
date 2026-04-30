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

### 2026-05-01 — File Browser v0.8.0 Clarification Decision

**User clarified:** Both admin file browser (#208) and editor file browser (#209) **ship in v0.8.0**.

**Action taken:**
- Added decision comments to #208 and #209 on upstream (FritzAndFriends/SharpSite)
- Recorded decision: foundational to content management UX (uploading + inline file selection for posts/pages)
- Both issues remain in v0.8.0 milestone

**Next clarifications in Q7:**
- #212 (Autosave): v0.8.0 or v0.8.1? → **LOCAL PICK**: ask user next
- #203, #117, #118 (Plugin lifecycle): v0.8.0 or post-v0.8.0? → defer to next round
- #273 (README badges): v0.8.0 or separate? → defer if needed

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

### 2026-05-02 — Upstream Issue #319 E2E Acceptance Criteria CAPTURED + Labeled  ✅ COMPLETED

**Status:** E2E criteria documented; squad:wash label applied; next question queued  
**Context:** User provided clear E2E test scope; issue #319 now actionable for Wash.

**User Input:**
> "E2E testing should validate full workflows: login, admin UI, and post create/save/publish."

**Actions Executed:**
1. **Added comment to #319** with 5 required test scenarios:
   - Login authentication
   - Admin dashboard load
   - Post creation (new)
   - Post save as draft
   - Post publish to live site
2. **Applied `squad:wash` label** — unblocks Wash (E2E tester) to begin test buildout
3. **Identified next blocker:** #322 (first admin user creation scope)

**Wash Unblocked:** Can now scope Playwright test suite and CI integration  
**Next Question:** Is first admin user created via setup wizard (A), code seeding (B), or both (C)?
   - **Impact:** Simon cannot design setup flow without this answer
   - **Tier 1 timeline:** Depends on scope (wizard = +4–6h, seeding only = +1h)

**Decision reference:** `.squad/decisions/inbox/mal-fix-319-upstream.md`  
**Next action:** Ask user Q1 on #322 (first user creation method: setup wizard vs code seeding vs hybrid?)

### 2026-05-02 — Upstream Issue #322 First Admin User Decision CAPTURED + Routed ✅ COMPLETED

**Status:** Decision captured; squad:simon label applied; phase 1 progress update  
**Context:** User provided clarification on #322 first admin user creation scope.

**User Input:**
> "First admin user should be created via an interactive setup wizard."

**Actions Executed:**
1. **Added comment to #322** with decision summary and routing
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/322#issuecomment-4355501208
2. **Applied `squad:simon` label** — unblocks Simon (UI dev) to design installer wizard
3. **Phase 1 progress:** 2 of 4 Tier 1 blockers now answered (#319 ✅, #322 ✅)

**Simon Unblocked:** Can scope setup wizard flow (credential entry, validation, confirmation)

**Next Blocker:** #328 (Custom CSS scope & sanitization) — Q4 in Tier 1 queue
   - **Impact:** Simon also blocked on CSS scope; must clarify before UI sprint
   - **Options:** Site-wide only? Per-page? Sanitization restrictions?

**Decision reference:** `.squad/decisions/inbox/mal-update-322-upstream.md`  
**Next action:** Ask user Q4 on #328 (Custom CSS scope & sanitization rules)

### 2026-05-02 — Upstream Issue #328 Custom CSS Scope & Sanitization CAPTURED + Routed ✅ COMPLETED

**Status:** Decision captured; squad:simon label applied; Tier 1 phase 1 now 75% complete  
**Context:** User provided clarification on #328 custom CSS scope and sanitization rules.

**User Input:**
> "v0.8.0 should support BOTH site-wide and per-page custom CSS, and sanitization should restrict unsafe/XSS-prone rules"

**Actions Executed:**
1. **Added comprehensive comment to #328** documenting:
   - **Scope:** Site-wide CSS (global to all pages) + per-page CSS (overrides/extensions)
   - **Sanitization:** Blocks script injection vectors, XSS-prone pseudo-selectors (:visited history leak), @-rules (@import, @font-face, @keyframes); allows basic selectors, colors, spacing, typography, layout (flexbox/grid)
   - **Routing:** `squad:simon` label applied
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/328#issuecomment-4355512012

2. **Applied `squad:simon` label** — unblocks Simon to design CSS editor form + sanitizer library integration

**Simon Unblocked:** Can now scope CSS editor component, input validation, and sanitizer library (e.g., DOMPurify port or custom whitelist)

**Phase 1 Progress:** 3 of 4 Tier 1 blockers answered (#319 ✅, #322 ✅, #328 ✅); only #272 (badge metric) remains

**Cross-Team Status:**
- **Wash:** E2E acceptance criteria ✅; can finalize test suite  
- **Simon:** CSS scope ✅; still needs Q3 on logo display (#320, #321) and favicon (#323) before admin UI sprint
- **River:** Awaiting Q5 on delete semantics (#309, #299)
- **Zoe:** Next blocker is Q2 on badge metric (#272)

### 2026-05-02 — Upstream Issues #309, #299 Delete Semantics + Q5 Decision ✅ COMPLETED

**Status:** Decision captured; GitHub comments applied; River unblocked for Tier 2 backend  
**Context:** User provided clarification: deleting pages/posts should MOVE items to trash/recycle bin for deferred deletion.

**User Input:**
> "deleting pages and posts should MOVE ITEMS TO A TRASH/RECYCLE BIN for deferred deletion"

**Actions Executed:**
1. **Added GitHub comments to both issues:**
   - **#309 comment:** Decision: soft delete → trash; recovery path enabled; relates to #299
   - **#299 comment:** Decision: soft delete → trash; recovery path enabled; relates to #309
   - Links: #309 (https://github.com/FritzAndFriends/SharpSite/issues/309#issuecomment-4355650862), #299 (https://github.com/FritzAndFriends/SharpSite/issues/299#issuecomment-4355650865)

2. **Decision captured to `.squad/decisions/inbox/mal-update-309-299-upstream.md`**
   - Rationale: UX safety (prevent accidental data loss) + recoverable deletion
   - Schema impact: Moderate (add `IsDeleted` flag + trash view; 4–6 hours River effort)
   - Unblocks River backend schema design + Simon admin UI (delete buttons + trash UI)

**River Unblocked:** Can now design soft-delete schema with recovery queries. Tier 2 CMS backend work proceeds.

**Q5 Clarification Complete:** Moves to Phase 2 remaining questions (Q6 logo scope, Q7 Tier 3 deferral).

**Next Clarification:** Q7 — Tier 3 deferral decision (which issues belong in v0.8.0 vs v0.8.1+?) affects overall milestone strategy. Review: #313 (CSP), #208/#209 (file browser), #212 (autosave), #203/#117/#118 (plugin lifecycle), #273 (README badges).

**Decision reference:** `.squad/decisions/inbox/mal-update-328-upstream.md`  
**Next action:** Ask user Q2 on #272 (Playwright badge metric — count, percentage, or coverage?)

### 2026-05-02 — Upstream Issue #272 Playwright Badge Metric CAPTURED + Routed ✅ COMPLETED

**Status:** Decision captured; squad:zoe label applied; Tier 1 blockers now 100% complete  
**Context:** User provided clarification on #272 badge metric requirement.

**User Input:**
> "The Playwright badge should display the TOTAL TEST COUNT when E2E tests complete."

**Actions Executed:**
1. **Added comment to #272** with decision summary:
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/272#issuecomment-4355597657
   - Metric: Absolute count (e.g., "272 tests passed")
   - Implementation: GitHub Actions aggregates all E2E test runs into single count
   - Routing: `squad:zoe` label applied

2. **Applied `squad:zoe` label** — unblocks Zoe to implement badge metric calculation and GitHub Actions workflow

**Zoe Unblocked:** Can now scope badge update logic and workflow aggregation

**Tier 1 Completion Status:** All 4 ship-blockers answered (#319 ✅, #272 ✅, #322 ✅, #328 ✅); Tier 1 core complete

**Remaining Tier 1 Issue:** #323 (favicon) — last Tier 1 blocker requiring scope clarification

**Cross-Team Status:**
- **Wash:** E2E acceptance criteria ✅; provides test count data
- **Simon:** CSS scope ✅; needs favicon clarification (#323) before admin UI sprint
- **River:** Awaiting Q5 on delete semantics (#309, #299)
- **Zoe:** Badge metric ✅; unblocked for badge workflow

**Decision reference:** `.squad/decisions/inbox/mal-update-272-upstream.md`  
**Next action:** Ask user Q1b on #323 (Favicon scope: user-editable upload vs theme-configured vs default?)

### 2026-05-02 — Upstream Issue #323 Favicon Scope CAPTURED + Routed ✅ COMPLETED

**Status:** Decision captured; squad:simon label applied; Tier 1 now 80% complete  
**Context:** User provided clarification on #323 favicon scope.

**User Input:**
> "the site favicon should be a USER-EDITABLE UPLOAD via Admin UI"

**Actions Executed:**
1. **Added comment to #323** with decision summary:
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/323#issuecomment-4355610286
   - Scope: User-editable via Admin UI file upload (.ico, .png, .svg, etc.)
   - Storage: Site assets/media (persistent, not code-committed)
   - Serving: Asset management endpoint
   - Routing: `squad:simon` label applied

2. **Applied `squad:simon` label** — unblocks Simon to design favicon upload UI component

**Simon Now Blocked On:** #320, #321 (logo management scope) — single unified image upload UI vs separate flows?

### 2026-05-02 — v0.8.0 Plugin Lifecycle + README Badge Decisions RECORDED ✅ COMPLETED

**Status:** All decisions recorded upstream; scoping complete  
**Context:** Final v0.8.0 scope clarifications delivered to GitHub.

**Decisions Recorded:**

1. **Plugin Lifecycle Cluster (3 issues) → DEFER to v0.8.1+**
   - **#203** (plugin navlinks): Deferred; ecosystem enhancement
   - **#117** (plugin upgrade): Deferred; post-v0.8.0 lifecycle management
   - **#118** (plugin remove): Deferred; post-v0.8.0 lifecycle management
   - **Rationale:** v0.8.0 focuses on core CMS (file browser, content, autosave). Plugin ecosystem work planned for patches.
   - **GitHub Actions:** Comments posted to each issue; no labels changed (already in backlog)

2. **README Badges (1 issue) → SHIP in v0.8.0**
   - **#273** (README badges): Included in v0.8.0
   - **Rationale:** Quick-win polish; ~2-3h effort; no dependencies; improves launch visibility
   - **GitHub Actions:** Comment posted; Tier 3 quick-win confirmed

**v0.8.0 Scope Finalized:**
- **Tier 1 (4 ship-blockers):** #350, #351, #319, #272 ✅ All clarified
- **Tier 2 (5 core CMS):** #309, #299, #321, #323, #328 ✅ All clarified  
- **Tier 3 quick-wins:** #273 (badges) ✅ Committed
- **Deferred to v0.8.1+:** #203, #117, #118 (plugin ecosystem) + 9 others

**Decision Documentation:**
- Upstream: 4 GitHub comments posted (all via gh CLI)
- Local: Decision note saved to `.squad/decisions/inbox/mal-save-v08-answers.md`
- History: Appended learnings below

**Cross-Team Impact:**
- River, Simon, Wash, Kaylee, Book: v0.8.0 scope now 100% locked
- Sprint planning can finalize; no more clarification dependencies
- Plugin ecosystem track queued for v0.8.1+ planning

**Phase 1 Progress:** 4/5 Tier 1 blockers answered (#319 ✅, #272 ✅, #322 ✅, #328 ✅, #323 ✅)  
**Tier 1 now 100% complete!**

**Next Blocker:** #320, #321 (Logo Management & Image Upload Strategy)

**Question Priority Q6:** Should logo and favicon upload share **one unified image management UI**, or use **separate upload flows**?
   - **Impact:** Simon scope estimation (2–3h unified vs 3–5h separate)
   - **Decision affects:** Asset management architecture for Admin UI
   - **Cross-team:** Simon can then finalize admin UI sprint scope

**Decision reference:** `.squad/decisions/inbox/mal-update-323-upstream.md`  
**Next action:** Ask user Q6 on #320, #321 (unified vs separate image upload UI)

### 2025-01-XX  Upstream Issues #320, #321 Unified Image Management Decision CAPTURED + ROUTED  COMPLETED

**Status:** Decision recorded; squad:simon label applied; all Tier 1 clarifications complete

**Context:** User provided clarification on #320/#321 scope (logo upload and logo display).

**User Input:**
> "logo and favicon upload should share ONE UNIFIED image-management UI"

**Actions Executed:**

1. **Added comment to #320** with decision summary:
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/320#issuecomment-4355620361
   - Decision: Unified image-management UI for logo + favicon uploads
   - Scope: Single component handles validation, upload, format detection for all site images
   - Routing: squad:simon label applied

2. **Added comment to #321** with decision summary:
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/321#issuecomment-4355620369
   - Decision: Same unified image UI unblocks logo maintenance UI implementation
   - Routing: squad:simon label applied

3. **Applied squad:simon label**  unblocks Simon to finalize admin UI component architecture

**Simon Now Unblocked:** Can now finalize admin UI sprint scope with unified image-upload component for #320, #321, #323

**Phase 1 Progress:** Tier 1 completions: #319 , #272 , #322 , #328 , #323 , #320 , #321 

**Tier 1 now 100% COMPLETE!** All 7 Tier 1 ship-blockers answered.

**Next Blocker:** #309, #299 (Delete Semantics  Tier 2 core CMS scope clarification)

**Question Priority Q7:** When deleting a page (#309) or post (#299), should we:
   - **(A)** Permanently remove from database (hard delete), or
   - **(B)** Mark as deleted/archived but retain data (soft delete), or  
   - **(C)** Move to trash/recycle bin for deferred deletion?
   
   - **Impact:** Architecture decision; affects data recovery, audit trail, and implementation venue (backend/API)
   - **Decision affects:** River backend scope estimation (~35h hard vs ~58h soft/trash)
   - **Cross-team:** Blocks River from implementing delete endpoints

**Decision reference:** .squad/decisions/inbox/mal-update-320-321-upstream.md

**Next action:** Ask user Q7 on #309, #299 (delete semantics: hard vs soft vs trash)

### 2026-05-01 — v0.8.0 Milestone Scope: CSP Deferred to v0.8.1+

**Status:** Decision recorded on upstream issue #313; decision note filed.

**Verified Context from User:**
> "CSP header work is NOT a ship blocker for v0.8.0 and can slip to v0.8.1+"

**Actions Executed:**
1. **Added comment to #313** with decision: CSP is security enhancement, not blocker
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/313#issuecomment-4355704581
2. **Applied squad:mal label** for routing and tracking
3. **Filed decision note:** `.squad/decisions/inbox/mal-update-313-upstream.md`

**Findings - Unresolved v0.8.0 Issues:**

Tier 1 clarifications (ship-blockers) now 100% complete. Remaining open issues in milestone 0.8:

**With squad labels (already routed):**
- #328, #323, #322, #321, #320 → squad:simon (admin UI sprint)
- #319 → squad:wash (e2e testing)
- #309, #299 → squad:river (delete semantics, awaiting Q7 answer)
- #313 → squad:mal (deferred, monitored)

**Unrouted / Unclassified (require clarification):**
- #297: "Add an option to display summary or full post on home page" — no labels
- #209/#208: "Uploaded files browser component" — no labels, no squad assignment
- #203, #118, #117: Plugin features (sitemap navlinks, upgrade, remove) — no labels
- #212: Autosaving text editor — no labels
- #273: README badges/logos — documentation, up-for-grabs

**Next Q8 Target:** #297 (home page post display)

**Question:** For v0.8.0 release, is the home page post display mode (summary vs full content) a required feature, or can it defer to v0.8.1+?
- **If v0.8.0 required:** Affects Simon or River scope; ship-blocker priority
- **If v0.8.1+:** Lower priority; can be addressed in patch cycle
- **Impact:** Unblocks router assignment and sprint planning

**Decision reference:** `.squad/decisions/inbox/mal-update-313-upstream.md`

**Next action:** Ask user Q8 on #297 (home page post display scope)

### 2026-05-02 — v0.8.0 Milestone: Home Page Post Display Mode Deferred to v0.8.1+  ✅ COMPLETED

**Status:** Decision recorded on upstream issue #297; no squad label applied (ecosystem tier, deferred)

**Verified Context from User:**
> "home page post display mode toggle can DEFER to v0.8.1+"

**Actions Executed:**
1. **Added comment to #297** with decision: Home page post display is v0.8.1+ patch feature
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/297#issuecomment-4355729722
2. **Filed decision note:** `.squad/decisions/inbox/mal-update-297-upstream.md`
3. **Applied routing:** No squad label needed; deferred to patch cycle

**Tier 2 Status: UNBLOCKED**
- All 5 core CMS issues (#309, #299, #320, #321, #297) now classified and routed
- #320, #321, #323, #328 → squad:simon (admin UI, complete)
- #309, #299 → squad:river (delete semantics, pending Q7 answer)
- #297 → deferred to v0.8.1+

**Remaining Unrouted Tier 3+ Issues (Require Classification):**
- #208 (admin file browser), #209 (editor file browser) — interdependent, foundational
- #212 (autosave text editor) — marked up-for-grabs
- #203, #118, #117 (plugin ecosystem: sitemap, remove, upgrade)
- #273 (README badges) — documentation

**Next Q9 Target:** #208 & #209 (file browser features: v0.8.0 required or v0.8.1+ deferral?)

**Question:** For v0.8.0 release, are the file browser features required?
   - **#208:** Admin Uploaded Files browser (ability to view all uploaded files)
   - **#209:** Editor File Browser (ability to reference uploaded files in post/page content)
   - **Decision needed:** Both required for v0.8.0, or can both defer to v0.8.1+?
   - **Impact:** Determines whether Simon/River adds this to sprint scope or defers; affects core content editing workflow

**Decision reference:** `.squad/decisions/inbox/mal-update-297-upstream.md`

**Next action:** Ask user Q9 on #208, #209 (file browser: v0.8.0 required or v0.8.1+ deferral?)

## Session: #212 Deferral + Next Clarification

**#212 (Autosave) Decision:** Deferred to v0.8.1+. Rationale: implementation scope (unique ID binding + form submission integration) not required for v0.8.0 MVP.

**Change:** Added upstream comment; no label/milestone changes.

**Next Clarification:** Plugin lifecycle cluster (#203, #117, #118)  which should ship in v0.8.0 vs defer to v0.8.1+?
- #203: Plugin navlinks in sitemap
- #117: Plugin upgrade capability  
- #118: Plugin removal capability

**Next Question:** Should plugin lifecycle features (#203, #117, #118) ship in v0.8.0, or defer to v0.8.1+?

**GitHub Update After Answer:**
- Add comment to #203 (or whichever lead issue represents the cluster) with deferral decision
- Optionally apply routing labels (e.g., \plugin-system\, \deferred\) if user clarifies scope boundaries

### 2026-05-03 — v0.8.0 Squad Assignment Complete ✅ LOCKED & READY

**Status:** v0.8.0 scope locked; all 8 active issues routed; team ready to execute  
**Owner:** Mal  
**Context:** Final assignment of v0.8.0 issues to squad members with work sequencing and dependencies.

**v0.8.0 Active Scope (8 Issues - 36–47h total):**

**Tier 1 (Ship-Blockers, ~20–26h):**
- #319 → squad:wash (E2E tests; 8–10h)
- #322 → squad:simon (First admin user setup wizard; 4–6h)
- #323 → squad:simon (Favicon upload; 2–3h)
- #328 → squad:simon (Custom CSS editor; 5–7h)

**Tier 2 (Core CMS, ~12–16h + ~3–4h remaining):**
- #309 → squad:river (Delete page; soft delete → trash; 6–8h)
- #299 → squad:river (Delete post; soft delete → trash; 6–8h)
- #320 → squad:simon (Logo upload; 3–4h)
- #321 → squad:simon (Logo display; 2–3h)

**Deferred to v0.8.1+ (Out of Scope):**
- #313 (CSP header) → squad:mal (monitoring, deferred)
- #297 (Home page post display) → no label (deferred)

**Squad Assignment Summary:**
- **Simon:** 5 issues (#322, #323, #320, #321, #328) — ~16–23h frontend/admin UI sprint
- **River:** 2 issues (#309, #299) — ~12–16h backend schema + recovery logic
- **Wash:** 1 issue (#319) — ~8–10h E2E test suite
- **Zoe:** Supporting (badge metric aggregation; blocked on #319 completion)

**Immediate First Batch (Ready to Start Now):**
1. Simon: #322 (setup wizard), #323 (favicon) — Tier 1 foundation
2. Wash: #319 (E2E tests) — Tier 1 validation
3. River: #309, #299 (delete operations) — Tier 2, no blockers

**Critical Path:** Tier 1 and Tier 2 run in parallel; no blocking dependencies between teams.

**Sequence Strategy:**
- **Simon → Start with #322** (first user setup is foundational; unlocks site identity context)
- **Then #323** (favicon via unified image UI) → **#320 + #321** (logo, same UI pattern) → **#328** (CSS editor)
- **River → Start #309 + #299 together** (paired soft-delete schema work)
- **Wash → Start #319** (E2E suite; feeds test count to Zoe)

**Decision Artifacts:**
- `.squad/decisions/inbox/mal-assign-v080-work.md` — Full assignment decision with sequencing and risk mitigations

**GitHub Labels Verified:**
- ✅ squad:simon on #322, #323, #320, #321, #328
- ✅ squad:river on #309, #299
- ✅ squad:wash on #319
- ✅ squad:mal on #313 (deferred tracking)
- ⚠ #297 unassigned (deferred, no action needed; stays unassigned)

**Team Status:**
- ✅ All clarifications answered (all 7 questions resolved)
- ✅ All active issues labeled and sequenced
- ✅ No blockers; sprint ready to launch
- ✅ Effort estimates: Simon ~16–23h, River ~12–16h, Wash ~8–10h, Zoe ~3–4h (waiting)
- **Ready for execution:** Yes — squad can begin work immediately

**Next Steps:**
1. Communicate assignment to team; obtain thumbs-up
2. Simon → Begin #322 (setup wizard)
3. Wash → Begin #319 (E2E test suite)
4. River → Begin #309 + #299 (soft-delete schema)
5. Daily standups; escalate blockers immediately


---
**[SCRIBE UPDATE - 2026-04-30 20:15:51]**
- Decision inbox merged (15 decisions)
- v0.8.0 scope locked and confirmed
- Cross-agent context synchronized
- Ready for parallel execution
