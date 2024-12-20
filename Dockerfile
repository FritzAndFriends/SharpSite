# Use the mcr.microsoft.com/dotnet/aspnet:9.0 base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use the mcr.microsoft.com/dotnet/sdk:9.0 base image for build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SharpSite.Web/SharpSite.Web.csproj", "SharpSite.Web/"]
COPY ["SharpSite.Data.Postgres/SharpSite.Data.Postgres.csproj", "SharpSite.Data.Postgres/"]
COPY ["SharpSite.Security.Postgres/SharpSite.Security.Postgres.csproj", "SharpSite.Security.Postgres/"]
COPY ["SharpSite.ServiceDefaults/SharpSite.ServiceDefaults.csproj", "SharpSite.ServiceDefaults/"]
RUN dotnet restore "SharpSite.Web/SharpSite.Web.csproj"
COPY . .
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