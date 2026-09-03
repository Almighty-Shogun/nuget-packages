# AuthDbContext

The EF Core base context the package queries through. An application derives its own context from `AuthDbContext<TUser>`, so credential data shares the application's provider, migrations, and transactions instead of living in a database of its own. Every entity names its own snake_case table, and `OnModelCreating` adds the cascades and the unique indexes on username, email, public identifier, and every token hash.

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

```csharp [Project.cs]
public sealed class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

:::

::: warning
Call `base.OnModelCreating(modelBuilder)` first when overriding it. Skipping it leaves every credential relationship and index unconfigured, which surfaces as missing columns at query time rather than as a failure at startup.
:::

## Users

The credential users in `users`, typed as the application's own `TUser`. Login matches `Username` and `Email` against this set, and both are uniquely indexed.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

AppUser? user = await database.Users.FirstOrDefaultAsync(candidate => candidate.Email == email);
```

### Type signature

```csharp
public DbSet<TUser> Users { get; }
```

## UserSessions

The refresh-token sessions in `user_sessions`. Query it to show a user their signed-in devices; use [`IAuthSessionService<TUser>`](../services/auth-session-service) to create, rotate, or revoke one.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

List<UserSession> sessions = await database.UserSessions
    .Where(session => session.UserId == user.Id && !session.IsRevoked)
    .ToListAsync();
```

### Type signature

```csharp
public DbSet<UserSession> UserSessions { get; }
```

## PasswordResetTokens

The password resets in `password_reset_tokens`, at most one row per user. A spent row is marked used rather than deleted and stays until that user requests another reset and it is reused, so a cleanup job is what eventually removes the rest.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

int outstanding = await database.PasswordResetTokens
    .CountAsync(token => token.UserId == user.Id && token.UsedAt == null);
```

### Type signature

```csharp
public DbSet<PasswordResetToken> PasswordResetTokens { get; }
```

## EmailVerificationTokens

The issued email verifications in `email_verification_tokens`. The table is mapped and cascades with the user, but no package service writes to it; issuing and redeeming one is the application's own flow.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

string hash = TokenHasher.Hash(token);

EmailVerificationToken? verification = await database.EmailVerificationTokens
    .FirstOrDefaultAsync(stored => stored.TokenHash == hash && stored.UsedAt == null);
```

### Type signature

```csharp
public DbSet<EmailVerificationToken> EmailVerificationTokens { get; }
```

## UserLockouts

The lockout rows in `user_lockouts`, one per account currently failing sign-in. Empty when lockout is disabled, and the row for an account is deleted the moment it signs in successfully.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

DateTimeOffset now = DateTimeOffset.UtcNow;

List<UserLockout> locked = await database.UserLockouts
    .Where(lockout => lockout.LockoutEnd > now)
    .ToListAsync();
```

### Type signature

```csharp
public DbSet<UserLockout> UserLockouts { get; }
```

## UserTwoFactors

The TOTP enrolments in `user_two_factors`, one per user at most. Read it to ask whether a user is enrolled; everything that changes an enrolment goes through [`IAuthTwoFactorService<TUser>`](../services/auth-two-factor-service).

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

bool enrolled = await database.UserTwoFactors
    .AnyAsync(twoFactor => twoFactor.UserId == user.Id && twoFactor.IsEnabled);
```

### Type signature

```csharp
public DbSet<UserTwoFactor> UserTwoFactors { get; }
```

## TwoFactorRecoveryCodes

The recovery codes in `two_factor_recovery_codes`, one row per code, marked used when spent rather than deleted.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

int remaining = await database.TwoFactorRecoveryCodes
    .CountAsync(code => code.UserTwoFactorId == enrolmentId && code.UsedAt == null);
```

### Type signature

```csharp
public DbSet<TwoFactorRecoveryCode> TwoFactorRecoveryCodes { get; }
```
