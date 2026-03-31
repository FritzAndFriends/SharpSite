# Decision: Thread-Safety Fix for Plugin System (Issue #348)

**Date:** 2026-03-31
**Author:** River
**Status:** Completed
**Issue:** #348 — Thread-unsafe static service collection in PluginManager causes race conditions

## Context

`PluginManager` uses static mutable `IServiceCollection` and `IServiceProvider?` fields shared across all instances. `PluginAssemblyManager` uses a plain `Dictionary`. Neither had synchronization. Concurrent plugin loads or configuration changes could corrupt these collections mid-enumeration.

## Decision

Three-layer fix matching the recommended approach from the issue:

1. **PluginAssemblyManager** — Replaced `Dictionary` with `ConcurrentDictionary`. Used `AddOrUpdate` and `TryRemove` for atomic operations. The `IReadOnlyDictionary` property now exposes a thread-safe view.

2. **PluginManager** — Added a static `lock` object (`_ServiceLock`) guarding all `_ServiceDescriptors` mutations and reads. Used `Interlocked.Exchange` for `_ServiceProvider` swaps to ensure atomic visibility. The async `ConfigurationSectionChanged` handler was restructured into two lock blocks (pre-await snapshot + post-await mutation) since `lock` cannot span `await`.

3. **ApplicationState.Plugins** — Changed from `Dictionary` to `ConcurrentDictionary`. Simplified `AddPlugin` to use the thread-safe indexer.

## Verification

- Build: 0 errors, 0 warnings
- Unit tests: 67 passed (including 6 new thread-safety tests written by Kaylee)
- E2E: 9 pre-existing failures (require running Aspire host — unrelated)

## Trade-offs

- `lock` serializes all `_ServiceDescriptors` access — acceptable since plugin registration is infrequent and not performance-critical.
- `ConfigurationSections` dictionary in `ApplicationState` remains a plain `Dictionary` — it's serialized to JSON and changing it risks breaking the custom `ConfigurationSectionJsonConverter`. A separate follow-up could address this if concurrent config section updates become a concern.
