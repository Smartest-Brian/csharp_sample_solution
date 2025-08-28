FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /workspace

COPY ["src/Services/Service.GraphQL/Service.GraphQL.csproj", "src/Services/Service.GraphQL/"]
RUN dotnet restore "src/Services/Service.GraphQL/Service.GraphQL.csproj"

COPY . .
WORKDIR /workspace/src/Services/Service.GraphQL
RUN dotnet publish "Service.GraphQL.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ARG APP_UID=1000
ARG APP_GID=1000
# 先檢查是否存在，再決定要不要新增
RUN set -eux; \
    if ! getent group app >/dev/null; then groupadd -g "$APP_GID" app; fi; \
    if ! id -u app >/dev/null 2>&1; then useradd -m -u "$APP_UID" -g app app; fi
USER app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Service.GraphQL.dll"]
