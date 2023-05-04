FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TestServer.csproj", "TestServer/"]
RUN dotnet restore "TestServer/TestServer.csproj"
COPY . TestServer/
WORKDIR "/src/TestServer"
RUN dotnet build "TestServer.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src
RUN dotnet publish "TestServer/TestServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestServer.dll"]
