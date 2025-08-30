using System;
using System.Collections.Generic;

namespace Library.Database.Models.Auth;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public List<string> Roles { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
