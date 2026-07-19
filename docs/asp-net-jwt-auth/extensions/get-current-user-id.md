---
returns: The authenticated user's numeric user id.
---

# GetCurrentUserId

Reads the current user's id from a `ClaimsPrincipal`. The method checks the package `userId` claim first and then falls back to `ClaimTypes.NameIdentifier`. It is intended for controller, endpoint, and service code that has already received an authenticated principal and needs the application user id as an `int`.

If neither claim exists or the value cannot be parsed as an integer, the method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized`. This makes missing identity data fail immediately instead of silently returning a default value.

## Usage

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.JwtAuth;

var principal = new ClaimsPrincipal(new ClaimsIdentity(
    [new Claim("userId", "42")],
    authenticationType: "Bearer"
));

int userId = principal.GetCurrentUserId();
```

<FrontmatterDocs/>

## Type signature

```csharp
public int GetCurrentUserId();
```
