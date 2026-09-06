---
fields:
    - name: Id
      description: The surrogate key of the lockout row.
      type: int

    - name: UserId
      description: The account the failures are against, uniquely indexed so one account cannot accumulate two counters. Cascades with the user.
      type: int

    - name: AccessFailedCount
      description: How many credential checks have failed in a row, counting wrong two-factor codes alongside wrong passwords. It stands at the limit while the lockout it earned runs, and is zeroed by the first attempt made after that lockout has expired, so it counts towards the next lockout rather than a lifetime total. A correct password does not clear it while the account owes a second factor, and does not add to it either; the run survives untouched until that code is accepted.
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

::: danger
`UserLockout` is a database entity. Never return it from an endpoint: it carries the surrogate keys and the failed-attempt count, so map it to a DTO that exposes only the fields the client needs. A row exists only between the first failure and the next completed sign-in, which deletes it rather than zeroing it. For an account owing a second factor that is the code, not the password. An account with no row has nothing against it, which is not the same as having been reset.
:::

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class LockoutAdmin(AppDbContext database)
{
    public Task ClearAsync(int userId)
        => database.UserLockouts
            .Where(lockout => lockout.UserId == userId)
            .ExecuteDeleteAsync();
}
```

<FrontmatterDocs/>
