FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY src/PlanejadorCompras.API/PlanejadorCompras.API.csproj src/PlanejadorCompras.API/
COPY src/PlanejadorCompras.Application/PlanejadorCompras.Application.csproj src/PlanejadorCompras.Application/
COPY src/PlanejadorCompras.Domain/PlanejadorCompras.Domain.csproj src/PlanejadorCompras.Domain/
COPY src/PlanejadorCompras.Infrastructure/PlanejadorCompras.Infrastructure.csproj src/PlanejadorCompras.Infrastructure/
RUN dotnet restore src/PlanejadorCompras.API/PlanejadorCompras.API.csproj

COPY src/PlanejadorCompras.API/ src/PlanejadorCompras.API/
COPY src/PlanejadorCompras.Application/ src/PlanejadorCompras.Application/
COPY src/PlanejadorCompras.Domain/ src/PlanejadorCompras.Domain/
COPY src/PlanejadorCompras.Infrastructure/ src/PlanejadorCompras.Infrastructure/
RUN dotnet publish src/PlanejadorCompras.API/PlanejadorCompras.API.csproj \
    --configuration Release --no-restore --output /app/publish /p:UseAppHost=false

FROM build AS migration-build
RUN dotnet tool install dotnet-ef --version 8.0.11 --tool-path /tools
WORKDIR /source/src/PlanejadorCompras.Infrastructure
# Only model metadata is needed during bundle generation; no database is contacted.
RUN ConnectionStrings__DefaultConnection="Host=localhost;Database=bundle_build;Username=bundle_build" \
    /tools/dotnet-ef migrations bundle --configuration Release \
    --startup-project ../PlanejadorCompras.API --output /app/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS migrations
WORKDIR /app/PlanejadorCompras.Infrastructure
COPY --from=migration-build /app/efbundle /app/efbundle
COPY --from=build /app/publish/appsettings.json /app/PlanejadorCompras.API/appsettings.json
ENV ASPNETCORE_ENVIRONMENT=Production
USER app
ENTRYPOINT ["/app/efbundle"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "PlanejadorCompras.API.dll"]
