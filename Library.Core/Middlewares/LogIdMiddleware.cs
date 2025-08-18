using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Library.Core.Middlewares;

public class LogIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogIdMiddleware> _logger;

    public LogIdMiddleware(RequestDelegate next, ILogger<LogIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var logId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object> { ["LogId"] = logId }))
        {
            context.Response.Headers["X-Log-Id"] = logId;
            _logger.LogInformation("Handling request {Method} {Path}", context.Request.Method, context.Request.Path);
            await _next(context);
            _logger.LogInformation("Finished handling request.");
        }
    }
}
