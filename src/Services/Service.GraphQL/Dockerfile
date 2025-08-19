FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /workspace
COPY ["src/Services/Service.GraphQL/Service.GraphQL.csproj", "src/Services/Service.GraphQL/"]
RUN dotnet restore "src/Services/Service.GraphQL/Service.GraphQL.csproj"
COPY . .
WORKDIR "/workspace/src/Services/Service.GraphQL"
RUN dotnet build "Service.GraphQL.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Service.GraphQL.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.GraphQL.dll"]
