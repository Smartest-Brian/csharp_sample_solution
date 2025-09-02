using Library.ApiClient.Models.Auth;
using Library.Core.Results;

using Refit;

namespace Library.ApiClient.Services.Auth;

public interface IAuthApi
{
    [Get("/api/auth/getUserInfo")]
    Task<ApiResponse<Result<UserInfoResponse>>> GetUserInfoAsync(
        [Header("Authorization")] string authorization);

    [Post("/api/auth/validate")]
    Task<ApiResponse<Result<UserInfoResponse>>> ValidateTokenAsync(
        [Body] ValidateTokenRequest request);
}
