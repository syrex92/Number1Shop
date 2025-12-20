# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get -y update && apt-get -y install curl && apt-get clean

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY CatalogService/CatalogService/CatalogService.csproj CatalogService/CatalogService/
COPY CatalogService/CatalogService.Core/CatalogService.Core.csproj CatalogService/CatalogService.Core/
COPY CatalogService/CatalogService.DataAccess/CatalogService.DataAccess.csproj CatalogService/CatalogService.DataAccess/
COPY Common/Shop.Core/Shop.Core.csproj Common/Shop.Core/
COPY Common/Shop.Demo.Data/Shop.Demo.Data.csproj Common/Shop.Demo.Data/

RUN dotnet restore CatalogService/CatalogService/CatalogService.csproj

COPY CatalogService/ CatalogService/
COPY Common/ Common/

WORKDIR /src/CatalogService/CatalogService
RUN dotnet build CatalogService.csproj -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CatalogService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.dll"]