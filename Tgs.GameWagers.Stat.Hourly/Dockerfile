FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameWagers.Stat.Hourly/Tgs.GameWagers.Stat.Hourly.csproj", "Tgs.GameWagers.Stat.Hourly/"]
COPY ["Tgs.GameWagers.Stat.Hourly.Common/Tgs.GameWagers.Stat.Hourly.Common.csproj", "Tgs.GameWagers.Stat.Hourly.Common/"]
COPY ["Tgs.GameWagers.Stat.Hourly.Core/Tgs.GameWagers.Stat.Hourly.Core.csproj", "Tgs.GameWagers.Stat.Hourly.Core/"]
COPY ["Tgs.GameWagers.Stat.Hourly.Repositories/Tgs.GameWagers.Stat.Hourly.Repositories.csproj", "Tgs.GameWagers.Stat.Hourly.Repositories/"]
RUN dotnet restore "./Tgs.GameWagers.Stat.Hourly/./Tgs.GameWagers.Stat.Hourly.csproj"
COPY . .
WORKDIR "/src/Tgs.GameWagers.Stat.Hourly"
RUN dotnet build "./Tgs.GameWagers.Stat.Hourly.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameWagers.Stat.Hourly.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameWagers.Stat.Hourly.dll"]