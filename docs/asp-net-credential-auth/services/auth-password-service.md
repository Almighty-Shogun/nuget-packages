# AuthPasswordService

Changes a signed-in user's password and runs the forgot-password flow. Application code depends on `IAuthPasswordService`, which is not generic: it works from the public identifier or the reset token rather than from a user entity.

Both flows revoke the user's other sessions and spend every outstanding reset token, so a password that has just changed cannot leave an old session or an old reset link working.

## ChangePasswordAsync

Verifies the current password, then replaces it. Passing the caller's own refresh token as `currentRefreshToken` leaves that one session alive, so changing a password does not sign the user out of the browser they are changing it from; passing nothing ends every session.

Throws [`PasswordMismatchException`](../exceptions) when the confirmation differs from the new password, [`InvalidCredentialsException`](../exceptions) when the current password is wrong, and [`PasswordReusedException`](../exceptions) when the new password verifies against the one already stored.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth/password")]
public sealed class PasswordController(
    IAuthPasswordService passwords
) : ControllerBase
{
    [HttpPost("change")]
    [RequireRefreshToken]
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
    string? currentRefreshToken = null
);
```

## RequestForgotPasswordAsync

Issues a reset token for the account with that email address and returns it in plain text for the application to email. Only its hash is stored, so this return value is the single opportunity to send it. Any unspent token the user already had is deleted first, which means the newest link is always the only working one.

Returns `null` when no account has that address, after a short randomised delay so the response cannot be timed to learn which addresses are registered. Report the same thing to the client either way. The token expires after [`PasswordResetMinutes`](../configuration).

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.CredentialAuth;

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
    string? requestIpAddress = null
);
```

## CompleteForgotPasswordAsync

Redeems a reset token, sets the new password, and marks the token spent. The token identifies the user, so no signed-in caller is needed and nothing about the account has to be supplied alongside it.

Throws [`InvalidPasswordResetTokenException`](../exceptions) when the token is unknown, already spent, or expired, [`PasswordMismatchException`](../exceptions) when the confirmation differs, and [`PasswordReusedException`](../exceptions) when the new password is the one already stored.

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

await passwords.CompleteForgotPasswordAsync(request);
```

### Type signature

```csharp
public Task CompleteForgotPasswordAsync(CompleteForgotPasswordRequest request);
```
