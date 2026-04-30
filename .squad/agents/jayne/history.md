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
