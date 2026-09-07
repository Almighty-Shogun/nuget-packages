# AuthPasswordService

Changes a signed-in user's password and runs the forgot-password flow. Application code depends on `IAuthPasswordService`, which is not generic: it works from the public identifier or the reset token rather than from a user entity.

Both flows revoke the user's other sessions, spend every outstanding reset token, and retire every sign-in still waiting on a second factor, so a password that has just changed cannot leave an old session, an old reset link, or a half-finished sign-in working.

## ChangePasswordAsync

Verifies the current password, then replaces it. Passing the caller's own refresh token as `currentRefreshToken` leaves that one session alive, so changing a password does not sign the user out of the browser they are changing it from; passing nothing ends every session. Any sign-in still waiting on a second factor is retired whichever token is passed, so a challenge bought with the old password stops being redeemable.

Throws [`PasswordMismatchException`](../exceptions) when the confirmation differs from the new password, [`InvalidCredentialsException`](../exceptions) when the current password is wrong, and [`PasswordReusedException`](../exceptions) when the new password verifies against the one already stored.

A refresh on one of the user's other sessions can commit while the change is open, which makes it lose and start over. It is retried a bounded number of times and then throws [`ConcurrentSessionUpdateException`](../exceptions), having written nothing. Another request writing the password itself is recognised the same way, and the attempt that follows verifies against the hash that is then stored, so it ends in [`InvalidCredentialsException`](../exceptions).

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

[ApiController]
[Route("auth/password")]
public sealed class PasswordController(
    IAuthPasswordService passwords
) : ControllerBase
{
    [HttpPost("change")]
    public async Task<IActionResult> Change(ChangePasswordRequest request)
    {
        Guid identifier = User.GetCurrentUserId();
        string currentRefreshToken = Request.GetRefreshTokenCookie();

        await passwords.ChangePasswordAsync(
            identifier,
            request,
            currentRefreshToken
        );

        return NoContent();
    }
}
```

### Type signature

```csharp
public Task ChangePasswordAsync(
    Guid identifier,
    ChangePasswordRequest request,
    string? currentRefreshToken = null,
    CancellationToken cancellationToken = default
);
```

## RequestForgotPasswordAsync

Issues a reset token for the account with that email address and returns it in plain text for the application to email. Only its hash is stored, so this return value is the single opportunity to send it. A user holds one reset token at a time, so issuing a new one overwrites whatever the previous link used and the newest link is always the only working one.

Returns `null` when no account has that address. Both outcomes are held to [`ForgotPasswordMinimumMilliseconds`](../configuration), so the time taken says nothing about whether the address exists. The token expires after [`PasswordResetMinutes`](../configuration).

::: danger
The endpoint must answer identically whether or not the address existed. Returning `NotFound()` for `null` and `Ok()` for a token hands an attacker the answer the timing floor was there to hide, so reply with the same status and body in both cases, such as `Ok()` and a message saying a link has been sent if the address is registered.
:::

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth.Credentials;

string? ipAddress = httpContext.GetIpAddress();
string? resetToken = await passwords.RequestForgotPasswordAsync(
    request,
    ipAddress
);

if (resetToken is not null)
    await passwordResetMailer.SendAsync(request.Email, resetToken);
```

### Type signature

```csharp
public Task<string?> RequestForgotPasswordAsync(
    ForgotPasswordRequest request,
    string? requestIpAddress = null,
    CancellationToken cancellationToken = default
);
```

## CompleteForgotPasswordAsync

Redeems a reset token, sets the new password, and marks the token spent. Every session is revoked and every sign-in still waiting on a second factor retired, so nothing bought with the old password survives the reset. The token identifies the user, so no signed-in caller is needed and nothing about the account has to be supplied alongside it.

Throws [`InvalidPasswordResetTokenException`](../exceptions) when the token is unknown, already spent, or expired, and also when a concurrent request spends it first, since the token is claimed with a guarded update rather than on the strength of the read that found it. [`PasswordMismatchException`](../exceptions) covers a confirmation that differs, and [`PasswordReusedException`](../exceptions) a new password that is the one already stored.

A refresh on any of the user's sessions can commit while the reset is open, which makes it lose and start over. It is retried a bounded number of times and then throws [`ConcurrentSessionUpdateException`](../exceptions), leaving the token unspent and the link still usable.

```csharp
using AlmightyShogun.AspNet.Auth.Credentials;

await passwords.CompleteForgotPasswordAsync(request);
```

### Type signature

```csharp
public Task CompleteForgotPasswordAsync(
    CompleteForgotPasswordRequest request,
    CancellationToken cancellationToken = default
);
```
