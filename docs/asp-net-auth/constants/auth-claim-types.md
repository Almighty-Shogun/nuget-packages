# AuthClaimTypes

The claim names this package reads and writes. Use these constants rather than string literals, so a token minted by one package is understood by another.

## UserId

Carries the authenticated user's public identifier as a `Guid`. Read by [`GetCurrentUserId`](../extensions/get-current-user-id), which also falls back to the standard `ClaimTypes.NameIdentifier`.

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

public sealed class SessionTokenFactory(IAuthTokenGenerator tokenGenerator)
{
    public AuthToken CreateFor(Guid identifier) => tokenGenerator.Generate([
        new Claim(AuthClaimTypes.UserId, identifier.ToString())
    ]);
}
```

### Type signature

```csharp
public const string UserId = "userId";
```

## Permission

Carries a single granted permission. A principal may hold many, and [`AuthPermission`](../attributes/auth-permission) matches against all of them.

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

AuthToken token = tokenGenerator.Generate([
    new Claim(AuthClaimTypes.Permission, "users.read"),
    new Claim(AuthClaimTypes.Permission, "orders.*")
]);
```

### Type signature

```csharp
public const string Permission = "permission";
```
