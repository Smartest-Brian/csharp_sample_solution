using Library.Core.Results;
using Library.Database.Contexts.Auth;
using Library.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

using Service.Auth.Models.Auth;
using Service.Auth.Services.Jwt;
using Service.Auth.Services.Password;

namespace Service.Auth.Services.Auth;

public class AuthService(
    AuthDbContext db,
    IPasswordHasher passwordHasher,
    IJwtService jwtService,
    ILogger<AuthService> logger
) : IAuthService
{
    public async Task<Result<UserResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            bool exists = await db.Users.AnyAsync(u => u.Username == request.Username);
            if (exists) return Result<UserResponse>.Fail("Username already exists");

            (string hash, string salt) = passwordHasher.HashPassword(request.Password);

            if (!request.Roles.All(r => Enum.TryParse<Role>(r, true, out _)))
            {
                return Result<UserResponse>.Fail("Invalid roles");
            }
            List<string> roles = request.Roles
                .Select(r => Enum.Parse<Role>(r, true).ToString().ToLowerInvariant())
                .ToList();

            User user = new()
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Roles = roles,
                IsActive = true
            };

            db.Users.Add(user);
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
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return Result<TokenResponse>.Fail("Invalid credentials");
            }

            bool valid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!valid) return Result<TokenResponse>.Fail("Invalid credentials");

            string accessToken = jwtService.GenerateAccessToken(user);
            string refreshToken = jwtService.GenerateRefreshToken();

            RefreshToken refreshEntity = new()
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(jwtService.RefreshTokenExpiryDays)
            };

            user.LastLoginAt = DateTime.UtcNow;

            db.RefreshTokens.Add(refreshEntity);
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
            User? user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null) return Result<UserResponse>.Fail("User not found");

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
            logger.LogError(ex, "AuthService.GetUserByIdAsync Error");
            throw;
        }
    }

    public async Task<Result<TokenResponse>> RefreshAsync(RefreshRequest request)
    {
        try
        {
            RefreshToken? tokenEntity = await db.RefreshTokens.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (tokenEntity is null || tokenEntity.RevokedAt != null || tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                return Result<TokenResponse>.Fail("Invalid refresh token");
            }

            tokenEntity.RevokedAt = DateTime.UtcNow;

            User user = tokenEntity.User;

            string newAccessToken = jwtService.GenerateAccessToken(user);
            string newRefreshToken = jwtService.GenerateRefreshToken();

            RefreshToken newRefreshEntity = new()
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(jwtService.RefreshTokenExpiryDays)
            };

            db.RefreshTokens.Add(newRefreshEntity);
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
}
