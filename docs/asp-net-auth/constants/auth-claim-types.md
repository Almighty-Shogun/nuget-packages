# AuthClaimTypes

The claim names this package reads and writes. Use these constants rather than string literals, so a token minted by one package is understood by another.

## Usage

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

Guid identifier = Guid.Parse("0195f0c8-9a1e-7d3f-8b21-5c9a4e2f7d10");

AuthToken token = tokenGenerator.Generate([
    new Claim(AuthClaimTypes.UserId, identifier.ToString()),
    new Claim(AuthClaimTypes.Permission, "users.read"),
    new Claim(AuthClaimTypes.Permission, "orders.*")
]);
```

## UserId

Carries the authenticated user's public identifier. Read by [`GetCurrentUserId`](../extensions/get-current-user-id), which also falls back to the standard `ClaimTypes.NameIdentifier`.

### Type signature

```csharp
public const string UserId = "userId";
```

## Permission

Carries a single granted permission. A principal may hold many, and [`AuthPermission`](../attributes/auth-permission) matches against all of them.

### Type signature

```csharp
public const string Permission = "permission";
```
