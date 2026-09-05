# CookieNames

The cookie names this package reads and writes. Use these rather than string literals, so application code and the package helpers can never disagree about which cookie carries what.

## RefreshToken

The cookie carrying the refresh token, written by [`SetRefreshTokenCookie`](../extensions/set-refresh-token-cookie) and read by [`GetRefreshTokenCookie`](../extensions/get-refresh-token-cookie). Written `HttpOnly`, so script on the page cannot read it even though the browser sends it with every request to the origin.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;

[ApiController]
[Route("sessions")]
public sealed class SessionsController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
        => Ok(Request.Cookies.ContainsKey(CookieNames.RefreshToken));
}
```

### Type signature

```csharp
public const string RefreshToken = "refreshToken";
```
