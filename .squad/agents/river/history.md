# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-26 — Security Context from Mal's Analysis

River should be aware of two critical P0 security issues identified on spike_DatabasePlugin branch:

1. **RCE via Insecure Deserialization** — `ApplicationState.cs` uses `TypeNameHandling.Auto` with Newtonsoft.Json. Attackers can modify `plugins/applicationState.json` to instantiate arbitrary types. **Fix:** Replace with `System.Text.Json` or implement custom whitelist-based `SerializationBinder`.

2. **No Assembly Validation** — Plugin DLLs loaded with zero integrity/signing checks. Any DLL in plugins directory runs with full app permissions. **Fix:** Validate assembly name against manifest ID at minimum.

These are blocking issues for production readiness. While .NET 10 upgrade is in progress, ensure P0 fixes are addressed in parallel or immediately after build stabilization.

Reference: `.squad/decisions/inbox/mal-plugin-analysis.md` (now merged to decisions.md)
