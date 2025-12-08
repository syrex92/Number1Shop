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
COPY ["OrdersService/OrdersService/OrdersService.csproj", "OrdersService/"]
RUN dotnet restore "OrdersService/OrdersService.csproj"
COPY . .
WORKDIR "/src/OrdersService/OrdersService"
RUN dotnet build "OrdersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrdersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrdersService.dll"]
