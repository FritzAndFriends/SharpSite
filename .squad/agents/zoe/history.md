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
