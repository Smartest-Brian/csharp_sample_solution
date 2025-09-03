namespace Service.Auth.Models.Auth;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
