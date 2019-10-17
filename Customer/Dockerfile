FROM mcr.microsoft.com/dotnet/core/sdk:3.0 as base
WORKDIR /app

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app

COPY --from=base /app/out .
ENTRYPOINT [ "dotnet", "producer.dll" ]