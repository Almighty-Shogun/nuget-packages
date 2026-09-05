# AuthTwoFactorService

Enrols a user in TOTP two-factor authentication and verifies their codes. Application code depends on `IAuthTwoFactorService<TUser>`; the shared secret is encrypted with ASP.NET Core data protection and the recovery codes are stored only as hashes.

::: warning
The package stores and verifies the second factor but never requires it. [`LoginAsync`](./auth-user-service#loginasync) succeeds on a correct password whether or not the user is enrolled, so gate the flow yourself on `user.TwoFactor?.IsEnabled` and decide when a code is demanded.
:::

## BeginEnrolmentAsync

Generates a secret, stores it encrypted as a pending enrolment, and returns it alongside an `otpauth://` URI to render as a QR code. Calling it again discards the previous pending secret and offers a fresh one, so only the newest QR can be confirmed.

A second factor already in force is left untouched, codes and all, until [`CompleteEnrolmentAsync`](#completeenrolmentasync) verifies a code against the pending secret. Abandoning enrolment halfway therefore changes nothing, and the pending secret stops being confirmable [`TwoFactorPolicy.PendingSecretMinutes`](../configuration) after it was issued. `issuer` is the label an authenticator app shows, and is ignored when [`TwoFactorPolicy.Issuer`](../configuration) is set.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.Auth.Credentials;

[ApiController]
[Authorize]
[Route("auth/two-factor")]
public sealed class TwoFactorController(
    IAuthTwoFactorService<AppUser> twoFactor
) : ControllerBase
{
    [HttpPost("begin")]
    public async Task<ActionResult<AuthTwoFactorResult>> Begin(
        CancellationToken cancellationToken
    ) => Ok(await twoFactor.BeginEnrolmentAsync(
            User.GetCurrentUserId(),
            "Example",
            cancellationToken
         ));
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

Verifies a code against the pending secret, promotes it to the secret in force, turns two-factor on, and returns freshly generated recovery codes. Promotion, code replacement, and enabling happen in one transaction, so a half-applied enrolment cannot leave an account with no working factor.

Only hashes of the recovery codes are kept, so these are shown to the user once and cannot be produced again. Throws [`InvalidTwoFactorCodeException`](../exceptions) when the code is wrong, when there is no pending enrolment to complete, and when the pending secret has expired. How many codes are issued is set by [`TwoFactorPolicy.RecoveryCodeCount`](../configuration).

```csharp
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

IReadOnlyList<string> recoveryCodes = await twoFactor
    .CompleteEnrolmentAsync(User.GetCurrentUserId(), code);
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

Checks a submitted value as a TOTP code first and as a recovery code second, spending the recovery code when it matches. Both are claimed with a guarded update rather than read and then written, so two requests presenting the same code at once cannot both be accepted.

Returns `false` for a wrong code, an unreadable secret, a recovery code that was already spent, and an enrolment that was begun but never confirmed, rather than throwing: at sign-in a wrong code is an ordinary outcome the caller decides how to report. A user with no enrolment row at all still throws [`InvalidTwoFactorCodeException`](../exceptions), so call this only for a user you have already established is enrolled.

```csharp
using AlmightyShogun.AspNet.Auth.Credentials;

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
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

await twoFactor.DisableAsync(User.GetCurrentUserId());
```

### Type signature

```csharp
public Task DisableAsync(
    Guid identifier,
    CancellationToken cancellationToken = default
);
```
