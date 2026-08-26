---
fields:
    - name: Id
      description: The surrogate key. Never leaves the server; the emailed token is the only handle a caller has on this row.
      type: int

    - name: UserId
      description: The user the reset was issued for. Cascades with the user.
      type: int

    - name: TokenHash
      description: Hash of the token that was emailed, uniquely indexed. The emailed value cannot be recovered from the database.
      type: string

    - name: CreatedAt
      description: When the reset was requested. Kept after the token is spent, so a burst of requests against one account stays visible.
      type: DateTimeOffset

    - name: ExpiresAt
      description: When the token stops being usable, set at issue from `PasswordResetMinutes`.
      type: DateTimeOffset

    - name: UsedAt
      description: When the token was spent, or null while it is still usable. Set instead of deleting the row, so a second attempt reads as a replay.
      type: DateTimeOffset?
      default: 'null'

    - name: RequestedIpAddress
      description: The address the reset was requested from, when the caller passed one. Kept for auditing a reset the account owner did not ask for.
      type: string?
      default: 'null'

    - name: IsActive
      description: Whether the token would still be accepted, meaning unspent and not past its expiry. Computed, not mapped, so it cannot be used in a query.
      type: bool
---

# PasswordResetToken

One issued password reset. Rows are kept after use rather than deleted, so presenting a spent token is recognised as a replay instead of looking like a token that never existed.

Normal flows go through [`IAuthPasswordService`](../services/auth-password-service), which issues, redeems, and invalidates these consistently. Read the entity directly for audit views and cleanup jobs.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class PasswordResetCleanup(AppDbContext database)
{
    public async Task DeleteSpentAsync()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        await database.PasswordResetTokens
            .Where(token => token.UsedAt != null || token.ExpiresAt <= now)
            .ExecuteDeleteAsync();
    }
}
```

<FrontmatterDocs/>
