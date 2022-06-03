FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src

COPY src/Api/Api.csproj .
RUN dotnet restore

COPY src/Api/* .
RUN dotnet publish -c Release -o /../out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "Api.dll"]