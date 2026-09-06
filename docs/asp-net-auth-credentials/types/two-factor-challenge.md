---
fields:
    - name: Id
      description: The surrogate key. Never leaves the server; the challenge handed to the client is the only handle a caller has on this row.
      type: int

    - name: UserId
      description: The user whose password verified. Cascades with the user, so removing an account takes its outstanding challenges with it. Not uniquely indexed, so a spent row stays beside the live one.
      type: int

    - name: TokenHash
      description: Hash of the challenge that was returned, uniquely indexed. The value the client holds cannot be read back out of the table.
      type: string

    - name: App
      description: The application the sign-in came through, or null when the deployment is not app-scoped. Completion resolves its own request's host and refuses a challenge whose value differs, so the code has to be presented to the application the password was sent to and the session ends up scoped to it.
      type: string?
      default: 'null'

    - name: CreatedAt
      description: When the password verified and this challenge was issued, set as the row is written and never changed afterwards.
      type: DateTimeOffset

    - name: ExpiresAt
      description: When the challenge stops being redeemable, set at issue from [`ChallengeMinutes`](../configuration). It bounds how long the code prompt may sit open, not how long the session it leads to lasts.
      type: DateTimeOffset

    - name: UsedAt
      description: When the challenge was spent, or null while it is still redeemable. Stamped when a code completes the sign-in, when a later sign-in for the same user retires it, so only the newest row is ever live, and when [`ChangePasswordAsync`](../services/auth-password-service#changepasswordasync) or [`CompleteForgotPasswordAsync`](../services/auth-password-service#completeforgotpasswordasync) sets a new password. The first two stamps are written by an update statement that bypasses the change tracker, so the row in the database carries the new value while an instance loaded beforehand keeps the one it was read with.
      type: DateTimeOffset?
      default: 'null'

    - name: RequestedIpAddress
      description: The address the password was sent from, truncated to 45 characters. Kept for auditing a sign-in that stopped at the second factor.
      type: string?
      default: 'null'

    - name: IsActive
      description: Whether the challenge would still be accepted, meaning unspent and not past its expiry. Completion tests both conditions in the database and claims the row with a guarded update, so an active row is not necessarily one that request will get to spend. Computed, not mapped, so it cannot be used in a query.
      type: bool
---

# TwoFactorChallenge

One sign-in that got past the password and is waiting on a second factor, written by [`LoginAsync`](../services/auth-user-service#loginasync) and spent by [`CompleteTwoFactorLoginAsync`](../services/auth-user-service#completetwofactorloginasync). A spent row is kept rather than deleted, so presenting the same challenge twice is refused exactly as an expired one is. Issuing a challenge retires the user's unspent ones and deletes the ones that have expired, so sign-ins made one after another leave a single live row per user; two arriving at once can leave more, since no index enforces the limit.

::: danger
`TwoFactorChallenge` is a database entity. Never return it from an endpoint: it carries the challenge hash, the requesting address, and the surrogate keys. Query it by token hash rather than by the value the client holds, which the table never holds; [`TokenHasher.Hash`](../utilities/token-hasher#hash) produces the form `TokenHash` holds.
:::

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class TwoFactorChallengeCleanup(AppDbContext database)
{
    public async Task DeleteExpiredAsync()
    {
        DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddDays(-7);

        await database.TwoFactorChallenges
            .Where(challenge => challenge.ExpiresAt < cutoff)
            .ExecuteDeleteAsync();
    }
}
```

<FrontmatterDocs/>
