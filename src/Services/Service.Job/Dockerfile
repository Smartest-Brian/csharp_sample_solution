# Build and publish the Service.Job project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace
COPY ["src/Services/Service.Job/Service.Job.csproj", "src/Services/Service.Job/"]
COPY ["src/Librarys/", "src/Librarys/"]
RUN dotnet restore "src/Services/Service.Job/Service.Job.csproj"
COPY . .
WORKDIR "/workspace/src/Services/Service.Job"
RUN dotnet publish "Service.Job.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Service.Job.dll"]
