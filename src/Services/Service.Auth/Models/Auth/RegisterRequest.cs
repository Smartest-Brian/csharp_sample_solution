namespace Service.Auth.Models.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
