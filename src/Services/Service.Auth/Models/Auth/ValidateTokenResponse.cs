namespace Service.Auth.Models.Auth;

public class ValidateTokenResponse
{
    public Guid Id { get; set; }
    public bool Active { get; set; }
    public string? Username { get; set; }
    public List<string> Roles { get; set; } = new();
    public long Exp { get; set; }
    public long Iat { get; set; }
}
