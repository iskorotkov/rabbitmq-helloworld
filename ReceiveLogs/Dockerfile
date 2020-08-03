FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ReceiveLogs/ReceiveLogs.csproj", "ReceiveLogs/"]
RUN dotnet restore "ReceiveLogs/ReceiveLogs.csproj"
COPY . .
WORKDIR "/src/ReceiveLogs"
RUN dotnet build "ReceiveLogs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReceiveLogs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReceiveLogs.dll"]
