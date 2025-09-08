using Library.Core.Results;
using Library.Database.Contexts.Auth;
using Library.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Service.Auth.Models.Auth;
using Service.Auth.Options;
using Service.Auth.Services.Jwt;
using Service.Auth.Services.Password;

namespace Service.Auth.Services.Auth;

public class AuthService(
    AuthDbContext db,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    ILogger<AuthService> logger,
    IOptions<JwtOptions> jwtOptions
) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    public async Task<Result<UserResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            bool exists = await db.UserInfo.AnyAsync(u => u.Username == request.Username);
            if (exists) return Result<UserResponse>.Fail("Username already exists");

            (string hash, string salt) = passwordHasher.HashPassword(request.Password);

            if (!request.Roles.All(r => Enum.TryParse<Role>(r, true, out _)))
            {
                return Result<UserResponse>.Fail("Invalid roles");
            }
            List<string> roles = request.Roles
                .Select(r => Enum.Parse<Role>(r, true).ToString().ToLowerInvariant())
                .ToList();

            UserInfo user = new()
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Roles = roles,
                IsActive = true
            };

            db.UserInfo.Add(user);
            await db.SaveChangesAsync();

            UserResponse response = new()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = user.Roles
            };

            return Result<UserResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AuthService.RegisterAsync Error");
            throw;
        }
    }

    public async Task<Result<TokenResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            UserInfo? user = await db.UserInfo.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return Result<TokenResponse>.Fail("Invalid credentials");
            }

            bool valid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!valid) return Result<TokenResponse>.Fail("Invalid credentials");

            string accessToken = jwtService.GenerateAccessToken(user);
            string refreshToken = jwtService.GenerateRefreshToken();

            UserRefreshToken refreshEntity = new()
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(jwtService.RefreshTokenExpiryDays)
            };

            user.LastLoginAt = DateTime.UtcNow;

            db.UserRefreshToken.Add(refreshEntity);
            await db.SaveChangesAsync();

            TokenResponse response = new()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Result<TokenResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AuthService.LoginAsync Error");
            throw;
        }
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            UserInfo? user = await db.UserInfo.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return Result<UserResponse>.Fail("User not found");

            UserResponse response = new()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                Roles = user.Roles
            };

            return Result<UserResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AuthService.GetUserByIdAsync Error");
            throw;
        }
    }

    public async Task<Result<TokenResponse>> RefreshAsync(RefreshRequest request)
    {
        try
        {
            UserRefreshToken? tokenEntity = await db.UserRefreshToken.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (tokenEntity is null || tokenEntity.RevokedAt != null || tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                return Result<TokenResponse>.Fail("Invalid refresh token");
            }

            tokenEntity.RevokedAt = DateTime.UtcNow;

            UserInfo user = tokenEntity.User;

            string newAccessToken = jwtService.GenerateAccessToken(user);
            string newRefreshToken = jwtService.GenerateRefreshToken();

            UserRefreshToken newRefreshEntity = new()
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(jwtService.RefreshTokenExpiryDays)
            };

            db.UserRefreshToken.Add(newRefreshEntity);
            await db.SaveChangesAsync();

            TokenResponse response = new()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return Result<TokenResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AuthService.RefreshAsync Error");
            throw;
        }
    }

    public async Task<Result<ValidateTokenResponse>> ValidateTokenAsync(string token)
    {
        try
        {
            string cleanToken = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? token["Bearer ".Length..]
                : token;

            TokenValidationParameters parameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key))
            };

            JwtSecurityTokenHandler handler = new();
            ClaimsPrincipal principal = handler.ValidateToken(cleanToken, parameters, out _);

            string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Result<ValidateTokenResponse>.Fail("Invalid token");

            Result<UserResponse> userInfo = await GetUserByIdAsync(Guid.Parse(userId));
            if (!userInfo.Success) return Result<ValidateTokenResponse>.Fail("Get user info failed");

            ValidateTokenResponse res = new()
            {
                Id = userInfo.Data!.Id,
                Active = userInfo.Data.IsActive,
                Username = userInfo.Data.Username,
                Roles =  userInfo.Data.Roles,
                Exp = 0,
                Iat = 0
            };

            return Result<ValidateTokenResponse>.Ok(res);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "AuthService.ValidateTokenAsync Error");
            return Result<ValidateTokenResponse>.Fail("Invalid token");
        }
    }
}
