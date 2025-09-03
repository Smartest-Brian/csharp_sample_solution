using System;
using System.Linq;

using Library.ApiClient.Constants;
using Library.ApiClient.Models.Auth;
using Library.ApiClient.Services.Auth;
using Library.Core.Results;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace Library.ApiClient.Attributes;

/// <summary>
/// Action filter attribute that validates the Authorization token by calling the Auth API.
/// If the token is invalid or missing, an <see cref="UnauthorizedResult"/> is returned.
/// When the token is valid, the user information will be stored in <see cref="HttpContext.Items"/> with key <c>UserInfo</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateTokenAttribute : Attribute, IAsyncActionFilter
{
    public string? Roles { get; set; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var header) || header.Count == 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string? authorization = header.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string token = authorization["Bearer ".Length..].Trim();
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        IAuthApi authApi = context.HttpContext.RequestServices.GetRequiredService<IAuthApi>();

        ApiResponse<Result<ValidateTokenResponse>> response = await authApi.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = token
        });

        if (!response.IsSuccessStatusCode || response.Content?.Data is null || !response.Content.Success)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        ValidateTokenResponse tokenInfo = response.Content.Data;
        context.HttpContext.Items[HttpContextItemKeys.TokenInfo] = tokenInfo;

        if (!string.IsNullOrWhiteSpace(Roles))
        {
            string[] requiredRoles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!requiredRoles.Any(role => tokenInfo.Roles.Contains(role)))
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }
}
