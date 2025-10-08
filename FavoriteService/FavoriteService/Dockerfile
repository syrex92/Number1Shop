FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get -y update;
RUN apt-get -y install curl

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FavoriteService/FavoriteService/FavoriteService.csproj", "FavoriteService/"]
RUN dotnet restore "FavoriteService/FavoriteService.csproj"
COPY . .
WORKDIR "/src/FavoriteService"
RUN dotnet build "FavoriteService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FavoriteService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FavoriteService.dll"]
