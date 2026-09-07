# AuthDbContext

The EF Core base context the package queries through. An application derives its own context from `AuthDbContext<TUser>`, so credential data shares the application's provider and migrations instead of living in a database of its own, though most credential operations open a transaction of their own on that context and throw `InvalidOperationException` when one is already open, while the ones whose whole write is a single statement or a single save open none and run inside the transaction the application has. Every entity names its own snake_case table, and `OnModelCreating` adds the cascades and the unique indexes on username, email, public identifier, and every token hash. The package ships no migrations, so every table below is created by one generated against the derived context.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : AuthDbContext<AppUser>(options)
{
    public DbSet<Project> Projects => Set<Project>();
}
```

```csharp [AppUser.cs]
using AlmightyShogun.AspNet.Auth.Credentials;

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
using AlmightyShogun.AspNet.Auth.Credentials;

AppUser? user = await database.Users
    .FirstOrDefaultAsync(candidate => candidate.Email == email);
```

### Type signature

```csharp
public DbSet<TUser> Users { get; }
```

## UserSessions

The refresh-token sessions in `user_sessions`. Query it to show a user their signed-in devices; use [`IAuthSessionService<TUser>`](../services/auth-session-service) to create, rotate, or revoke one.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

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
using AlmightyShogun.AspNet.Auth.Credentials;

int outstanding = await database.PasswordResetTokens
    .Where(token => token.UsedAt == null)
    .CountAsync(token => token.UserId == user.Id);
```

### Type signature

```csharp
public DbSet<PasswordResetToken> PasswordResetTokens { get; }
```

## EmailVerificationTokens

The issued email verifications in `email_verification_tokens`, written and spent by [`IAuthEmailService`](../services/auth-email-service). A row is marked used rather than deleted, whether it is redeemed, retired by a later request of the same purpose, or retired as a registration link when a change of email is completed, so a cleanup job is what eventually removes them. Those stamps are written by an update statement that bypasses the change tracker, so the row in the database carries the new `UsedAt` while an `EmailVerificationToken` you loaded beforehand keeps the one it was read with. Requests made one after another leave one unspent row per user per purpose; two arriving at once can leave more, since no index enforces the limit.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddDays(-30);

int removed = await database.EmailVerificationTokens
    .Where(token => token.ExpiresAt < cutoff)
    .ExecuteDeleteAsync();
```

### Type signature

```csharp
public DbSet<EmailVerificationToken> EmailVerificationTokens { get; }
```

## UserLockouts

The lockout rows in `user_lockouts`, one per account currently failing sign-in. Empty when lockout is disabled, and the row for an account is deleted the moment a sign-in completes, which for an account owing a second factor is when the code is accepted rather than when the password verifies.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

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
using AlmightyShogun.AspNet.Auth.Credentials;

bool enrolled = await database.UserTwoFactors
    .Where(twoFactor => twoFactor.IsEnabled)
    .AnyAsync(twoFactor => twoFactor.UserId == user.Id);
```

### Type signature

```csharp
public DbSet<UserTwoFactor> UserTwoFactors { get; }
```

## TwoFactorRecoveryCodes

The recovery codes in `two_factor_recovery_codes`, one row per code, marked used when spent rather than deleted.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

int remaining = await database.TwoFactorRecoveryCodes
    .Where(code => code.UsedAt == null)
    .CountAsync(code => code.UserTwoFactorId == enrolmentId);
```

### Type signature

```csharp
public DbSet<TwoFactorRecoveryCode> TwoFactorRecoveryCodes { get; }
```

## TwoFactorChallenges

The sign-ins waiting on a second factor in `two_factor_challenges`, written by [`LoginAsync`](../services/auth-user-service#loginasync) and marked used rather than deleted. Issuing one retires the user's unspent rows and deletes that user's expired ones, so sign-ins made one after another leave a single live row per user and rows accumulate only for accounts that never sign in again; two arriving at once can leave more live, since no index enforces the limit. Those stamps are written by an update statement that bypasses the change tracker, so the row in the database carries the new `UsedAt` while a [`TwoFactorChallenge`](./two-factor-challenge) you loaded beforehand keeps the one it was read with.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

DateTimeOffset now = DateTimeOffset.UtcNow;

int waiting = await database.TwoFactorChallenges
    .Where(challenge => challenge.UsedAt == null)
    .CountAsync(challenge => challenge.ExpiresAt > now);
```

### Type signature

```csharp
public DbSet<TwoFactorChallenge> TwoFactorChallenges { get; }
```
