FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BirthdayNotificationWorker/BirthdayNotificationWorker.csproj", "BirthdayNotificationWorker/"]
RUN dotnet restore "BirthdayNotificationWorker/BirthdayNotificationWorker.csproj"
COPY . .
WORKDIR "/src/BirthdayNotificationWorker"
RUN dotnet build "BirthdayNotificationWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BirthdayNotificationWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BirthdayNotificationWorker.dll"]
