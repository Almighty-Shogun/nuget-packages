---
fields:
    - name: Id
      description: The surrogate key. Never leaves the server; the emailed token is the only handle a caller has on this row.
      type: int

    - name: UserId
      description: The user the reset was issued for, uniquely indexed so an account cannot hold two reset tokens at once. Cascades with the user.
      type: int

    - name: TokenHash
      description: Hash of the token that was emailed, uniquely indexed. The emailed value cannot be recovered from the database.
      type: string

    - name: CreatedAt
      description: When the reset now held was requested. Rewritten each time the user asks for another link, so it dates the current one rather than the first ever issued.
      type: DateTimeOffset

    - name: ExpiresAt
      description: When the token stops being usable, set at issue from `PasswordResetMinutes`.
      type: DateTimeOffset

    - name: UsedAt
      description: When the token was spent, or null while it is still usable. Set instead of deleting the row, so a second attempt reads as a replay; it returns to null when the row is reused for a new request.
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

A user's password reset, at most one row per account. Requesting another rewrites this row rather than adding a second, and a spent row is kept until then, so presenting a spent token is recognised as a replay instead of looking like a token that never existed.

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
