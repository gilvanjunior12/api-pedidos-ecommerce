# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Pedidos.sln ./
COPY src/Pedidos.Domain/Pedidos.Domain.csproj src/Pedidos.Domain/
COPY src/Pedidos.Application/Pedidos.Application.csproj src/Pedidos.Application/
COPY src/Pedidos.Infrastructure/Pedidos.Infrastructure.csproj src/Pedidos.Infrastructure/
COPY src/Pedidos.Api/Pedidos.Api.csproj src/Pedidos.Api/
COPY tests/Pedidos.UnitTests/Pedidos.UnitTests.csproj tests/Pedidos.UnitTests/

RUN dotnet restore src/Pedidos.Api/Pedidos.Api.csproj

COPY . .
RUN dotnet publish src/Pedidos.Api/Pedidos.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN mkdir -p /app/logs

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pedidos.Api.dll"]
