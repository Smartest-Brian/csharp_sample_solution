# Build and publish the Service.ScheduleJob project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /workspace
COPY ["src/Services/Service.ScheduleJob/Service.ScheduleJob.csproj", "src/Services/Service.ScheduleJob/"]
COPY ["src/Librarys/", "src/Librarys/"]
RUN dotnet restore "src/Services/Service.ScheduleJob/Service.ScheduleJob.csproj"
COPY . .
WORKDIR "/workspace/src/Services/Service.ScheduleJob"
RUN dotnet publish "Service.ScheduleJob.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Service.ScheduleJob.dll"]
