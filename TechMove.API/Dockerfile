# Stage 1 - Build the API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the API project file first
COPY ["TechMove.API/TechMove.API.csproj", "TechMove.API/"]
COPY ["TechMove/TechMove.csproj", "TechMove/"]

# Restore dependencies
RUN dotnet restore "TechMove.API/TechMove.API.csproj"

# Copy all source code
COPY . .

# Build the API
WORKDIR "/src/TechMove.API"
RUN dotnet build "TechMove.API.csproj" -c Release -o /app/build

# Stage 2 - Publish the application
FROM build AS publish
RUN dotnet publish "TechMove.API.csproj" -c Release -o /app/publish \
    /p:ErrorOnDuplicatePublishOutputFiles=false

# Stage 3 - Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMove.API.dll"]