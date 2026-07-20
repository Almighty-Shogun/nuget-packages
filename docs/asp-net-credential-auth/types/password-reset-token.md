# PasswordResetToken

Represents a stored password reset token issued for a credential user. The package stores only the SHA-256 token hash, so the plain reset token is available only when [`RequestForgotPasswordAsync`](../services/auth-password-service#requestforgotpasswordasync) returns it.

Use this entity for audit views, cleanup jobs, or administrative diagnostics. Normal password reset flows should go through [`IAuthPasswordService`](../services/auth-password-service), which creates, validates, marks, and invalidates reset tokens consistently.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class PasswordResetCleanup(AppDbContext database)
{
    public async Task DeleteExpiredAsync()
    {
        List<PasswordResetToken> expiredTokens = await database.PasswordResetTokens
            .Where(token => !token.IsActive)
            .ToListAsync();

        database.PasswordResetTokens.RemoveRange(expiredTokens);
        await database.SaveChangesAsync();
    }
}
```

## Type signature

```csharp
[Table("password_reset_tokens")]
public sealed class PasswordResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? RequestedIpAddress { get; set; }
    public bool IsActive { get; }
}
```
