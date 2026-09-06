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
      description: The address being verified, which `Purpose` says how to read. For a registration it repeats the address the user already holds; for a change it is the pending one, written onto the user at redemption.
      type: string

    - name: Purpose
      description: Which flow the token was issued for, one of the [`EmailVerificationPurpose`](./email-verification-purpose) values. Redemption requires it to match the method presented with the token, so a change link cannot be spent on the registration endpoint.
      type: EmailVerificationPurpose
      default: Registration

    - name: CreatedAt
      description: When the verification was requested.
      type: DateTimeOffset

    - name: ExpiresAt
      description: When the token stops being usable. Fixed at issue from [`EmailVerificationMinutes`](../configuration) rather than extended on each attempt.
      type: DateTimeOffset

    - name: UsedAt
      description: When the token was spent, or null while it is still usable. Stamped at redemption, when a later request of the same purpose retires it, and, on a registration token, when a change of email is redeemed, whether or not that change moves the account off the address the token names, so the row stays in the table every time.
      type: DateTimeOffset?
      default: 'null'

    - name: IsActive
      description: Whether the token would still be accepted, meaning unspent and not past its expiry. Neither `Purpose` nor, for a registration, whether `Email` is still the account's is part of it, so an active row is not necessarily one the endpoint at hand will take. Computed, not mapped, so it cannot be used in a query.
      type: bool
---

# EmailVerificationToken

One issued email verification, written and spent by [`IAuthEmailService`](../services/auth-email-service). A spent row is kept rather than deleted, so a second click on the same link is refused exactly as an expired one is. Requesting a token retires the user's unspent ones of that purpose, so requests made one after another leave a single live token per `Purpose`; two arriving at once can leave both, since no index enforces the limit.

::: danger
`EmailVerificationToken` is a database entity. Never return it from an endpoint: it carries the token hash and the surrogate keys, so map it to a DTO that exposes only the fields the client needs. Query it by token hash rather than by the emailed value, which the table never holds; [`TokenHasher.Hash`](../utilities/token-hasher#hash) produces the form `TokenHash` holds.
:::

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class PendingEmailChangeReader(AppDbContext database)
{
    public async Task<string?> GetPendingAddressAsync(int userId)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        EmailVerificationToken? pending = await database
            .EmailVerificationTokens
            .Where(token => token.UserId == userId && token.UsedAt == null)
            .Where(token => token.Purpose == EmailVerificationPurpose.EmailChange)
            .FirstOrDefaultAsync(token => token.ExpiresAt > now);

        return pending?.Email;
    }
}
```

<FrontmatterDocs/>
