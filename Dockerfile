FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish src/PreBot/PreBot.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_ENVIRONMENT=Production \
    ConnectionStrings__PreBot="Data Source=/data/prebot.db"
VOLUME /data
ENTRYPOINT ["dotnet", "PreBot.dll"]
