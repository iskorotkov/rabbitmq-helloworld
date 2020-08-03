FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RPCServer/RPCServer.csproj", "RPCServer/"]
RUN dotnet restore "RPCServer/RPCServer.csproj"
COPY . .
WORKDIR "/src/RPCServer"
RUN dotnet build "RPCServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPCServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPCServer.dll"]
