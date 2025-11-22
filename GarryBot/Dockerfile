FROM mcr.microsoft.com/dotnet/runtime:10.0-preview AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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
RUN dotnet publish "./GarryBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:SelfContained=true /p:RuntimeIdentifier=linux-arm64

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./GarryBot"]
