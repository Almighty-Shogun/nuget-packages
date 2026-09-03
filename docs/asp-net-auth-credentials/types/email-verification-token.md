---
fields:
    - name: Id
      description: The surrogate key. Never leaves the server; the emailed token is the only handle a caller has on this row.
      type: int

    - name: UserId
      description: The user the verification was issued for.
      type: int

    - name: TokenHash
      description: Hash of the token that was emailed. The emailed value cannot be read back out of the database.
      type: string

    - name: Email
      description: The address being verified, stored on the token rather than read from the user so the same flow covers a sign-up and a change of address.
      type: string

    - name: CreatedAt
      description: When the verification was requested.
      type: DateTimeOffset

    - name: ExpiresAt
      description: When the token stops being usable. Fixed at issue rather than extended on each attempt.
      type: DateTimeOffset

    - name: UsedAt
      description: When the token was spent, or null while it is still usable. Set instead of deleting the row, so a second attempt reads as a replay.
      type: DateTimeOffset?
      default: 'null'

    - name: IsActive
      description: Whether the token would still be accepted, meaning unspent and not past its expiry. Computed, not mapped, so it cannot be used in a query.
      type: bool
---

# EmailVerificationToken

One issued email verification. [`AuthDbContext<TUser>`](./auth-db-context) maps the table and exposes the set, but no package service writes to it: issuing, emailing, and redeeming a verification is the application's own flow.

::: warning
Match a presented token by its hash, never by the value itself. [`TokenHasher.Hash`](../utilities/token-hasher#hash) produces the form `TokenHash` holds.
:::

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class EmailVerifier(AppDbContext database)
{
    public async Task<bool> RedeemAsync(string token)
    {
        string hash = TokenHasher.Hash(token);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        EmailVerificationToken? stored = await database.EmailVerificationTokens
            .FirstOrDefaultAsync(candidate => candidate.TokenHash == hash && candidate.UsedAt == null);

        if (stored is null || stored.ExpiresAt <= now)
            return false;

        stored.UsedAt = now;
        await database.SaveChangesAsync();

        return true;
    }
}
```

<FrontmatterDocs/>
