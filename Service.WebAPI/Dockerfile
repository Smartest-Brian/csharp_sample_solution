# Build and publish the Service.WebAPI project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Service.WebAPI/Service.WebAPI.csproj", "Service.WebAPI/"]
RUN dotnet restore "Service.WebAPI/Service.WebAPI.csproj"
COPY . .
WORKDIR "/src/Service.WebAPI"
RUN dotnet publish "Service.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Service.WebAPI.dll"]
