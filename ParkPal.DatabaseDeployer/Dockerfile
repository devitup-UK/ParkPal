FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ⭐️ THE FIX: Explicitly include the folder path so Docker can find the file from the root context
COPY ["ParkPal.DatabaseDeployer/ParkPal.DatabaseDeployer.csproj", "ParkPal.DatabaseDeployer/"]
RUN dotnet restore "ParkPal.DatabaseDeployer/ParkPal.DatabaseDeployer.csproj"

# Copy the rest of the source code
COPY . .

# Set the working directory to the project folder and publish
WORKDIR "/src/ParkPal.DatabaseDeployer"
RUN dotnet publish "ParkPal.DatabaseDeployer.csproj" -c Release -o /app/publish

# Build the final lightweight runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ParkPal.DatabaseDeployer.dll"]