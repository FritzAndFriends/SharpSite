# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

### Commit Movement Pattern: Cherry-Pick for Safe Branch Transfers (2026-04-30)
When moving a commit from one branch to another without rewriting remote history:
1. Use `git cherry-pick <commit>` on the destination branch — creates a new commit with identical author/message/tree, new commit hash
2. Verify the source commit still exists on its original branch (no deletion)
3. Use `git log --graph` to visualize both branches and confirm clean separation
4. Clean up stale `.git/index.lock` if git operations hang (e.g., from crashed prior processes)
5. After cherry-pick succeeds, push destination branch; source branch remains untouched
**Rationale:** Safer than rebasing for remote-tracked branches — no rewrite risk, clear audit trail. Source commit stays on its branch.

### v0.7 Release: Tag & GitHub Release (2026-04-30)
Completed end-to-end release process for v0.7:
1. Verified PR #304 merged to `upstream/main` (commit `c35e5e06c5e17b3e0fbada8fb006d30dadcbf5e6`)
2. Created annotated tag `v0.7` locally
3. Pushed tag to `upstream` with explicit `refs/tags/v0.7` (necessary because a branch named `v0.7` existed from earlier fetch)
4. Created GitHub release with auto-generated changelog (v0.2...v0.7)
5. Release URL: https://github.com/csharpfritz/SharpSite/releases/tag/v0.7
**Key Learning:** Explicit tag refspec (`refs/tags/v0.7`) prevents ambiguity when a branch and tag share names. GitHub release auto-notes work well after v0.2 baseline is established.
