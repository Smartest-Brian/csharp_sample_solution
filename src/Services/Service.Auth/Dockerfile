# Build and publish the Service.Auth project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace
COPY ["src/Services/Service.Auth/Service.Auth.csproj", "src/Services/Service.Auth/"]
COPY ["src/Librarys/", "src/Librarys/"]
RUN dotnet restore "src/Services/Service.Auth/Service.Auth.csproj"
COPY . .
WORKDIR "/workspace/src/Services/Service.Auth"
RUN dotnet publish "Service.Auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Service.Auth.dll"]
