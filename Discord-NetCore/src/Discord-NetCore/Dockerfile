FROM microsoft/dotnet:2.0-runtime AS base
RUN apt-get update && apt-get install -y youtube-dl ffmpeg libopus0 libsodium18 libsodium-dev libopus-dev
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY "src/Discord-NetCore/NuGet.config" "src/Discord-NetCore/"
COPY src/Discord-NetCore/Discord-NetCore.csproj src/Discord-NetCore/
RUN dotnet restore
COPY . .
WORKDIR /src/src/Discord-NetCore
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Discord-NetCore.dll"]
