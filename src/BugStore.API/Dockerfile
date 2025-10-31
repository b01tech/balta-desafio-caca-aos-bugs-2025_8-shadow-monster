# BugStore.API
# BASE
FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BugStore.API/BugStore.API.csproj", "BugStore.API/"]
COPY ["BugStore.Application/BugStore.Application.csproj", "BugStore.Application/"]
COPY ["BugStore.Domain/BugStore.Domain.csproj", "BugStore.Domain/"]
COPY ["BugStore.Exception/BugStore.Exception.csproj", "BugStore.Exception/"]
COPY ["BugStore.Infra/BugStore.Infra.csproj", "BugStore.Infra/"]
RUN dotnet restore "./BugStore.API/BugStore.API.csproj"
COPY . .
WORKDIR "/src/BugStore.API"
RUN dotnet build "./BugStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BugStore.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# RUN
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BugStore.API.dll"]
