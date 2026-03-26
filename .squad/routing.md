# Work Routing

How to decide who handles what.

## Routing Table

| Work Type | Route To | Examples |
|-----------|----------|----------|
| Blazor UI, components, Razor pages | Simon | Build components, fix layouts, accessibility |
| APIs, EF Core, services, data layer | River | Endpoints, migrations, business logic, plugins |
| Architecture, code review, triage | Mal | Review PRs, scope decisions, issue triage |
| Unit tests, mocking, coverage | Kaylee | Write xUnit tests, improve coverage, test gaps |
| E2E tests, Playwright, browser automation | Wash | User journeys, Playwright scripts, cross-browser |
| GitHub Actions, CI pipelines, builds | Zoe | Workflow files, build scripts, test badges |
| Docker, deployment, infrastructure | Jayne | Dockerfile, containers, env config, releases |
| Social media, announcements, engagement | Inara | Twitter/X posts, LinkedIn, community outreach |
| Blog posts, tutorials, documentation | Book | Write posts, how-to guides, README updates |
| Code review | Mal | Review PRs, check quality, suggest improvements |
| Testing | Kaylee + Wash | Write tests, find edge cases, verify fixes |
| Scope & priorities | Mal | What to build next, trade-offs, decisions |
| Session logging | Scribe | Automatic — never needs routing |

## Issue Routing

| Label | Action | Who |
|-------|--------|-----|
| `squad` | Triage: analyze issue, assign `squad:{member}` label | Mal (Lead) |
| `squad:mal` | Architecture, review, cross-cutting | Mal |
| `squad:simon` | Frontend / Blazor UI work | Simon |
| `squad:river` | Backend / API / data work | River |
| `squad:kaylee` | Unit test work | Kaylee |
| `squad:wash` | E2E / Playwright test work | Wash |
| `squad:zoe` | CI pipeline work | Zoe |
| `squad:jayne` | Deployment / container work | Jayne |
| `squad:inara` | Social media work | Inara |
| `squad:book` | Blog / documentation work | Book |

### How Issue Assignment Works

1. When a GitHub issue gets the `squad` label, **Mal** triages it — analyzing content, assigning the right `squad:{member}` label, and commenting with triage notes.
2. When a `squad:{member}` label is applied, that member picks up the issue in their next session.
3. Members can reassign by removing their label and adding another member's label.
4. The `squad` label is the "inbox" — untriaged issues waiting for Mal's review.

## Rules

1. **Eager by default** — spawn all agents who could usefully start work, including anticipatory downstream work.
2. **Scribe always runs** after substantial work, always as `mode: "background"`. Never blocks.
3. **Quick facts → coordinator answers directly.** Don't spawn an agent for "what port does the server run on?"
4. **When two agents could handle it**, pick the one whose domain is the primary concern.
5. **"Team, ..." → fan-out.** Spawn all relevant agents in parallel as `mode: "background"`.
6. **Anticipate downstream work.** If a feature is being built, spawn Kaylee/Wash to write test cases from requirements simultaneously.
7. **Issue-labeled work** — when a `squad:{member}` label is applied to an issue, route to that member. Mal handles all `squad` (base label) triage.
