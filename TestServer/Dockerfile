FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# apt install 的软件和 ubuntu 相同
RUN apt update -y && \
    apt install lftp ca-certificates apt-transport-https tzdata telnet less iproute2 iputils-ping selinux-utils policycoreutils ntp ntpdate htop nethogs nload tree lrzsz iotop iptraf-ng zip unzip ca-certificates curl gnupg libpcre3 libpcre3-dev openssl libssl-dev build-essential rsync sshpass -y && \
    ls
ENV TZ=Asia/Shanghai
WORKDIR /app
# 去掉警告
ENV ASPNETCORE_HTTP_PORTS=''
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TestServer.csproj", "TestServer/"]
RUN dotnet restore "TestServer/TestServer.csproj"
COPY . TestServer/
RUN dotnet publish TestServer/TestServer.csproj -o /app/build /p:PublishProfile=Properties/PublishProfiles/linux-x64.pubxml

FROM base AS publish
WORKDIR /app
COPY --from=build /app/build /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app /app
ENTRYPOINT ["dotnet","TestServer.dll"]
