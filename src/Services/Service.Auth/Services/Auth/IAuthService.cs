using Library.Core.Results;

using Service.Auth.Models.Auth;

namespace Service.Auth.Services.Auth;

public interface IAuthService
{
    Task<Result<UserResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<TokenResponse>> LoginAsync(LoginRequest request);
    Task<Result<UserResponse>> GetUserByIdAsync(Guid userId);
    Task<Result<TokenResponse>> RefreshAsync(RefreshRequest request);
}
