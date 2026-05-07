FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ParkPal.SyncService/ParkPal.SyncService.csproj", "ParkPal.SyncService/"]
RUN dotnet restore "ParkPal.SyncService/ParkPal.SyncService.csproj"
COPY . .
WORKDIR "/src/ParkPal.SyncService"
RUN dotnet build "./ParkPal.SyncService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ParkPal.SyncService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParkPal.SyncService.dll"]
