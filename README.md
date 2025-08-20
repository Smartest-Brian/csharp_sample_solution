# csharp_sample_solution

---

# command line

## create Solution

dotnet new sln -n Solution

## create service WebAPI controller project

dotnet new webapi -n Service.WebAPI --use-controllers -f net9.0

dotnet new webapi -n Service.GraphQL -f net9.0

dotnet new worker -n Service.ScheduleJob -f net9.0

## create library

dotnet new classlib -n Library.Core -f net9.0

dotnet new classlib -n Library.Database

## add project to solution

dotnet sln Solution.sln add Service.WebAPI/Service.WebAPI.csproj

dotnet sln Solution.sln add Library.Core/Library.Core.csproj

## reference

dotnet add Service.WebAPI/Service.WebAPI.csproj reference Library.Core/Library.Core.csproj

---

## docker

docker build -t webapi -f src/Services/Service.WebAPI/Dockerfile .

docker build -t schedulejob -f src/Services/Service.Job/Dockerfile .

docker run -p 8080:8080 webapi

## docker-compose

### build container

docker-compose up -d

### clean container

停止並刪除容器（保留 volume 與網路）

docker-compose down

停止並刪除容器 + volume（清空資料）

docker-compose down -v

### clean completely (image、volume、network) include other images

docker system prune -a --volumes

---

# describe

## Service.WebAPI

dotnet add package Swashbuckle.AspNetCore -v 6.7.0

使用 Controller 執行 RESTful API

## Service.ScheduleJob

使用 Quartz 實現 Job

## Service.GraphQL

dotnet add package HotChocolate.AspNetCore --version 13.9.14

dotnet add package HotChocolate.Subscriptions.InMemory --version 13.9.14

如果要佈署到多執行個體，請把上面的 InMemory 換成 Redis 或其他 provider，並在程式碼中使用相對應的 AddRedisSubscriptions()
等方法。







