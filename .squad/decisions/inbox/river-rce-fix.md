# Decision: Replace Newtonsoft.Json TypeNameHandling.Auto with System.Text.Json

**Date:** 2026-03-26
**Author:** River
**Issue:** #346 — [Security P0] Remote Code Execution via TypeNameHandling.Auto deserialization
**Status:** Implemented

## Context

`Newtonsoft.Json` with `TypeNameHandling.Auto` was used in 4 locations across the plugin/configuration system. This is a well-documented RCE deserialization vulnerability — if an attacker can write to `plugins/applicationState.json`, they can instantiate arbitrary .NET types via known gadget chains.

## Decision

Replace all Newtonsoft.Json usage with `System.Text.Json`. For the polymorphic `ISharpSiteConfigurationSection` dictionary, implement a custom `ConfigurationSectionJsonConverter` that only resolves types implementing the interface.

## Alternatives Considered

1. **Custom `ISerializationBinder` whitelist** — Still depends on Newtonsoft.Json, which has a larger attack surface. Requires ongoing maintenance of the type whitelist.
2. **`System.Text.Json` with `[JsonDerivedType]`** — Not viable because plugin configuration types are unknown at compile time.

## Changes Made

| File | Change |
|------|--------|
| `src/SharpSite.Web/ConfigurationSectionJsonConverter.cs` | **New** — Safe polymorphic converter with type validation |
| `src/SharpSite.Web/ApplicationState.cs` | Replaced Newtonsoft serialization with System.Text.Json |
| `src/SharpSite.Abstractions/ApplicationStateModel.cs` | Swapped Newtonsoft attributes to System.Text.Json equivalents |
| `src/SharpSite.Web/SharpsiteConfigurationExtensions.cs` | Replaced Newtonsoft clone with System.Text.Json |
| `src/SharpSite.Web/Components/Admin/PluginConfigUI.razor` | Removed Newtonsoft using directive |
| `src/SharpSite.Abstractions/SharpSite.Abstractions.csproj` | Removed Newtonsoft.Json PackageReference |
| `src/SharpSite.Web/SharpSite.Web.csproj` | Removed Newtonsoft.Json PackageReference |
| `tests/.../WhenFileExists.cs` | Updated test to use System.Text.Json |

## Verification

- Solution builds clean
- All 47 unit tests pass
- Zero remaining Newtonsoft.Json references in source code
