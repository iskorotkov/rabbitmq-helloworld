FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ReceiveLogsTopic/ReceiveLogsTopic.csproj", "ReceiveLogsTopic/"]
RUN dotnet restore "ReceiveLogsTopic/ReceiveLogsTopic.csproj"
COPY . .
WORKDIR "/src/ReceiveLogsTopic"
RUN dotnet build "ReceiveLogsTopic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReceiveLogsTopic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReceiveLogsTopic.dll"]
