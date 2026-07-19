# RequireRefreshTokenAttribute

Requires the current request to include the package refresh-token cookie before an ASP.NET Core controller or action is allowed to run. Use this attribute on refresh, logout, revocation, or session endpoints that should only execute when the browser has sent the refresh token managed by [`SetRefreshTokenCookie`](../extensions/set-refresh-token-cookie).

When the cookie is missing, the attribute returns an empty `401 Unauthorized` result. It does not depend on the ASP.NET Utils error response package, but an application that also uses [`UseHttpErrorResponses`](/asp-net-utils/extensions/use-http-error-responses) can still convert the empty unauthorized response into the standardized JSON error body.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;

[ApiController]
[Route("auth/session")]
public sealed class AuthSessionController : ControllerBase
{
    [HttpPost("refresh")]
    [RequireRefreshToken]
    public IActionResult Refresh() => Ok();
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireRefreshTokenAttribute : Attribute, IAuthorizationFilter;
```
