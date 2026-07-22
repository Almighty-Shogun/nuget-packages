# AuthPasswordService

Handles password changes and forgot-password flows for credential users. Application code should depend on `IAuthPasswordService`; [`AddCredentialAuth`](../extensions/add-credential-auth) registers the package implementation.

The service always hashes stored passwords with ASP.NET Core's password hasher. Password-sensitive operations revoke existing sessions and invalidate active password reset tokens so old credentials and reset links cannot continue being used after the password changes.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth/password")]
public sealed class PasswordController(IAuthPasswordService passwords) : ControllerBase
{
    [HttpPost("change")]
    [RequireRefreshToken]
    public async Task<IActionResult> Change(ChangePasswordRequest request)
    {
        int userId = User.GetCurrentUserId();
        string currentRefreshToken = Request.GetRefreshTokenCookie();

        await passwords.ChangePasswordAsync(userId, request, currentRefreshToken);

        return NoContent();
    }
}
```

## ChangePasswordAsync

Changes the current user's password after the request has passed [`ChangePasswordRequest`](../requests/change-password-request) validation. The optional `currentRefreshToken` keeps the current browser session active while revoking the user's other active sessions.

The method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized` when the user cannot be found. Current-password and reused-password checks are handled by [`CurrentPassword`](../attributes/current-password-attribute) and [`NotCurrentPassword`](../attributes/not-current-password-attribute) before the service runs.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

int userId = httpContext.User.GetCurrentUserId();
string refreshToken = httpContext.Request.GetRefreshTokenCookie();

await passwords.ChangePasswordAsync(userId, request, refreshToken);
```

### Type signature

```csharp
public Task ChangePasswordAsync(
    int userId,
    ChangePasswordRequest request,
    string? currentRefreshToken = null
);
```

## RequestForgotPasswordAsync

Creates a one-hour password reset token for the user with the supplied email address. The returned token is plain text and should be sent through the application's email system; only a SHA-256 hash is stored in the database.

When the user already has active reset tokens, the service removes them before storing the new token. The optional `requestIpAddress` is stored on the [`PasswordResetToken`](../types/password-reset-token) record for auditing.

The method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized` and message key `auth.failed` when no credential user exists for the submitted email address.

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

string? ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
string resetToken = await passwords.RequestForgotPasswordAsync(request, ipAddress);

await passwordResetMailer.SendAsync(request.Email, resetToken);
```

### Type signature

```csharp
public Task<string> RequestForgotPasswordAsync(
    ForgotPasswordRequest request,
    string? requestIpAddress = null
);
```

## CompleteForgotPasswordAsync

Completes a forgot-password flow by finding an active reset token, marking the token used, updating the user's password, revoking all active sessions, and invalidating any other reset tokens for the same user.

The method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized` when the reset token is unknown, used, expired, or points to a missing user. Reused-password checks are handled by [`NotCurrentPassword`](../attributes/not-current-password-attribute) before the service runs.

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

await passwords.CompleteForgotPasswordAsync(request);
```

### Type signature

```csharp
public Task CompleteForgotPasswordAsync(
    CompleteForgotPasswordRequest request
);
```
