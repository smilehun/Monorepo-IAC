# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/backend/AccessHub.Identity.sln src/backend/
COPY src/backend/src/AccessHub.Identity.Api/AccessHub.Identity.Api.csproj src/backend/src/AccessHub.Identity.Api/
COPY src/backend/src/AccessHub.Identity.Application/AccessHub.Identity.Application.csproj src/backend/src/AccessHub.Identity.Application/
COPY src/backend/src/AccessHub.Identity.Domain/AccessHub.Identity.Domain.csproj src/backend/src/AccessHub.Identity.Domain/
COPY src/backend/src/AccessHub.Identity.Infrastructure/AccessHub.Identity.Infrastructure.csproj src/backend/src/AccessHub.Identity.Infrastructure/
COPY src/backend/tests/AccessHub.Identity.IntegrationTests/AccessHub.Identity.IntegrationTests.csproj src/backend/tests/AccessHub.Identity.IntegrationTests/
COPY src/backend/tests/AccessHub.Identity.UnitTests/AccessHub.Identity.UnitTests.csproj src/backend/tests/AccessHub.Identity.UnitTests/

RUN dotnet restore src/backend/AccessHub.Identity.sln

COPY src/backend/ src/backend/

RUN dotnet publish src/backend/src/AccessHub.Identity.Api/AccessHub.Identity.Api.csproj \
  --configuration Release \
  --no-restore \
  --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update \
  && apt-get install --no-install-recommends --yes curl \
  && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

EXPOSE 8080

ENTRYPOINT ["dotnet", "AccessHub.Identity.Api.dll"]
