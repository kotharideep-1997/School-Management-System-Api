# =============================================================================
# Build context MUST be the solution root (folder that contains Api/, Domain/, …):
#
#   docker build -f Api/Dockerfile -t school-management-api .
#
# From PowerShell, if your path has spaces, run from the repo root:
#   cd "E:\School Management System\wdwd\School-Management-System-API"
#   docker build -f Api/Dockerfile -t school-management-api .
# =============================================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Layer cache: restore only when project files change
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"

# Copy remaining source and publish
COPY . .
RUN dotnet publish "Api/Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
