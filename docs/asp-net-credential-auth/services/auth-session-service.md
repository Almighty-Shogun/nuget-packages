# AuthSessionService

Refreshes and revokes credential-auth sessions. Application code should depend on `IAuthSessionService<TUser>`; [`AddCredentialAuth`](../extensions/add-credential-auth) registers the package implementation.

The service reads request metadata through [ASP.NET Utils](/asp-net-utils/records/session-context), updates browser, device, IP address, User-Agent, and last-active values on the stored [`UserSession`](../types/user-session), and rejects revoked, expired, unknown, or wrong-app refresh tokens. Refresh tokens are hashed before storage; only the generated token returned to the caller is plain text.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth/session")]
public sealed class AuthSessionController(IAuthSessionService<AppUser> sessions) : ControllerBase
{
    [HttpPost("refresh")]
    [RequireRefreshToken]
    public async Task<ActionResult<AuthSessionResult<AppUser>>> Refresh()
    {
        string refreshToken = Request.GetRefreshTokenCookie();
        AuthSessionResult<AppUser> result = await sessions.RefreshSessionAsync(refreshToken, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result);
    }

    [HttpPost("logout")]
    [RequireRefreshToken]
    public async Task<IActionResult> Logout()
    {
        string refreshToken = Request.GetRefreshTokenCookie();

        await sessions.RevokeSessionAsync(refreshToken);
        Response.DeleteAuthCookies();

        return NoContent();
    }
}
```

## RefreshSessionAsync

Finds the active session for the submitted refresh token, verifies it belongs to the current app scope when app scoping is enabled, rotates the refresh token, extends the session lifetime using [`AuthSettings.RefreshTokenDays`](/asp-net-jwt-auth/configuration/auth-settings), and returns a new [`AuthSessionResult<TUser>`](../results/auth-session-result).

The method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized` when the refresh token is unknown, expired, revoked, or scoped to a different app.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();
AuthSessionResult<AppUser> result = await sessions.RefreshSessionAsync(refreshToken, httpContext);
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> RefreshSessionAsync(
    string refreshToken,
    HttpContext httpContext
);
```

## RevokeSessionAsync

Revokes the active session that matches the submitted refresh token. Use this method for logout endpoints so the application does not compare the browser cookie against the hashed refresh token stored in [`UserSession`](../types/user-session).

The method does nothing when the token is unknown, expired, or already revoked. That keeps logout idempotent: deleting the browser cookie can still succeed even when the stored session is already gone.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();

await sessions.RevokeSessionAsync(refreshToken);
```

### Type signature

```csharp
public Task RevokeSessionAsync(string refreshToken);
```
