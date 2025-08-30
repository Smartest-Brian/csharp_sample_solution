using Library.Database.Models.Auth;

namespace Service.Auth.Services.Jwt;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int RefreshTokenExpiryDays { get; }
}
