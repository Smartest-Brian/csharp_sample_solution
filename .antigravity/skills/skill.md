# C# 模組化單體與微服務開發架構建置指引 (Skill)

本文件定義了一套用於建置 C# .NET 9.0 模組化單體與微服務架構的標準開發規範與程式碼範本。當 Agent 讀取此 Skill 時，應遵循此架構藍圖來初始化、調整或擴充專案。

---

## 📂 1. 專案拓撲與初始化流程 (Architecture & Setup)

專案結構應劃分為兩個核心目錄：
*   **`src/Librarys/` (共用基礎設施層)**：不可單獨執行的類別庫，提供底層機制與通訊。
*   **`src/Services/` (業務服務層)**：獨立運行的入口專案（Web API、GraphQL 或背景 Worker）。

### 1.1. 專案初始化指令範本
在建立新專案時，請依以下範本在專案根目錄執行：

```bash
# 1. 建立方案
dotnet new sln -n Solution

# 2. 建立業務服務
dotnet new webapi -n Service.WebAPI --use-controllers -o src/Services/Service.WebAPI -f net9.0
dotnet new webapi -n Service.GraphQL -o src/Services/Service.GraphQL -f net9.0
dotnet new webapi -n Service.Job -o src/Services/Service.Job -f net9.0
dotnet new webapi -n Service.Auth -o src/Services/Service.Auth -f net9.0

# 3. 建立共用類別庫
dotnet new classlib -n Library.Core -o src/Librarys/Library.Core -f net9.0
dotnet new classlib -n Library.Database -o src/Librarys/Library.Database -f net9.0
dotnet new classlib -n Library.ApiClient -o src/Librarys/Library.ApiClient -f net9.0
dotnet new classlib -n Library.RabbitMQ -o src/Librarys/Library.RabbitMQ -f net9.0
dotnet new classlib -n Library.Quartz -o src/Librarys/Library.Quartz -f net9.0

# 4. 將專案加入方案管理
dotnet sln Solution.sln add src/Services/Service.WebAPI/Service.WebAPI.csproj --solution-folder services
dotnet sln Solution.sln add src/Services/Service.Auth/Service.Auth.csproj --solution-folder services
dotnet sln Solution.sln add src/Services/Service.GraphQL/Service.GraphQL.csproj --solution-folder services
dotnet sln Solution.sln add src/Services/Service.Job/Service.Job.csproj --solution-folder services
dotnet sln Solution.sln add src/Librarys/Library.Core/Library.Core.csproj --solution-folder libraries
dotnet sln Solution.sln add src/Librarys/Library.Database/Library.Database.csproj --solution-folder libraries
dotnet sln Solution.sln add src/Librarys/Library.ApiClient/Library.ApiClient.csproj --solution-folder libraries
dotnet sln Solution.sln add src/Librarys/Library.RabbitMQ/Library.RabbitMQ.csproj --solution-folder libraries
dotnet sln Solution.sln add src/Librarys/Library.Quartz/Library.Quartz.csproj --solution-folder libraries
```

---

## 🧩 2. 共用基礎設施建置指引 (Common Infrastructures)

### 2.1. 統一回傳格式與錯誤管理 (`Library.Core/Results`)
所有業務服務 API 必須統一回傳格式。

*   **實作封裝範本 [Result.cs](./src/Librarys/Library.Core/Results/Result.cs)**：
    ```csharp
    namespace Library.Core.Results
    {
        public class Result<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }

            public static Result<T> Ok(T data) => new() { Success = true, Data = data };
            public static Result<T> Fail(string error) => new() { Success = false, Message = error };
        }
    }
    ```
*   **自訂靜態錯誤碼 [ErrorCode.cs](./src/Librarys/Library.Core/Results/ErrorCode.cs)**：
    ```csharp
    namespace Library.Core.Results;
    public static class ErrorCodes
    {
        public static class Common
        {
            public const string Unknown = "common.unknown";
            public const string Validation = "common.validation";
            public const string Unauthorized = "common.unauthorized";
            public const string Forbidden = "common.forbidden";
        }
    }
    ```

### 2.2. 全域 HTTP 管道中間件 (`Library.Core/Middlewares`)
所有業務 API 均需掛載以下兩項 Middleware：

*   **請求追蹤 [RequestIdMiddleware.cs](./src/Librarys/Library.Core/Middlewares/RequestIdMiddleware.cs)**：
    ```csharp
    using Microsoft.AspNetCore.Http;
    namespace Library.Core.Middlewares;
    public class RequestIdMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.TraceIdentifier;
            context.Items["RequestId"] = requestId;
            context.Response.Headers["X-Request-ID"] = requestId;
            await next(context);
        }
    }
    ```
*   **全域異常捕捉 [GlobalExceptionMiddleware.cs](./src/Librarys/Library.Core/Middlewares/GlobalExceptionMiddleware.cs)**：
    ```csharp
    using System.Net;
    using System.Text.Json;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    namespace Library.Core.Middlewares;
    public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try { await next(context); }
            catch (Exception ex)
            {
                var requestId = context.TraceIdentifier;
                logger.LogError(ex, $"Unhandled exception. RequestId: {requestId}");
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var problem = new ProblemDetails {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = "Please contact support with the provided request id.",
                    Extensions = { ["requestId"] = requestId }
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
    ```

### 2.3. 跨服務通訊與權限攔截 (`Library.ApiClient`)
採用 Refit 作為宣告式 HTTP API 客戶端，並實作自訂 Action Filter 保護 API 控制器。

*   **Refit 客戶端介面範本 [IAuthApi.cs](./src/Librarys/Library.ApiClient/Services/Auth/IAuthApi.cs)**：
    ```csharp
    using Refit;
    using Library.Core.Results;
    namespace Library.ApiClient.Services.Auth;
    public interface IAuthApi
    {
        [Post("/auth/validate")]
        Task<ApiResponse<Result<ValidateTokenResponse>>> ValidateTokenAsync([Body] ValidateTokenRequest request);
    }
    ```
*   **權限驗證過濾器 [ValidateTokenAttribute.cs](./src/Librarys/Library.ApiClient/Attributes/ValidateTokenAttribute.cs)**：
    ```csharp
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Library.ApiClient.Services.Auth;
    using Library.Core.Results;

    namespace Library.ApiClient.Attributes;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateTokenAttribute : Attribute, IAsyncActionFilter
    {
        public string[]? Roles { get; set; }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var header) || header.Count == 0)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            string token = header.ToString().Replace("Bearer ", "").Trim();
            var authApi = context.HttpContext.RequestServices.GetRequiredService<IAuthApi>();
            var response = await authApi.ValidateTokenAsync(new ValidateTokenRequest { Token = token });

            if (!response.IsSuccessStatusCode || response.Content?.Data is null || !response.Content.Success)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var tokenInfo = response.Content.Data;
            if (Roles is { Length: > 0 } && !Roles.Any(role => tokenInfo.Roles.Contains(role)))
            {
                context.Result = new ObjectResult(Result<object>.Fail("User does not have the required permissions")) { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }
            await next();
        }
    }
    ```

### 2.4. 訊息佇列封裝與事件分發 (`Library.RabbitMQ`)
使用事件驅動時，統一封裝連線機制與非同步訂閱接收。

*   **RabbitMQ 服務實作 [RabbitMqService.cs](./src/Librarys/Library.RabbitMQ/Services/RabbitMqService.cs)**：
    ```csharp
    using System.Text;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    namespace Library.RabbitMQ.Services;
    public class RabbitMqService : IRabbitMqService, IDisposable
    {
        private IConnection? _connection;
        private IModel? _channel;
        public event Func<string, string, Task>? MessageReceived;

        public Task PublishAsync(string exchange, string routingKey, string message)
        {
            // 初始化 Channel 並發布 Persistent 訊息
            byte[] body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange, routingKey, basicProperties: null, body: body);
            return Task.CompletedTask;
        }

        public void Subscribe(string exchange, string queue, string exchangeType, params string[] routingKeys)
        {
            _channel.ExchangeDeclare(exchange, exchangeType, durable: true);
            _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
            foreach (var key in routingKeys) _channel.QueueBind(queue, exchange, key);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) => {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                if (MessageReceived != null) await MessageReceived.Invoke(ea.RoutingKey, message);
            };
            _channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
        }
        // Dispose 邏輯略...
    }
    ```

---

## ⚡ 3. 業務服務整合與開發規範 (Service Layer Implementation)

各個可運行的專案在各自的啟動流程中組裝上述元件：

### 3.1. 控制器範本 (RESTful API Controller)
業務 API 的開發應遵循強型別 Token 校驗與非同步處理原則：

```csharp
using Microsoft.AspNetCore.Mvc;
using Library.ApiClient.Attributes;
using Library.Core.Results;
using Library.RabbitMQ.Services;

[ApiController]
[Route("[controller]")]
public class DemoController(IRabbitMqService rabbitMqService) : ControllerBase
{
    [ValidateToken(Roles = ["admin"])] // 整合跨服務權限驗證
    [HttpPost("action")]
    public async Task<IActionResult> ExecuteAction([FromBody] ActionRequest request)
    {
        // 執行商業邏輯
        var result = Result<string>.Ok("Success");
        
        // 觸發事件傳播
        await rabbitMqService.PublishAsync("exchange.demo", "key.demo", JsonSerializer.Serialize(request));
        
        return Ok(result);
    }
}
```

### 3.2. 排程服務與事件訂閱佇列整合 (`Service.Job`)
在 Worker / BackgroundJob 專案中，應整合 Quartz.NET 與 RabbitMQ 消費者：

*   **事件訂閱配置範本 [Program.cs](./src/Services/Service.Job/Program.cs)**：
    ```csharp
    private static void ConfigRabbitMqSubscriber(WebApplication app)
    {
        var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
        var rabbitMqService = app.Services.GetRequiredService<IRabbitMqService>();

        rabbitMqService.Subscribe("exchange.demo", "queue.demo", ExchangeType.Direct, "key.demo");
        rabbitMqService.MessageReceived += async (routingKey, message) => {
            if (routingKey == "key.demo")
            {
                var data = new JobDataMap { ["message"] = message };
                // 收到 MQ 事件後，動態觸發 Quartz Job 進行非同步處理
                await scheduler.TriggerJob(new JobKey("JOB-DemoJob", "STATIC"), data);
            }
        };
    }
    ```

---

## 📏 4. 開發與提交標準規範 (Standards)

1.  **程式碼排版與自動修正**：
    送交程式碼前，必須在方案目錄下執行 `dotnet format Solution.sln`，確保排版風格（空白、縮排、Using 引用順序）符合 `.editorconfig` 規範。
2.  **I/O 非阻塞**：
    凡涉及資料庫（EF Core）、外部呼叫（Refit Client）或佇列發送（RabbitMQ）之操作，一律優先使用非同步方法（`async/await`）。
3.  **無敏感資訊硬編碼 (No Hardcoded Secrets)**：
    資料庫連線字串、RabbitMQ 密碼與 JWT 金鑰等，嚴禁直接寫死在程式碼中，一律放置在 `appsettings.json` 或透過容器環境變數注入。
