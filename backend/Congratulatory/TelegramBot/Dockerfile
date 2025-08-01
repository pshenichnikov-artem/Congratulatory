FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["TelegramBot/TelegramBot.csproj", "TelegramBot/"]
COPY ["BotShared/BotShared.csproj", "BotShared/"]

RUN dotnet restore "TelegramBot/TelegramBot.csproj"

COPY . .

WORKDIR "/src/TelegramBot"
RUN dotnet build "TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY BotShared/appsettings.json ./appsettings.json

ENTRYPOINT ["dotnet", "TelegramBot.dll"]
