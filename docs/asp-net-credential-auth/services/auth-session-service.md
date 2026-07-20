# AuthSessionService

Refreshes credential-auth sessions by rotating stored refresh tokens and returning a fresh JWT access token. Application code should depend on `IAuthSessionService<TUser>`; [`AddCredentialAuth`](../extensions/add-credential-auth) registers the package implementation.

The service reads request metadata through [ASP.NET Utils](/asp-net-utils/records/session-context), updates browser, device, IP address, User-Agent, and last-active values on the stored [`UserSession`](../types/user-session), and rejects revoked, expired, unknown, or wrong-app refresh tokens.

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
