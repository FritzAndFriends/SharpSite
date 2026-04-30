# Squad Decisions
## Active Decisions
## Governance
## Merged Decisions (2026-04-30)
# v0.8.0 Locked Scope & Squad Work Assignment
## Executive Summary
## Active v0.8.0 Issue Set (8 Issues)
## Deferred to v0.8.1+ (Out of Active Scope)
## Squad Work Assignment
## Immediate First Work Batch (Launch Now)
## Risks & Mitigations
## GitHub Labels Applied
## Checklist
## Next Actions
# v0.8.0 Scope Decision Notes
## Decision Summary
## Impact
## Next Steps
# Decision: README Badges Milestone Assignment (#273)
## Decision
## Impact on Interview Queue
## Next Steps
## Reference
# Decision: #212 Autosave Deferral
## What Changed
## Next Clarification Queue
# Decision: File Browsers Ship in v0.8.0
## Decision
## Rationale
## Changes Made
## Next Clarifications Needed
# Decision: Issue #297 — Home Page Post Display Mode Deferred to v0.8.1+
## Decision
## User Input
## Rationale
## GitHub Actions Executed
## Cross-Team Impact
## Next Phase
## Reference
# Decision: CSP Header Work Deferred to v0.8.1+
## Decision
## Next Steps
# Decision: Soft Delete via Trash/Recycle Bin for Pages & Posts (#309, #299)
## Decision
## Rationale
## GitHub Updates Applied
## Cross-Team Impact
## Next Action
# Decision Note: Unified Image Management UI (#320, #321)
## Date
## Decision
## Rationale
## Changes Made
## Next Steps
## Related
# Decision: #323 Favicon Scope Clarified
## User Input
## Decision Summary
## GitHub Update
## Next Blocker
# Upstream Issue #272: Playwright Badge Metric Decision
## User Decision
## Actions Executed
## Implementation Context
## Cross-Team Impact
## Phase 1 Tier 1 Completion
# Update #328 Custom CSS Scope & Sanitization (2026-05-02)
## Decision Recorded
## User Input
## Actions Executed
## Impact
## Cross-Team Status
## Decision Reference
# Mal — Update #322 Upstream (2026-05-02)
## Decision Captured
## GitHub Actions Executed
## Impact
## Crosslinks
# Decision: Issue #319 E2E Test Acceptance Criteria — CAPTURED
## What Changed on #319
## Next Clarification Question (Priority 1)
## Cross-Team Status
## Next Action
# E2E Test Acceptance Criteria Clarified (2026-05-02)
## User-Provided E2E Acceptance Criteria
## Next Clarification Question
## Cross-Team Unblocking
## Syncing Note


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


- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction





**Date:** 2026-05-03  
**Owner:** Mal (Lead)  
**Status:** ✅ READY FOR EXECUTION  


v0.8.0 scope is **fully locked** with all clarifications answered. 8 active issues routed to squad members with clear dependencies and immediate first batch identified. No blockers remain; sprint can launch now.

---


| # | Title | Tier | Squad | Effort | Status |
|---|-------|------|-------|--------|--------|
| #319 | E2E test suite (login, dashboard, post workflows) | 1 | Wash | 8–10h | Ready |
| #322 | First admin user setup wizard | 1 | Simon | 4–6h | Ready |
| #323 | Favicon upload to Admin UI | 1 | Simon | 2–3h | Ready |
| #328 | Site-wide + per-page custom CSS editor (sanitized) | 1 | Simon | 5–7h | Ready |
| #309 | Delete page (soft delete → trash/recycle bin) | 2 | River | 6–8h | Ready |
| #299 | Delete post (soft delete → trash/recycle bin) | 2 | River | 6–8h | Ready |
| #320 | Logo upload (image formats, validation) | 2 | Simon | 3–4h | Ready |
| #321 | Logo display/maintenance in Admin UI | 2 | Simon | 2–3h | Ready |

**Total Effort:** ~36–47 hours  
**Critical Path:** Tier 1 → Tier 2 (Tier 1 blocks no other work; both can proceed in parallel after clarifications)

---


| # | Title | Rationale |
|---|-------|-----------|
| #313 | CSP header (security enhancement) | Not a ship-blocker; deferred for patch cycle |
| #297 | Home page post display toggle | v0.8.1+ feature; lower priority than core content editing |

---


### Simon (Frontend/Admin UI Lead) — 5 Active Issues

**Issues:** #322, #323, #320, #321, #328

**Delivery:** Unified image-management UI component + setup wizard + CSS editor  
**Effort:** ~16–23h  
**Dependencies:** None (all clarifications locked)

**Sequence (recommended):**
1. **#322** (First admin user setup wizard) — foundational, other features depend on first-user context
2. **#323** (Favicon upload) — leverage unified image UI pattern  
3. **#320** (Logo upload) — part of same unified image UI
4. **#321** (Logo display) — consumes uploaded logo; depends on #320  
5. **#328** (Custom CSS editor) — distinct component; can parallel with images if bandwidth available

**Batch 1 (Start now):**
- #322, #323 — Setup wizard + favicon upload (ship-blockers)

**Batch 2 (After Batch 1 or parallel):**
- #320, #321 — Unified image UI for logo
- #328 — CSS editor (can parallel)

---

### River (Backend Lead) — 2 Active Issues

**Issues:** #309, #299

**Delivery:** Soft-delete schema + trash/recovery queries + cleanup policies  
**Effort:** ~12–16h  
**Dependencies:** None (delete semantics clarified: soft delete → trash/recycle)

**Sequence:**
1. Extend Post/Page models with `IsDeleted` flag + `DeletedAt` timestamp
2. Add trash view queries (retrieves deleted items within retention period)
3. Implement recovery endpoint (un-delete with restore)
4. Implement permanent cleanup (admin can purge old trash)

**Batch 1 (Start now):**
- **#309 + #299** — Both can start immediately; implement together as paired schema work

---

### Wash (E2E Tester) — 1 Active Issue

**Issue:** #319

**Delivery:** E2E test suite covering full workflows  
**Effort:** ~8–10h  
**Dependencies:** None (acceptance criteria locked)

**Test Scenarios (per clarification):**
1. Login authentication to admin area
2. Admin dashboard navigation and load
3. Post creation (new)
4. Post save as draft
5. Post publish to live site

**Batch 1 (Start now):**
- **#319** — Playwright test suite; CI integration for badge metric

---

### Zoe (CI/DevOps) — Supporting Role

**Dependency:** Wash completes #319 E2E tests

**Delivery:** Badge metric aggregation + GitHub Actions workflow  
**Effort:** ~3–4h

**Blocked until:** Wash provides test count data from #319  
**Note:** #272 metric already clarified (total test count, not %)

---


**Parallel Tier 1 (Ship-Blockers):**

1. **Simon:** #322 (First admin user setup) + #323 (Favicon)
   - ~6–9h combined
   - Unblocked; can start immediately
   - Creates foundation for site identity (logo, CSS, initial auth)

2. **Wash:** #319 (E2E test suite)
   - ~8–10h
   - Unblocked; can start immediately  
   - Test scenarios fully defined
   - Provides data to Zoe for badge metric

**Parallel Tier 2 (Can start after OR immediately):**

3. **River:** #309 + #299 (Delete operations)
   - ~12–16h combined
   - No blockers; proceed in parallel with Tier 1
   - Design soft-delete schema while Simon/Wash execute Tier 1

**Critical Path Optimization:**
- Tier 1 + Tier 2 can overlap; no blocking dependencies between teams
- Simon finishes Tier 1 → can start #320, #321 (logo) + #328 (CSS)
- All issues can progress without gating on each other

**Recommended Sprint Cadence:**
- **Week 1:** Tier 1 foundation (#322, #323, #319)
- **Week 1–2:** Tier 2 design + initial implementation (River)
- **Week 2–3:** Tier 2 completion + Tier 1 polish + integration testing

---


| Risk | Likelihood | Mitigation |
|------|------------|-----------|
| Unified image UI complexity (Simon) | Low | Start with MVP (single input), extend if time permits |
| E2E test environment setup (Wash) | Medium | Coordinate with River on AppHost health checks |
| Trash cleanup retention policy (River) | Low | Default 30-day retention; make configurable later |
| CSS sanitizer XSS vectors (Simon) | Medium | Use well-vetted library (e.g., HtmlSanitizer); avoid regex parsing |

---


✅ All squad labels already correct as of this review:
- `squad:simon` → #322, #323, #320, #321, #328
- `squad:river` → #309, #299
- `squad:wash` → #319
- `squad:mal` → #313 (deferred, monitoring)

---


- [x] v0.8.0 scope locked (no outstanding clarifications)
- [x] All 8 active issues routed to squad members
- [x] Dependencies identified and sequenced
- [x] First batch identified and ready
- [x] Effort estimates aligned with team expertise
- [x] Deferred items marked and rationale documented
- [x] GitHub labels verified
- [x] No blockers remain

---


1. **Mal:** Present this assignment to squad + obtain team thumbs-up
2. **Simon, River, Wash:** Begin assigned work per batch recommendations
3. **All:** Daily standups on progress; escalate blockers immediately
4. **Scribe:** Merge inbox decision to squad decisions.md
5. **Ralph (Monitor):** Track v0.8.0 milestone closure on GitHub issues board

---

**Decision Delivered:** `.squad/decisions/inbox/mal-assign-v080-work.md`  
**Scope:** FritzAndFriends/SharpSite v0.8.0 milestone (GitHub source of truth)  
**Team Ready:** ✅ Yes — all clarifications answered, no outstanding questions



**Date:** 2026-05-01  
**Owner:** Mal (Lead)  
**Status:** Committed to upstream GitHub issues


Two issue groups triaged and decisions recorded on GitHub:

### 1. Plugin Lifecycle Cluster — DEFERRED to v0.8.1+

**Issues:** #203, #117, #118  
**Decision:** Defer plugin ecosystem enhancements (navigation links, upgrade, remove) to v0.8.1+  

**Rationale:**
- v0.8.0 scope is core CMS delivery: file browser, content management, autosave
- Plugin lifecycle features are post-v0.8.0 enhancements; no user stories block v0.8.0 ship
- Ecosystem work planned for patch releases after v0.8.0 stabilizes
- Clears capacity for Tier 1 & 2 blockers

**GitHub Actions:**
- #203 comment posted: Navigation links deferred
- #117 comment posted: Upgrade functionality deferred  
- #118 comment posted: Remove functionality deferred
- No label changes (ecosystem already identified in backlog)

### 2. README Badges — SHIP in v0.8.0

**Issue:** #273  
**Decision:** Include README status badges in v0.8.0 release  

**Rationale:**
- Quick-win polish item; ~2-3 hours effort max
- No dependencies on other features
- Improves project visibility + CI status transparency at launch
- Low risk of blockers

**GitHub Actions:**
- #273 comment posted: Badges ship in v0.8.0
- Label routing: None applied (already identified in Tier 3 ecosystem)


**v0.8.0 Scope Unchanged:**
- Tier 1 (4 ship-blockers): #350, #351, #319, #272
- Tier 2 (5 core CMS): #309, #299, #321, #323, #328
- Tier 3 quick-wins: #273 (badges) + others
- Deferred: #203, #117, #118 → v0.8.1+ backlog

**Team Bandwidth:** Reinforces v0.8.0 focus on core CMS, clears plugin ecosystem work for patches.


1. GitHub decisions synced upstream ✅
2. Await squad confirmation on Tier 1 & 2 progress
3. Track v0.8.1+ roadmap (plugin lifecycle, CSP headers, editor enhancements)



**Date:** 2025 (current session)  
**Clarification Question:** Q7 — Which Tier 3+ issues belong in v0.8.0 vs later releases?  
**User Answer:** README badges ship in **v0.8.0**

- README badges (#273) are a **v0.8.0 deliverable** alongside core functionality
- Rationale: Customer-facing documentation, ships with initial release
- Label routing: Not required (already scoped as documentation, up-for-grabs)

- Q7 (Tier 3 deferral) now has its first binding decision
- Remaining Tier 3+ issues (#313, #208, #209, #212, #203, #118, #117) still pending deferral decisions
- Interview queue **NOT complete** — 6 more Tier 3 items need clarification

1. Review remaining Tier 3+ deferral decisions (CSP, file browsers, autosave, plugin lifecycle)
2. Determine if all 8 should ship in v0.8.0 or some defer to v0.8.1+

- Upstream: FritzAndFriends/SharpSite#273
- Clarification plan: `.squad/decisions/inbox/mal-v08-clarification-plan.md`



**Date:** 2025-01-12  
**Issue:** #212 (Enable autosaving of content when editing in TextEditor)  
**Decision:** Defer to v0.8.1+  
**Rationale:** Implementation scope requires unique ID binding + form submission integration. Not a blocking requirement for v0.8.0 MVP.

- Added upstream comment explaining deferral rationale
- No label changes (already marked `enhancement`, `up-for-grabs`)
- Milestone remains `0.8 - The Version after this` (no milestone change)

Remaining unresolved items after #212:
1. **Plugin lifecycle cluster** (#203, #117, #118): v0.8.0 or post-v0.8.0?
2. **README badges** (#273): v0.8.0 or separate release?

**Next Question:** Should plugin navlinks (#203), plugin removal (#118), and plugin upgrade (#117) ship in v0.8.0, or defer to v0.8.1+?



**Date:** 2026-03-31  
**Owner:** Mal  
**Issues:** #208, #209  

Both the admin file browser (#208) and editor file browser (#209) will ship in **v0.8.0**.

- These are foundational for the content management workflow (uploading and selecting files for posts/pages)
- Admin browser provides site-wide file management
- Editor browser enables inline file selection during content editing
- Both features are closely coupled in user intent

- Added decision comment to #208 and #209
- Both issues remain in v0.8.0 milestone

- #212 (Autosave): v0.8.0 or v0.8.1?
- #203, #117, #118 (Plugin ecosystem): scope for v0.8.0?
- #273 (README badges): v0.8.0 or separate?



**Date:** 2026-05-02  
**Owner:** Mal (Lead)  
**Status:** ✅ Recorded  
**Issue:** #297 "Add an option to display summary or full post on home page"


Home page post display mode toggle is **NOT a v0.8.0 ship-blocker** and can defer to v0.8.1+ patch cycle.


> "home page post display mode toggle can DEFER to v0.8.1+"


- Feature is aesthetic/UX enhancement, not core CMS functionality required for v0.8.0 release
- Deferral unblocks v0.8.0 scope to focus on critical ship-blockers (Tier 1) and core CMS features (Tier 2)
- Can be implemented as a straightforward Site Appearance checkbox in follow-up patch


1. **Comment added to #297** — Decision recorded upstream
   - URL: https://github.com/FritzAndFriends/SharpSite/issues/297#issuecomment-4355729722
   - Routing: v0.8.1+ patch cycle (no squad label needed at this stage)


- **Simon:** Unlocked; no admin UI work required for v0.8.0 on this feature
- **River:** Unlocked; no backend API required for v0.8.0 on this feature
- **All:** Tier 2 scope now finalized; all 5 core CMS issues (#309, #299, #320, #321, #297) are routed or deferred


All Tier 1 (ship-blocker) and most Tier 2 clarifications now complete. Remaining unresolved:
- #309, #299: Delete semantics (hard vs soft delete)
- #208, #209: File browser features (admin + editor)

**Next target:** Clarify v0.8.0 vs v0.8.1+ split for remaining Tier 3+ ecosystem issues.


- Issue: FritzAndFriends/SharpSite#297
- Milestone: 0.8 - The Version after this
- Related: .squad/decisions/inbox/mal-update-313-upstream.md (CSP deferral pattern)



**Issue:** #313 — Add CSP to base/default theme to enhance security on site  
**Decision Date:** 2026-05-01  
**Owner:** Mal (Lead)  
**Status:** RECORDED on upstream issue


CSP (Content Security Policy) header implementation is **NOT** a v0.8.0 ship blocker. Work deferred to v0.8.1+ patch release.

### Rationale
- Security enhancement with no impact on core CMS functionality
- Adds defense depth but does not unblock other v0.8.0 deliverables
- Can be implemented post-release without breaking changes
- Aligns with v0.8.0 scope: admin UI, page/post management, plugin ecosystem

### Action Taken
- Commented on #313 with decision rationale
- Applied `squad:mal` routing label for tracking
- No GitHub milestone change needed (remains in 0.8 for future decision point)

- Monitor for any security escalations that would promote this to ship-blocker status
- Schedule CSP design/implementation for v0.8.1 planning cycle



**Date:** 2026-05-02  
**Owner:** Mal  
**Status:** Applied  
**Issues:** #309 (delete page), #299 (delete post)


When a user deletes a page or post, the item should be **moved to a trash/recycle bin** for deferred deletion, not immediately purged. Permanent deletion occurs only when the user explicitly empties the trash.

**Design Implication:** Soft delete with recovery path.


- **User experience:** Prevents accidental permanent data loss
- **Admin capability:** Recoverable deletion provides safety net
- **Schema impact:** Moderate (add `IsDeleted` flag + trash view; River backend effort: 4–6 hours)


- **#309 comment:** Decision recorded with relation to #299
- **#299 comment:** Decision recorded with relation to #309


- **River (Backend):** Unblocked; can design soft-delete schema & queries
- **Simon (Admin UI):** Delete buttons + trash UI component added to scope
- **Effort shift:** Schema complexity moderate; recoverable within Tier 2 CMS estimate


Q6 (Logo feature scope) or Q7 (Tier 3 deferral) to be clarified next per queue priority.



2025-01-XX

Logo and favicon upload will share **ONE UNIFIED image-management UI** rather than separate upload flows.

- **Consistency:** Single image handling pattern across site configuration
- **Scope Reduction:** Reduces frontend complexity vs. separate flows (Simon estimate: 2–3h unified vs 3–5h separate)
- **UX:** Admin users get consistent experience for all image uploads
- **Maintainability:** Fewer edge cases; simpler validation/storage logic

- ✅ Comment added to #320 with decision recorded
- ✅ Comment added to #321 with decision recorded  
- ✅ Label `squad:simon` added to both issues
- ✅ Both issues now scoped as Simon frontend work

- Simon can now estimate and scope admin UI sprint for #320, #321, #323, #328
- Unified image-upload component serves both logo and favicon (#323) requirements
- Awaiting clarification on **delete semantics** (#309, #299) for Tier 2 scope finalization

- Tier 2 Clarifications: #319 ✅, #272 ✅, #322 ✅, #323 ✅, #328 ✅
- Next Clarification: Delete behavior (#309, #299) — soft vs hard delete semantics



**Status:** CAPTURED + ROUTED  
**Issue:** #323 (Set a favicon for the site)  
**Owner:** Mal  
**Date:** 2026-05-02


> "the site favicon should be a USER-EDITABLE UPLOAD via Admin UI"


**Scope:** Favicon is user-editable via Admin UI file upload, not theme-configured or static.

**Implementation Details:**
- Admin UI provides file upload component for favicon (.ico, .png, .svg, etc.)
- Favicon stored in site assets/media (not committed to code)
- Served from Admin UI asset management or dedicated favicon endpoint
- Replaces default favicon on all pages

**Impact:**
- **Simon:** Design favicon upload UI component; integrate with asset management
- **Effort:** 2-3 hours (depends on asset management refactoring)
- **Blocker:** None; can proceed in parallel with #320, #321 (logo/image management)


**Issue:** #323  
**Actions:**
1. Add comment with decision and routing
2. Apply `squad:simon` label (UI component dev)
3. Reference related issues: #320 (logo image types), #321 (logo admin UI)


**Issue:** #320, #321 (Logo Management Scope)

**Question:** Should logo and favicon upload use the **same unified image management UI**, or separate upload flows?
- **Impact:** Simon scope estimation (2-3h single UI vs 3-5h dual UIs)
- **Cross-team:** Affects asset management architecture decision

---

**Tier 1 Progress:** 4/5 issues clarified (#319 ✅, #272 ✅, #322 ✅, #328 ✅)  
**Next blocker:** #320, #321 (logo management) OR #309, #299 (delete semantics)



**Status:** Captured and routed  
**Owner:** Mal  
**Issue:** #272 — Playwright badge and reports not updating properly  
**Blocker:** Zoe (CI/Badge implementation)


> "The Playwright badge should display the TOTAL TEST COUNT when E2E tests complete."


1. **Added comment to #272** documenting decision:
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/272#issuecomment-4355597657
   - Metric: Total test count (not percentage or coverage)
   - Impact: GitHub Actions workflow aggregates all E2E test runs into single count

2. **Applied `squad:zoe` label** — unblocks Zoe to implement badge update logic


- **Metric:** Display absolute count (e.g., "272 tests passed")
- **Trigger:** On E2E test completion in GitHub Actions
- **Aggregation:** Merge multiple test runs (if parallel jobs) into single count via publish-unit-test-result-action
- **Badge source:** Shields.io or similar dynamic badge service


- **Wash:** E2E test suite provides input data (test count)
- **Zoe:** Can now implement badge update workflow
- **CI/Badge:** Unblocked for v0.8.0 sprint


All 4 original Tier 1 blockers now answered:
- Q1: #319 E2E acceptance criteria ✅
- Q2: #272 Badge metric ✅
- Q3: #322 First admin user ✅
- Q4: #328 Custom CSS ✅
- **Remaining Tier 1:** #323 (favicon) — needs scope clarification



**Status:** Scope clarified; upstream #328 updated  
**Owner:** Mal  
**Issue:** #328 (Allow administrators to submit some custom CSS)

> "v0.8.0 should support BOTH site-wide and per-page custom CSS, and sanitization should restrict unsafe/XSS-prone rules"

1. **Added comprehensive comment to #328** documenting:
   - **Scope:** Site-wide CSS + per-page CSS overrides/extensions
   - **Sanitization:** Blocks script injection, XSS-prone pseudo-selectors, @-rules; allows basic selectors, colors, spacing, typography
   - **Routing:** `squad:simon` label applied

2. **Applied `squad:simon` label** — unblocks Simon (UI dev) to design CSS editor form + sanitizer integration

- **Simon Unblocked:** Can now scope CSS editor component, input validation, and sanitizer library integration
- **Tier 1 Progress:** 3 of 4 Tier 1 blockers now answered (#319 ✅, #322 ✅, #328 ✅)
- **Next Blocker:** #272 (Playwright badge metric) — Q2 in phase 1 clarification queue

- **Wash:** Awaiting no further Tier 1 input (E2E acceptance criteria ✅)
- **Simon:** Unblocked on CSS; still needs clarification on logo display scope (#320, #321) and favicon (#323)
- **River:** Awaiting Q5 on delete semantics (#309, #299)
- **Zoe:** Blocked on Q2 (badge metric #272)
- **All:** Phase 1 Tier 1 (60–70% complete); Phase 2 Tier 2 can begin once Q1–Q4 answered

**Link:** https://github.com/FritzAndFriends/SharpSite/issues/328#issuecomment-4355512012



**Status:** Completed  
**Owner:** Mal  
**Issue:** #322 (First Admin User Creation)


**User Input:**
> First admin user should be created via an interactive setup wizard.


1. **Comment added to #322**
   - Link: https://github.com/FritzAndFriends/SharpSite/issues/322#issuecomment-4355501208
   - Content: Decision summary + squad routing

2. **Label applied: `squad:simon`**
   - Unblocks Simon (UI dev) to scope installer wizard


- **Simon:** Can now design setup wizard flow (password entry, confirmation, validation)
- **Tier 1 Clarity:** 3 of 4 Tier 1 blockers answered (#319 E2E ✅, #322 First User ✅, #272 Badge pending, #328 CSS pending)
- **Next Blocker:** #328 (Custom CSS scope & sanitization) — Q4 in clarification queue


- Relates to: #350 (forced password reset)
- Tier 1 sprint: E2E (#319) + Installer (#322) + Badges (#272) + CSS (#328)



**Date:** 2026-05-02  
**Owner:** Mal  
**Status:** Acceptance criteria documented; next clarification queued


**Comment Added:** E2E test acceptance criteria captured with 5 required test scenarios:
1. Login authentication to admin area
2. Admin dashboard navigation and load
3. Post creation (new post)
4. Post save as draft
5. Post publish to live site

**Label Added:** `squad:wash` — unblocks Wash (E2E tester) to scope test buildout

**Impact:**
- Wash: Can now define Playwright test cases and CI integration
- v0.8 Tier 1: E2E test scope is now actionable


**Issue:** #322 (Create first user with Admin access)  
**Gap:** Scope unclear — is first user created during installation/setup (interactive UI), or via database seeding (code-based)?

**Question to User:**
> "When SharpSite is deployed for the first time, how should the first admin user be created? Should the system provide an interactive setup wizard (e.g., form-based admin credentials entry), or is it code-based database seeding with hardcoded defaults?"

**Blockers if unclear:**
- Simon (UI) cannot design admin setup flow without knowing the pattern
- Installation documentation depends on user creation method
- Tier 1 timeline at risk if scope expands

**Expected Answers:**
- **(A) Interactive Setup:** SharpSite shows first-run form → user creates admin via UI → redirects to dashboard
- **(B) Code Seeding:** Default admin user (e.g., `admin@localhost` / `Admin123!`) created at deploy-time; user must change on first login
- **(C) Hybrid:** Setup wizard available; pre-seeded default also available as fallback

**Impact if A:** +4–6h UI work (form, validation, auth integration)  
**Impact if B:** +1h seeding logic; no UI needed  
**Impact if C:** +5–7h (both paths required)


- **Wash:** ✅ Unblocked on #319 (E2E acceptance criteria)
- **Simon:** ⏸️ Awaiting Q1 answer (#322 first user creation)
- **River:** ⏸️ Awaiting Q2 answer (#309/#299 delete semantics — hard/soft/archive)
- **All:** v0.8 Tier 1 sprint planning continues after Q1 answered


Ask user: Q1 on #322 (first user creation: setup wizard vs code seeding vs hybrid?)

Once answered → update #322 + unblock Simon → decide Tier 1 timeline feasibility



**Status:** Accepted — User-provided criteria captured  
**Issue Reference:** #319 (planned in local decisions.md as Tier 1 E2E work)  
**Upstream Status:** Issue #319 does not exist in upstream GitHub; v0.8 milestone absent


> "E2E testing should validate full workflows: login, admin UI, and post create/save/publish."

### Acceptance Criteria Breakdown
1. **Login flow:** Successful authentication to admin area
2. **Admin UI navigation:** Verify admin dashboard loads and is accessible
3. **Post creation:** Create a new post (step 1 of workflow)
4. **Post save:** Save post as draft (step 2 of workflow)
5. **Post publish:** Publish post to live site (step 3 of workflow)

### Test Venues
- **Location:** `tests/SharpSite.Tests.E2E/` (Playwright + .NET Aspire host)
- **Coverage:** Full end-to-end workflow in single session
- **Flakiness flag:** #319 item noted as having flaky tests; investigate common timeouts/race conditions


After E2E acceptance criteria, the single **best next question** for upstream open items is:

**Issue #4 (Forced Password Reset) — Implementation Detail:**
> "On password reset completion in step 4 of the forced-change flow, should the system:
> - A) Auto-redirect to admin dashboard? 
> - B) Show confirmation page with manual "Go to Admin" link?
> - C) Force re-login with new password?"

**Impact:** Blocks Simon (UI component) from determining redirect logic and error handling.


- **Wash:** E2E criteria now clear → can scope test cases and prioritize flaky test fixes
- **Simon:** Waiting on #4 password reset UX detail (A/B/C above) to start component build
- **Mal:** Will update GitHub once upstream issues are created and synced to local roadmap


**Critical:** v0.8 roadmap lives in local `decisions.md` (21 issues planned) but upstream GitHub is out of sync (only #4 exists). Before assigning Tier 1 work to Wash, River, Simon, need to clarify:
- Should all 21 planned issues be created in upstream GitHub?
- Or is upstream a separate, simpler track?

Owner (Jeffrey T. Fritz) should confirm source of truth.



# Simon — Admin Asset UI Implementation (2026-04-30) ✅ COMPLETED
## Status: Completed
## Owner: Simon

### Context
Working #320, #321, and #323 alongside the #322 setup-wizard flow exposed a duplication risk: setup and admin both needed logo/favicon upload, preview, and removal behavior.

### Decision
Use one reusable Blazor asset-management component for both startup and admin maintenance:

- `SiteAssetManager` handles preview, upload, validation, and removal
- parents own persistence by updating `ApplicationState.HasCustomLogo` or `ApplicationState.HasCustomFavicon`
- assets resolve through `/api/files/{filename}` using deterministic names (`site-logo.*`, `site-favicon.*`)

### Why
This keeps the unified image-management decision concrete in code, reduces divergent UX between setup and admin, and makes future asset work (alt text, cropping, theme variants) easier to extend in one place.

### Implementation Status
- ✅ Setup wizard: 4-step flow (site name → assets → database → first admin)
- ✅ Admin UI: Asset management panel in admin dashboard
- ✅ Security: Wizard now seeds first admin (cleaner than separate DB seed)
- ✅ Build: SharpSite.Web passes clean
- ✅ Tests: SharpSite.Tests.Web 57/57 passing

### Impact
- **Wash:** Can now test E2E asset upload scenarios in setup/admin flows
- **River:** Asset API contract stable; trash delete patterns documented for future Trash UI
- **Kaylee:** Test infrastructure ready for asset component coverage

---

# River — Trash Delete Backend Contract (2026-05-03)
## Status: Proposed
## Owner: River

### Decision
Implement page/post delete as a shared trash pattern, not a one-off flag per repository.

### Implementation
- Added `ITrashableContent` + `TrashableContentExtensions` in `SharpSite.Abstractions`
- Extended both page/post repository contracts with:
  - `GetDeleted*()`
  - `Restore*()`
  - `PermanentlyDelete*()`
- Normal read queries exclude trashed records by default
- Permanent deletion is a separate explicit action, matching the recycle-bin decision

### Why
This keeps delete semantics consistent across pages and posts, and gives Simon a clear backend contract for a future Trash UI without putting business rules in Razor components.

### Simon UI Contract
- Delete button continues calling existing `DeletePage(int id)` / `DeletePost(string slug)` methods
- Trash UI should read from `GetDeletedPages()` / `GetDeletedPosts()`
- Restore actions call `RestorePage(int id)` / `RestorePost(string slug)`
- Empty-trash or per-item purge calls `PermanentlyDeletePage(int id)` / `PermanentlyDeletePost(string slug)`

### Cross-Impact
- **Simon:** Unblocks Trash UI implementation when #309/#299 scope is active
- **Kaylee:** Test infrastructure ready for trash/restore test cases

