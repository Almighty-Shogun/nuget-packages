# UserSession

Represents a stored refresh-token session for a credential user. The package creates this entity during login and registration, rotates it during session refresh, and revokes it during password-sensitive operations.

Sessions store the owning user id, hashed refresh token, app audience, expiry time, revocation state, request IP address, raw User-Agent, parsed device, and parsed browser. When [JWT Auth app scoping](/asp-net-jwt-auth/configuration/auth-settings) is enabled, refresh requests only match sessions for the current resolved app.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class SessionAuditService(AppDbContext database)
{
    public Task<List<UserSession>> GetActiveSessionsAsync(int userId)
        => database.UserSessions
            .Where(session => session.UserId == userId && session.IsActive)
            .ToListAsync();
}
```

## Type signature

```csharp
[Table("user_sessions")]
public sealed class UserSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string RefreshTokenHash { get; set; } = string.Empty;
    public string App { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Device { get; set; }
    public string? Browser { get; set; }
    public bool IsExpired { get; }
    public bool IsActive { get; }
}
```
