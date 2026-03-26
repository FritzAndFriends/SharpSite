# Decision: .NET 10 + Aspire 13.2 Package Versions

**Author:** River  
**Date:** 2026-03-26  

## Context

Upgraded the entire SharpSite solution from .NET 9 / Aspire 9.1 to .NET 10 / Aspire 13.2.

## Decisions

### Package versions chosen for .NET 10

| Package Family | Version | Rationale |
|---|---|---|
| Aspire.Hosting.*, Aspire.Npgsql.* | 13.2.0 | Target Aspire version |
| Aspire.AppHost.Sdk | 13.2.0 | Must match Aspire packages |
| Microsoft.EntityFrameworkCore.* | 10.0.5 | Floor required by Aspire 13.2 transitives |
| Microsoft.Extensions.Hosting.* | 10.0.5 | Floor required by Aspire 13.2 transitives |
| Microsoft.Extensions.Caching.Memory | 10.0.5 | Floor required by Aspire 13.2 transitives |
| Microsoft.Extensions.ServiceDiscovery | 10.4.0 | NOT an Aspire package; 13.2.0 doesn't exist |
| Microsoft.Extensions.Http.Resilience | 10.4.0 | Latest stable for .NET 10 |
| OpenTelemetry.* | 1.15.0 | Fixes CVE in 1.11.x; floor required by Aspire 13.2 |
| Newtonsoft.Json | 13.0.4 | Floor required by Aspire 13.2 |
| System.Text.Json | 10.0.0 | Kept in central management, removed from csproj (pruned) |
| ASP.NET Core packages | 10.0.0 | Standard .NET 10 RTM |

### TargetFramework centralized

Moved `<TargetFramework>net10.0</TargetFramework>` to `Directory.Build.props` and removed it from all 19 individual csproj files. Future framework upgrades only need one line change.

### Packages removed from csproj references (pruned by .NET 10)

- `Microsoft.Extensions.Localization` — removed from SharpSite.Web.csproj
- `Microsoft.Extensions.Caching.Memory` — removed from SharpSite.Web.csproj  
- `System.Text.Json` — removed from SharpSite.AppHost.csproj

These are now part of the .NET 10 shared framework and flagged as NU1510 if referenced directly.

### Pre-existing issues flagged (not caused by upgrade)

- `SharpSite.UI.Security`: Service interfaces missing many methods the Razor components expect
- `SharpSite.Plugins.Data.Postgres`: `PgEmailSender` class never created, `Configure.cs` passes wrong constructor args
