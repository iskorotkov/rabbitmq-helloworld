FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["EmitLogsTopic/EmitLogsTopic.csproj", "EmitLogsTopic/"]
RUN dotnet restore "EmitLogsTopic/EmitLogsTopic.csproj"
COPY . .
WORKDIR "/src/EmitLogsTopic"
RUN dotnet build "EmitLogsTopic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmitLogsTopic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmitLogsTopic.dll"]
