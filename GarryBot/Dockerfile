FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GarryBot/GarryBot.csproj", "GarryBot/"]
COPY ["GarryLibrary/GarryLibrary.csproj", "GarryLibrary/"]
RUN dotnet restore "GarryBot/GarryBot.csproj"
COPY . .
WORKDIR "/src/GarryBot"
RUN dotnet build "./GarryBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GarryBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GarryBot.dll"]
