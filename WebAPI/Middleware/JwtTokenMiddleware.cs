using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace WebAPI.Middleware
{
    /// <summary>
    /// Simple middleware that ensures each request contains a JWT token
    /// in the Authorization header using the Bearer scheme.
    /// </summary>
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            var hasToken = context.Request.Headers.TryGetValue("Authorization", out var authHeader)
                && authHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

            if (!hasToken)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("JWT token is missing.");
                return;
            }

            await _next(context);
        }
    }
}
