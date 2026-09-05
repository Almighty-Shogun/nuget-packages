---
fields:
    - name: Id
      description: The surrogate key of the recovery code row.
      type: int

    - name: UserTwoFactorId
      description: The enrolment the code belongs to. Cascades with the enrolment, so disabling two-factor removes the codes with it.
      type: int

    - name: CodeHash
      description: Hash of the code that was shown to the user. The code itself is never stored, so a lost set cannot be recovered and must be reissued.
      type: string

    - name: UsedAt
      description: When the code was spent, or null while it is still usable. Set instead of deleting the row, so a replayed code is recognised rather than looking unknown.
      type: DateTimeOffset?
      default: 'null'
---

# TwoFactorRecoveryCode

One single-use recovery code, accepted by [`VerifyAsync`](../services/auth-two-factor-service#verifyasync) in place of a TOTP code when a user has lost their authenticator.

Codes are issued as a set by [`CompleteEnrolmentAsync`](../services/auth-two-factor-service#completeenrolmentasync) and returned in plain text exactly once. One row per code means spending one is a single update rather than a rewrite of the whole set.

::: danger
`TwoFactorRecoveryCode` is a database entity. Never return it from an endpoint: it carries the code hash and the surrogate keys. Map it to a DTO that exposes only the fields the client needs.
:::

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class RecoveryCodeCounter(AppDbContext database)
{
    public Task<int> CountRemainingAsync(int enrolmentId)
        => database.TwoFactorRecoveryCodes
            .Where(code => code.UsedAt == null)
            .CountAsync(code => code.UserTwoFactorId == enrolmentId);
}
```

<FrontmatterDocs/>
