# dotnet .NET 9 SDK

- Q: `dotnet restore` fails with missing NuGet packages errors

- A: You need to add NuGet package source and install workload for .NET Aspire

```bash
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
dotnet workload install aspire
```