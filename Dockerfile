FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY BlogWeb/BlogWeb.csproj BlogWeb/
RUN dotnet restore BlogWeb/BlogWeb.csproj
COPY BlogWeb/ BlogWeb/
RUN dotnet publish BlogWeb/BlogWeb.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
# Railway injects $PORT at runtime; ASP.NET Core needs ASPNETCORE_URLS, not $PORT directly.
CMD ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:$PORT dotnet BlogWeb.dll"]
