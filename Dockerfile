FROM mcr.microsoft.com/dotnet/core/sdk:3.1.401-buster-arm64v8

WORKDIR /overkill-framework

COPY Overkill Overkill
COPY Overkill.Common Overkill.Common
COPY Overkill.Core Overkill.Core
COPY Overkill.Proxies Overkill.Proxies
COPY Overkill.PubSub Overkill.PubSub
COPY Overkill.Services Overkill.Services
COPY Overkill.Websockets Overkill.Websockets
COPY WebSocketSharp.NetCore WebSocketSharp.NetCore

COPY Plugin.Lidar Plugin.Lidar
COPY Vehicle.Traxxas Vehicle.Traxxas

COPY Overkill.Services.Tests Overkill.Services.Tests

COPY Overkill.sln Overkill.sln

RUN dotnet build

ENTRYPOINT ["dotnet", "Overkill/bin/Release/netcoreapp3.1/Overkill.dll"]
