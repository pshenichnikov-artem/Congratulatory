FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["VkBot/VkBot.csproj", "VkBot/"]
COPY ["BotShared/BotShared.csproj", "BotShared/"]

RUN dotnet restore "VkBot/VkBot.csproj"

COPY . .

WORKDIR "/src/VkBot"
RUN dotnet build "VkBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VkBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY BotShared/appsettings.json ./appsettings.json

ENTRYPOINT ["dotnet", "VkBot.dll"]
