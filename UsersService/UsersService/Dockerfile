FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get -y update && apt-get -y install curl && apt-get clean

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UsersService/UsersService/UsersService.csproj", "UsersService/UsersService/"]
COPY ["UsersService/UsersService.Application/UsersService.Application.csproj", "UsersService/UsersService.Application/"]
COPY ["UsersService/UsersService.Domain/UsersService.Domain.csproj", "UsersService/UsersService.Domain/"]
COPY ["UsersService/UsersService.Infrastructure/UsersService.Infrastructure.csproj", "UsersService/UsersService.Infrastructure/"]
COPY ["UsersService/UsersService.Persistence/UsersService.Persistence.csproj", "UsersService/UsersService.Persistence/"]
RUN dotnet restore "./UsersService/UsersService/UsersService.csproj"
COPY . .
WORKDIR "/src/UsersService/UsersService"
RUN dotnet build "./UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UsersService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]