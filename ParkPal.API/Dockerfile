FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# ⭐️ MATCH THE MAC FOLDER STRUCTURE EXACTLY!
# Notice we are copying the API to a double-nested folder inside the container
COPY ParkPal.API/ParkPal.API/ParkPal.API.csproj ParkPal.API/ParkPal.API/
COPY ParkPal.Common/ParkPal.Common.csproj ParkPal.Common/

# Restore using the double-nested path
RUN dotnet restore "ParkPal.API/ParkPal.API/ParkPal.API.csproj"

# Copy the rest of the repository code over
COPY . .

# ⭐️ Set the working directory to the inner API folder before publishing
WORKDIR "/src/ParkPal.API/ParkPal.API"

# Build and publish
RUN dotnet publish "ParkPal.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ParkPal.API.dll"]