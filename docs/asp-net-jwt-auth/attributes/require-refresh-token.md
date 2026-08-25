# RequireRefreshToken

Rejects a request that does not carry the refresh token cookie, returning `401` before the action runs.

It is an MVC authorization filter, so minimal API endpoints do not run it; check the cookie explicitly there with [`GetRefreshTokenCookie`](../extensions/get-refresh-token-cookie).

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;

[ApiController]
[Route("sessions")]
public sealed class SessionsController(
    ISessionService sessionService
) : ControllerBase
{
    [HttpPost("refresh")]
    [RequireRefreshToken]
    public async Task<IActionResult> Refresh()
    {
        string refreshToken = Request.GetRefreshTokenCookie();

        return Ok(await sessionService.RefreshAsync(refreshToken));
    }
}
```

::: warning
This checks **presence only**. The cookie is not decoded, looked up, or verified against anything, so an endpoint behind this attribute must still validate the token itself before trusting it.

Its purpose is to fail obviously-malformed requests early, not to authenticate them.
:::

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireRefreshTokenAttribute
    : Attribute, IAuthorizationFilter;
```
