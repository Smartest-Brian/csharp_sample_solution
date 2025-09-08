namespace Service.Auth.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int AccessTokenExpiresMinutes { get; set; } = 30;
    public int RefreshTokenExpiresDays { get; set; } = 7;
}
