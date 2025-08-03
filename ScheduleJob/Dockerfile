# Build and publish the ScheduleJob worker service
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ScheduleJob/ScheduleJob.csproj", "ScheduleJob/"]
RUN dotnet restore "ScheduleJob/ScheduleJob.csproj"
COPY . .
WORKDIR "/src/ScheduleJob"
RUN dotnet publish "ScheduleJob.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ScheduleJob.dll"]