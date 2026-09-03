# AuthTokenGenerator

Mints signed access tokens from the configured issuer, secret, and lifetime, with the caller supplying the claims so no user model is baked into the package.

The generator adds the audience itself, from the request host when host scoping is active and from `DefaultApp` otherwise. An explicit audience overrides both, which a background job minting a token outside a request needs.

## Usage

::: code-group

```csharp [SessionsController.cs]
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;

[ApiController]
[Route("sessions")]
public sealed class SessionsController(
    IAuthTokenGenerator tokenGenerator
) : ControllerBase
{
    [HttpPost]
    public IActionResult Create(Guid identifier)
    {
        AuthToken token = tokenGenerator.Generate([
            new Claim(AuthClaimTypes.UserId, identifier.ToString()),
            new Claim(AuthClaimTypes.Permission, "users.read"),
            new Claim(AuthClaimTypes.Permission, "orders.read")
        ]);

        return Ok(new { token.Token, token.ExpiresAt });
    }
}
```

```csharp [ExplicitAudience.cs]
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

Guid identifier = Guid.Parse("0195f0c8-9a1e-7d3f-8b21-5c9a4e2f7d10");

AuthToken token = tokenGenerator.Generate(
    [new Claim(AuthClaimTypes.UserId, identifier.ToString())],
    audience: "example-admin"
);
```

:::

## Generate

Creates a signed token carrying the supplied claims, plus the resolved audience, issuer, and expiry.

Throws `InvalidOperationException` when no audience can be resolved, which happens only if the request host is unmapped and `DefaultApp` is unset. Startup validation normally prevents that configuration from existing.

Grant multiple permissions by adding multiple `permission` claims, not one comma-separated claim.

### Type signature

```csharp
AuthToken Generate(
    IEnumerable<Claim> claims,
    string? audience = null
);
```
