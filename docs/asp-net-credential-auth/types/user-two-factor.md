---
fields:
    - name: Id
      description: The surrogate key of the enrolment row.
      type: int

    - name: UserId
      description: The enrolled user, uniquely indexed so an account cannot hold two enrolments. Cascades with the user.
      type: int

    - name: IsEnabled
      description: Whether enrolment was confirmed. Set only when `CompleteEnrolmentAsync` verifies a code, so a row exists while enrolment is still half-finished.
      type: bool
      default: 'false'

    - name: Secret
      description: The shared secret, encrypted with ASP.NET Core data protection rather than hashed, because verification needs the original value back.
      type: string

    - name: LastWindow
      description: The time step of the last accepted code. A code from that step or earlier is refused, so an intercepted code cannot be replayed inside its own window.
      type: long?
      default: 'null'

    - name: CreatedAt
      description: When the enrolment row was first created, which is when the user began enrolling rather than when they finished.
      type: DateTimeOffset

    - name: RecoveryCodes
      description: The single-use codes issued at enrolment. One row each, so spending one is an update rather than a rewrite of the whole set.
      type: 'List<TwoFactorRecoveryCode>'
      default: '[]'
---

# UserTwoFactor

One user's TOTP enrolment, held in its own table so signing in does not load a secret and a set of recovery codes it will not use.

Reach it through [`IAuthTwoFactorService<TUser>`](../services/auth-two-factor-service) for anything that changes it. Read it directly only to ask whether a user is enrolled, which is what gating a login on a second factor needs.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class TwoFactorGate(AppDbContext database)
{
    public Task<bool> IsRequiredAsync(int userId)
        => database.UserTwoFactors.AnyAsync(twoFactor => twoFactor.UserId == userId && twoFactor.IsEnabled);
}
```

<FrontmatterDocs/>
