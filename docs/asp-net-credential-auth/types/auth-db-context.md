# AuthDbContext

Base EF Core context for Credential Auth storage. Application code derives from `AuthDbContext<TUser>` so the package can query users, refresh-token sessions, and password reset tokens through a known contract while the application remains in control of its provider, migrations, and additional domain tables.

The base context configures relationships and indexes for credential users, sessions, and reset tokens. It adds unique indexes for username, email, refresh token, and password reset token hash, and it cascades session/reset-token data when a user is deleted.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : AuthDbContext<AppUser>(options)
{
    public DbSet<Project> Projects => Set<Project>();
}
```

```csharp [AppUser.cs]
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppUser : AuthUser
{
    public string DisplayName { get; set; } = string.Empty;
}
```

:::

## Type signature

```csharp
public abstract class AuthDbContext<TUser>(DbContextOptions options) : DbContext(options)
    where TUser : AuthUser
{
    public DbSet<TUser> Users { get; }
    public DbSet<UserSession> UserSessions { get; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; }
}
```
