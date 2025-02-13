# Use the mcr.microsoft.com/dotnet/aspnet:9.0 base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the mcr.microsoft.com/dotnet/sdk:9.0 base image for build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy in the Directory.Build and Directory.Packages files
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]

COPY ["src/SharpSite.Web/SharpSite.Web.csproj", "SharpSite.Web/"]
COPY ["src/SharpSite.Data.Postgres/SharpSite.Data.Postgres.csproj", "SharpSite.Data.Postgres/"]
COPY ["src/SharpSite.Security.Postgres/SharpSite.Security.Postgres.csproj", "SharpSite.Security.Postgres/"]
COPY ["src/SharpSite.ServiceDefaults/SharpSite.ServiceDefaults.csproj", "SharpSite.ServiceDefaults/"]
COPY ["src/SharpSite.Plugins/SharpSite.Plugins.csproj", "SharpSite.Plugins/"]
COPY ["src/SharpSite.Abstractions/SharpSite.Abstractions.csproj", "SharpSite.Abstractions/"]

# Copy the other projects that SharpSite.Abstractions depends on
COPY ["src/SharpSite.Abstractions.Base/SharpSite.Abstractions.Base.csproj", "SharpSite.Abstractions.Base/"]
COPY ["src/SharpSite.Abstractions.FileStorage/SharpSite.Abstractions.FileStorage.csproj", "SharpSite.Abstractions.FileStorage/"]
COPY ["src/SharpSite.Abstractions.Theme/SharpSite.Abstractions.Theme.csproj", "SharpSite.Abstractions.Theme/"]

RUN dotnet restore "SharpSite.Web/SharpSite.Web.csproj"
COPY src/. .
WORKDIR "/src/SharpSite.Web"
RUN dotnet build "SharpSite.Web.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SharpSite.Web.csproj" -c Release -o /app/publish

# Copy the published output from the build stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Set the entry point to dotnet SharpSite.Web.dll
ENTRYPOINT ["dotnet", "SharpSite.Web.dll"]