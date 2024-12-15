# dotnet .NET 9 SDK

- Q: `dotnet restore` fails with missing NuGet packages errors

- A: You need to add NuGet package source and install workload for .NET Aspire

```bash
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
dotnet workload install aspire
```

- Q: running .NET Aspire project AppHost leads to errors:

```text
Container runtime 'docker' was found but appears to be unhealthy. Ensure that Docker is running and that the Docker daemon is accessible. If Resource Saver mode is enabled, containers may not run. For more information, visit: https://docs.docker.com/desktop/use-desktop/resource-saver/
```

- A: You need to download Docker Desktop and install it or run it manually.

  - [Mac](https://docs.docker.com/desktop/setup/install/mac-install/)
  - [Windows](https://docs.docker.com/desktop/setup/install/windows-install/)
  - [Linux](https://docs.docker.com/desktop/setup/install/linux/)
