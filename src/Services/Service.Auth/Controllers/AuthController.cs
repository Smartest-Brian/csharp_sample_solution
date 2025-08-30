using System.Security.Claims;

using Library.Core.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Service.Auth.Models.Auth;
using Service.Auth.Services.Auth;

namespace Service.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        Result<UserResponse> result = await authService.RegisterAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        Result<TokenResponse> result = await authService.LoginAsync(request);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        Result<UserResponse> result = await authService.GetUserByIdAsync(Guid.Parse(userId));
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        Result<TokenResponse> result = await authService.RefreshAsync(request);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
