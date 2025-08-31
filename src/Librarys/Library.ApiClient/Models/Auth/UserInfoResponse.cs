namespace Library.ApiClient.Models.Auth;

public class UserInfoResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
}
