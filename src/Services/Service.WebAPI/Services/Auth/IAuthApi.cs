using Library.Core.Results;
using Refit;
using Service.WebAPI.Models.Auth;

namespace Service.WebAPI.Services.Auth;

public interface IAuthApi
{
    [Get("/api/auth/getUserInfo")]
    Task<ApiResponse<Result<UserInfoResponse>>> GetUserInfoAsync(
        [Header("Authorization")] string authorization);
}
