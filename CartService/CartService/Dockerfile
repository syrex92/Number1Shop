FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get -y update && apt-get -y install curl && apt-get clean

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release


WORKDIR /src
COPY ["CartService/CartService/CartService.csproj", "CartService/CartService/"]
COPY ["Common/Shop.Core/Shop.Core.csproj", "Common/Shop.Core/"]
COPY ["Common/Shop.Demo.Data/", "Common/Shop.Demo.Data/"]
RUN dotnet restore "CartService/CartService/CartService.csproj"
#COPY . .
COPY ["CartService/CartService/", "CartService/CartService/"]
COPY ["Common/Shop.Core/", "Common/Shop.Core/"]
COPY ["Common/Shop.Demo.Data/", "Common/Shop.Demo.Data/"]

WORKDIR "/src/CartService/CartService"
RUN dotnet build "CartService.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CartService.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartService.dll"]
