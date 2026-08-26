---
fields:
    - name: Id
      description: The surrogate key of the lockout row.
      type: int

    - name: UserId
      description: The account the failures are against, uniquely indexed so one account cannot accumulate two counters. Cascades with the user.
      type: int

    - name: AccessFailedCount
      description: How many sign-ins have failed in a row. Reset to zero when the limit is reached and the lockout is applied, so it counts towards the next lockout rather than a lifetime total.
      type: int
      default: '0'

    - name: LockoutEnd
      description: When the current lockout expires, or null while failures are only being counted. It lapses on its own, so no administrator is needed to restore access.
      type: DateTimeOffset?
      default: 'null'

    - name: IsLocked
      description: Whether a lockout is in force right now, rather than merely set at some point. Computed, not mapped, so it cannot be used in a query.
      type: bool
---

# UserLockout

One account's run of failed sign-ins and the lockout it earned. Held in its own table, so a deployment that leaves [lockout](../configuration) disabled never writes here and the user table carries no columns it does not use.

::: warning
A row exists only between the first failure and the next success, and signing in deletes it rather than zeroing it. An account with no row has nothing against it, which is not the same as having been reset.
:::

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class LockoutAdmin(AppDbContext database)
{
    public Task ClearAsync(int userId)
        => database.UserLockouts
            .Where(lockout => lockout.UserId == userId)
            .ExecuteDeleteAsync();
}
```

<FrontmatterDocs/>
