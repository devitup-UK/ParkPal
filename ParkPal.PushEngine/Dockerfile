FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["ParkPal.PushEngine/ParkPal.PushEngine.csproj", "ParkPal.PushEngine/"]
RUN dotnet restore "ParkPal.PushEngine/ParkPal.PushEngine.csproj"
COPY . .
WORKDIR "/src/ParkPal.PushEngine"
RUN dotnet build "ParkPal.PushEngine.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ParkPal.PushEngine.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParkPal.PushEngine.dll"]

