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
