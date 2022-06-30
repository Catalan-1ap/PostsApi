FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 80
EXPOSE 443
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Api/Api.csproj", "Api/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Application/Application.csproj", "Application/"]
RUN dotnet restore "Api/Api.csproj"
COPY ./src .
WORKDIR "Api"

FROM build AS publish
RUN dotnet publish "Api.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
