using Library.ApiClient.Models.Auth;
using Library.ApiClient.Services.Auth;
using Library.Core.Results;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string authorization = context.HttpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        IAuthApi? authApi = context.HttpContext.RequestServices.GetService(typeof(IAuthApi)) as IAuthApi;
        if (authApi is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        ApiResponse<Result<ValidateTokenResponse>> response = await authApi.ValidateTokenAsync(new ValidateTokenRequest
        {
            Token = authorization
        });

        if (!response.IsSuccessStatusCode || response.Content?.Data is null || !response.Content.Success)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["TokenInfo"] = response.Content.Data;

        await next();
    }
}
