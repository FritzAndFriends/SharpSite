# Project Context

## 2026-04-30 — v0.8 Milestone Role Assignment

**Role:** E2E Validation Lead (v0.8.0 Tier 1)  
**Effort:** 8–10 hours  
**Timeline:** v0.8.0 (2–3 weeks critical path)  

**Tier 1 (ship-blockers):**
- #351 — .NET 10 + Aspire validation (E2E) (2-3h) — Playwright + AppHost
- #319 — E2E test for out-of-box experience (4-6h) — prevents "works on my machine"
- #272 — Fix Playwright badge/reports in CI (2-3h) — unblock test visibility

**Responsibilities:**
- Add retry logic + diagnostics to #319 (E2E flakiness mitigation)
- Validate full E2E pipeline in CI post-fix
- Coordinate with Zoe (CI/badges) and Kaylee (test cases)

**Risk mitigation:** 2-week buffer for E2E flakiness; instrument diagnostics early.

**Sync point:** Tier 1 completion gates Tier 2-3 work. E2E tests must pass CI before v0.8.0 release.

### 2026-04-30 — Simon Completion: Admin Asset UI + Setup Wizard (Tier 2 ✅)

**Status:** COMPLETED  
**Issues:** #320, #321, #323, #322 (setup wizard integration)  
**Deliverables:** Reusable `SiteAssetManager` component + 4-step setup wizard

**Simon's Asset Decision:** One shared Blazor component for both setup and admin asset management. Assets managed through `ApplicationState` flags, resolved via `/api/files/{filename}` with deterministic names (`site-logo.*`, `site-favicon.*`).

**Impact on Wash:**
- **E2E Test Scenarios Now Available:**
  - Setup wizard: Verify asset upload in Step 2 (site identity)
  - Admin panel: Verify logo/favicon upload, preview, removal
  - Cross-flow: Verify assets persist across setup + admin maintenance
- **Ready:** Can spike E2E test case coverage for setup + admin asset workflows
- **Test data:** Use SiteAssetManager test patterns from Simon's implementation (upload validation, preview URLs)

**Build Status:** SharpSite.Web clean ✅ | SharpSite.Tests.Web 57/57 ✅

**Next:** Wash can now integrate asset upload E2E scenarios into #319 test spike.



---
**[SCRIBE UPDATE - 2026-05-01 00:03:00Z — Wash #319 E2E v0.8.0 Execution COMPLETE]**
- ✅ E2E test suite: Login + admin workflow acceptance coverage
- ✅ Admin workflow helpers: Reusable test utilities for setup wizard, asset management
- ✅ Data-testid seams: Stable element selectors across Blazor components
- ✅ Forced password change E2E: Full flow validation pass
- ✅ Navigation flakiness: Replaced timing waits with condition-based polling
- ✅ Build status: SharpSite.Web clean, SharpSite.E2E builds clean
- ⏳ **BLOCKER:** CreatePostTests.CanCreateSaveAndPublishPost fails at live-site assertion
  - Published post public URLs return 404 (route not mapped or URL generation issue)
  - Post create/save/publish flow works; issue is at public URL resolution layer
  - E2E test ready to validate once route is fixed
- **HANDOFF:** Simon assigned to fix public post route (Issue #319 blocker)
- **NEXT:** Wash standing by to revalidate E2E test against fixed route

---

**Status:** BLOCKED pending user clarification  
**Issue:** #351 (.NET 10 + Aspire 13.2 E2E validation) lacks clear pass criteria.  
**Critical Gap:** Which E2E scenarios constitute "validation complete"? Just AppHost health + page load, or full workflows?  

**Impact on Wash:**
- Cannot scope #351 effort accurately until test checklist is defined
- Cannot spike #319 (flaky tests) until E2E baseline is clear

**Next Action:** User provides test checklist; Mal updates #351 with acceptance criteria. Wash can then begin sprint planning.

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

---
**[SCRIBE UPDATE - 2026-04-30 20:15:51]**
- Decision inbox merged (15 decisions)
- v0.8.0 scope locked and confirmed
- Cross-agent context synchronized
- Ready for parallel execution

---
**[SCRIBE UPDATE - 2026-05-01 00:02:00Z — Kaylee Trash Backend Unit Tests Completed]**
- Kaylee completed repository-level unit test harness for trash/recycle bin flows (#309/#299)
- Tests validate: soft delete, restore, permanent purge, trashed-content exclusion
- All test projects passing (SharpSite.Tests.Web ✅, SharpSite.Tests.Plugins ✅)
- Unit test foundation ready for E2E test scenarios on #319
- River trash backend contracts validated; ready for Simon API integration
- **Important:** Repository tests exclude trashed content from normal queries by default — E2E tests should verify this behavior end-to-end
