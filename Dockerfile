FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "SmartSeason Field Monitoring System.csproj" -o /published /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /published .
ENTRYPOINT [ "dotnet","SmartSeason Field Monitoring System.dll" ]