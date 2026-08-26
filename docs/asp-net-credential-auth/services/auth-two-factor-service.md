# AuthTwoFactorService

Enrols a user in TOTP two-factor authentication and verifies their codes. Application code depends on `IAuthTwoFactorService<TUser>`; the shared secret is encrypted with ASP.NET Core data protection and the recovery codes are stored only as hashes.

::: warning
The package stores and verifies the second factor but never requires it. [`LoginAsync`](./auth-user-service#loginasync) succeeds on a correct password whether or not the user is enrolled, so gate the flow yourself on `user.TwoFactor?.IsEnabled` and decide when a code is demanded.
:::

## BeginEnrolmentAsync

Generates a secret, stores it encrypted, and returns it alongside an `otpauth://` URI to render as a QR code. Calling it again replaces the previous secret and discards any recovery codes, so a user who lost their authenticator can start over.

Two-factor is not on yet. It becomes active only once [`CompleteEnrolmentAsync`](#completeenrolmentasync) verifies a code, so abandoning enrolment halfway cannot lock a user out of their own account. `issuer` is the label an authenticator app shows, and is ignored when [`TwoFactorPolicy.Issuer`](../configuration) is set.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Authorize]
[Route("auth/two-factor")]
public sealed class TwoFactorController(
    IAuthTwoFactorService<AppUser> twoFactor
) : ControllerBase
{
    [HttpPost("begin")]
    public async Task<ActionResult<AuthTwoFactorResult>> Begin(CancellationToken cancellationToken)
        => Ok(await twoFactor.BeginEnrolmentAsync(User.GetCurrentUserId(), "Example", cancellationToken));
}
```

### Type signature

```csharp
public Task<AuthTwoFactorResult> BeginEnrolmentAsync(
    Guid identifier,
    string issuer,
    CancellationToken cancellationToken = default
);
```

## CompleteEnrolmentAsync

Verifies a code from the authenticator, turns two-factor on, and returns freshly generated recovery codes. Only hashes are kept, so these are shown to the user once and cannot be produced again; calling this method a second time issues a new set and voids the old one.

Throws [`InvalidTwoFactorCodeException`](../exceptions) when the code is wrong, and when the user has no enrolment to complete. How many codes are issued is set by [`TwoFactorPolicy.RecoveryCodeCount`](../configuration).

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

IReadOnlyList<string> recoveryCodes = await twoFactor.CompleteEnrolmentAsync(User.GetCurrentUserId(), code);
```

### Type signature

```csharp
public Task<IReadOnlyList<string>> CompleteEnrolmentAsync(
    Guid identifier,
    string code,
    CancellationToken cancellationToken = default
);
```

## VerifyAsync

Checks a submitted value as a TOTP code first and as a recovery code second, spending the recovery code when it matches. An accepted code's time step is recorded, so the same code cannot be presented twice even while it is still within its window.

Returns `false` for a wrong code, an unreadable secret, and a recovery code that was already spent, rather than throwing: at sign-in a wrong code is an ordinary outcome the caller decides how to report. A user with no enrolment at all still throws [`InvalidTwoFactorCodeException`](../exceptions), so call this only for a user you have already established is enrolled.

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

bool accepted = await twoFactor.VerifyAsync(userId, code);
```

### Type signature

```csharp
public Task<bool> VerifyAsync(
    Guid identifier,
    string code,
    CancellationToken cancellationToken = default
);
```

## DisableAsync

Deletes the enrolment along with every recovery code, returning the account to password-only sign-in. A user with no enrolment is not an error and nothing is written.

Demand a fresh password or a valid code before calling this. Otherwise a stolen session is enough to strip the second factor off an account.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

await twoFactor.DisableAsync(User.GetCurrentUserId());
```

### Type signature

```csharp
public Task DisableAsync(
    Guid identifier,
    CancellationToken cancellationToken = default
);
```
