# csharp_sample_solution

## create Empty Solution

## create WebAPI controller project

mkdir WebAPI

cd WebAPI

dotnet new webapi --use-controllers

## docker

docker build -t webapi -f WebAPI/Dockerfile .

docker build -t schedulejob -f ScheduleJob/Dockerfile .

docker run -p 8080:8080 webapi

## WebAPI

使用 Controller 執行 RESTful API

## ScheduleJob

使用 Quartz 實現 Job

## GraphQL

如果要佈署到多執行個體，請把上面的 InMemory 換成 Redis 或其他 provider，並在程式碼中使用相對應的 AddRedisSubscriptions()
等方法。

### export schema







