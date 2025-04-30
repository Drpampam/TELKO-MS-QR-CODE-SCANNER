# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

RUN mkdir -p src/InfrastructureFolder/Infrastructure

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Core/Application/Application.csproj", "src/Core/Application/"]
COPY ["src/Core/Domain/Domain.csproj", "src/Core/Domain/"]
COPY ["src/InfrastructureFolder/Identity/Identity.csproj", "src/InfrastructureFolder/Identity/"]
COPY ["src/InfrastructureFolder/Infrastructure/Infrastructure.csproj", "src/InfrastructureFolder/Infrastructure"]
COPY ["src/InfrastructureFolder/Persistence/Persistence.csproj", "src/InfrastructureFolder/Persistence/"]

RUN dotnet restore "src/Api/Rest/Api/Api.csproj"
COPY . .

WORKDIR "src/Api/Rest/Api/"
RUN dotnet build "Api.csproj" -c Release -o /app/build

# REST API Container
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS api
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "Api.dll"]