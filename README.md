# csharp_sample_solution

---

# command line

## create Solution

dotnet new sln -n Solution

## create service WebAPI controller project

dotnet new webapi -n Service.WebAPI --use-controllers -o src/Services/Service.WebAPI -f net9.0

dotnet new webapi -n Service.GraphQL -o src/Services/Service.GraphQL -f net9.0

dotnet new worker -n Service.ScheduleJob -o src/Services/Service.ScheduleJob -f net9.0

## create library

dotnet new classlib -n Library.Core -o src/Librarys/Library.Core -f net9.0

dotnet new classlib -n Library.Database -o src/Librarys/Library.Database -f net9.0

dotnet new classlib -n Library.Database

## add project to solution

dotnet sln Solution.sln add src/Services/Service.WebAPI/Service.WebAPI.csproj

dotnet sln Solution.sln add src/Librarys/Library.Core/Library.Core.csproj

## reference

dotnet add src/Services/Service.WebAPI/Service.WebAPI.csproj reference src/Librarys/Library.Database/Library.Database.csproj

dotnet add src/Services/Service.WebAPI/Service.WebAPI.csproj reference src/Librarys/Library.Core/Library.Core.csproj

---

## format

```bash
# 格式化整個 solution
dotnet format Solution.sln

# 格式化特定專案
dotnet format src/Service.WebAPI/Service.WebAPI.csproj

# 只檢查，不實際修改
dotnet format --verify-no-changes

# 只針對程式碼風格 (Style)
dotnet format style

# 只針對語法/編譯錯誤 (Analyzers)
dotnet format analyzers

# 只針對空白/縮排/using 排序 (Whitespace)
dotnet format whitespace
```

---

## EF CLI install (once)

dotnet tool install --global dotnet-ef

## run EF CLI in Library.Database path

### public

```shell
dotnet ef dbcontext scaffold \
"Host=localhost;Port=5432;Database=csharp_sample_solution_db;Username=db_admin;Password=P@ssw0rd" \
Npgsql.EntityFrameworkCore.PostgreSQL \
--schema public \
--context PublicDbContext \
--context-dir ./Contexts/Public \
--context-namespace Library.Database.Contexts.Public \
--output-dir ./Models/Public \
--namespace Library.Database.Models.Public \
--no-onconfiguring \
--no-pluralize \
--force
```

### auth

```shell
dotnet ef dbcontext scaffold \
"Host=localhost;Port=5432;Database=csharp_sample_solution_db;Username=db_admin;Password=P@ssw0rd" \
Npgsql.EntityFrameworkCore.PostgreSQL \
--schema auth \
--context AuthDbContext \
--context-dir ./Contexts/Auth \
--context-namespace Library.Database.Contexts.Auth \
--output-dir ./Models/Auth \
--namespace Library.Database.Models.Auth \
--no-onconfiguring \
--no-pluralize \
--force
```

create context by schema

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

## Service.Auth

Jwt Control

Jwt.Key 建立語法

```bash
# 產生後移除所有換行
openssl rand -base64 64 | tr -d '\n'
```

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







