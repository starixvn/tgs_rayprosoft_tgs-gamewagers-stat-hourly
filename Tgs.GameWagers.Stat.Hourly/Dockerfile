#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameWagers.Stat.Hourly.dll"]