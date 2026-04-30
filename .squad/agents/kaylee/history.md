# Project Context

## 2026-04-30 — v0.8 Milestone Role Assignment

**Role:** Test Coverage Lead (v0.8.1–0.8.3 Tier 3)  
**Effort:** 5–7 hours  
**Timeline:** v0.8.0 completion → 4–6 weeks (Tier 3)  

**Tier 3 (plugin ecosystem):**
- #341 — Unit tests for PluginPackager (3-4h) — testing infrastructure for #166
- #319 — E2E test cases (2-3h) — support Wash; cover OOTB experience gaps

**Support:**
- Support River on plugin ecosystem test cases; Kaylee to suggest test structures for #166, #336, #334, #254

**Sync point:** Await Tier 1 completion (E2E framework stability) before #319 test spike. Coordinate with River on packager test harness design (early in #166 work).

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** SharpSite — a modern, accessible CMS built with .NET 9 and Blazor
- **Stack:** .NET 9, Blazor (SSR + Interactive Server), ASP.NET Core, Entity Framework Core, PostgreSQL, Docker, Playwright, xUnit, GitHub Actions
- **Created:** 2026-03-26

## Cross-Agent Status

### River Completion: #346 P0 RCE Fix (2026-03-31T13:47)
River completed the Security P0 Remote Code Execution vulnerability fix (#346). All Newtonsoft.Json with TypeNameHandling.Auto replaced with System.Text.Json. Build clean, 47 tests pass, vulnerability eliminated. **Unblocks Kaylee's anticipatory security tests for #346–#348.**

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-03-26 — Anticipatory Security Tests for Issues #346, #347, #348
- **Test conventions**: Existing tests follow Arrange-Act-Assert with tab indentation, namespace = directory path, xUnit `[Fact]`/`[Theory]`, Moq for dependencies.
- **PluginManager instantiation pattern**: `new PluginManager(Mock<PluginAssemblyManager>.Object, Mock<ApplicationState>.Object, Mock<ILogger<PluginManager>>.Object)` — see `HandleUploadedPlugin.cs` for reference.
- **ZIP creation for tests**: Use `MemoryStream` + `ZipArchive` in `ZipArchiveMode.Create` with `leaveOpen: true`, then wrap with `new Plugin(memoryStream, name)`.
- **Files added**:
  - `tests/SharpSite.Tests.Web/ApplicationState/Security/SerializationSecurityTests.cs` — Issue #346: 7 tests for System.Text.Json round-trip, $type rejection, no TypeNameHandling artifacts.
  - `tests/SharpSite.Tests.Web/PluginManager/Security/ZipExtractionSecurityTests.cs` — Issue #347: 8 tests for size limits, compression ratio, path traversal, valid ZIP extraction.
  - `tests/SharpSite.Tests.Web/PluginManager/Security/ThreadSafetyTests.cs` — Issue #348: 3 tests for concurrent ApplicationState.Plugins and PluginManager static ServiceCollection access.
  - `tests/SharpSite.Tests.Plugins/ConcurrentAccessTests.cs` — Issue #348: 3 tests for concurrent PluginAssemblyManager AddAssembly/RemoveAssembly and read-while-write safety.
- **Lambda discard gotcha**: Don't use `_ =` for discards inside lambdas where `_` is already the lambda parameter (causes CS0029). Use named locals instead.
- **Central Package Management**: Test projects use `<PackageReference Include="..." />` without `Version` attribute; versions are in `Directory.Packages.props`.

---
**[SCRIBE UPDATE - 2026-04-30 20:15:51]**
- Decision inbox merged (15 decisions)
- v0.8.0 scope locked and confirmed
- Cross-agent context synchronized
- Ready for parallel execution
