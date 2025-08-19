# Build and publish the Service.WebAPI project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace
COPY ["src/Services/Service.WebAPI/Service.WebAPI.csproj", "src/Services/Service.WebAPI/"]
COPY ["src/Librarys/Library.Core/Library.Core.csproj", "src/Librarys/Library.Core/"]
RUN dotnet restore "src/Services/Service.WebAPI/Service.WebAPI.csproj"
COPY . .
WORKDIR "/workspace/src/Services/Service.WebAPI"
RUN dotnet publish "Service.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Service.WebAPI.dll"]
