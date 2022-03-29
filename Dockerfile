FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish /app/AlertsApi/AlertsApi.Api.csproj -o /app/api


FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app
COPY --from=build /app/api /app
ENTRYPOINT [ "dotnet", "/app/AlertsApi.Api.dll" ]