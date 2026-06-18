# Stage 1 - Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["TechMove/TechMove.csproj", "TechMove/"]


RUN dotnet restore "TechMove/TechMove.csproj"


COPY . .


WORKDIR "/src/TechMove"
RUN dotnet build "TechMove.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "TechMove.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMove.dll"]