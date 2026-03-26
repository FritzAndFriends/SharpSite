# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-26 — Plugin System Architecture Review (spike_DatabasePlugin branch)

- **Branch health: RED** — Build fails with 60 errors. Root cause is type ambiguity between `SharpSite.Abstractions.Security` types and `Microsoft.AspNetCore.Identity` types in `src/SharpSite.Security.Postgres/`. The security abstraction migration is incomplete.
- **Plugin system architecture** is convention-based + attribute-driven. Plugins are `.sspkg` ZIP files loaded via collectible `AssemblyLoadContext`. Services registered via `RegisterPluginAttribute` and reflection scanning in `PluginManager.RegisterWithServiceLocator()`.
- **Critical security finding:** `ApplicationState.cs` uses `TypeNameHandling.Auto` with Newtonsoft.Json — a known RCE deserialization vector (lines 130-134, 212-216). Must be fixed before production.
- **Critical security finding:** No assembly validation, code signing, or integrity checking on plugin DLLs. Any DLL in the plugins directory runs with full app permissions.
- **Plugin extension points:** FileStorage, DataStorage (Config, EfContext, PageRepo, PostRepo), Security (SignIn, UserManager, UserRepo, EmailSender). Mapped in `PluginTypeMapping.cs`.
- **Static mutable state** in `PluginManager` (`_ServiceDescriptors`, `_ServiceProvider`) is a thread safety concern.
- **`IRunAtStartup` interface** is defined but never invoked by the PluginManager — lifecycle hooks are dead code.
- **Postgres plugin `manifest.json` is empty** (0 bytes) — would fail validation through standard plugin loading.
- **Hardcoded default admin** (`admin@localhost` / `Admin123!`) in the Postgres plugin's `RegisterPluginServices.cs`.
- Full analysis written to `.squad/decisions/inbox/mal-plugin-analysis.md`.
