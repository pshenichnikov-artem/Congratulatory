FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Api/CoreService.Api.csproj", "Api/"]
COPY ["Core/CoreService.Core.csproj", "Core/"]
COPY ["Infrastructure/CoreService.Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "Api/CoreService.Api.csproj"

COPY . .

WORKDIR "/src/Api"
RUN dotnet build "CoreService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CoreService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY Api/wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*