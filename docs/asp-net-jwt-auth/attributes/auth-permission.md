# AuthPermission

Requires the authenticated principal to hold a permission claim satisfying the named permission. Applies to a controller, an action, or a minimal API endpoint through `RequireAuthorization`.

The attribute builds a policy named `permission:{name}`, resolved and cached by the package's policy provider.


## Usage

::: code-group

```csharp [UsersController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;

[ApiController]
[Route("users")]
[AuthPermission("users.read")]
public sealed class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok();

    [HttpDelete("{id:int}")]
    [AuthPermission("users.delete")]
    public IActionResult Delete(int id) => NoContent();
}
```

```csharp [MinimalApi.cs]
using AlmightyShogun.AspNet.JwtAuth;

app.MapGet("/users", [AuthPermission("users.read")] () => Results.Ok())
    .RequireAuthorization();
```

```csharp [GrantingClaims.cs]
using System.Security.Claims;
using AlmightyShogun.AspNet.JwtAuth;

AuthToken token = tokenGenerator.Generate([
    new Claim(AuthClaimTypes.Permission, "users.*"),
    new Claim(AuthClaimTypes.Permission, "orders.read")
]);
```

:::

## Wildcards

A granted claim ending in `.*` satisfies any permission beneath it, so one claim can stand for a group:

| Claim held | Endpoint requires | Result |
|---|---|---|
| `users.read` | `users.read` | allowed |
| `users.*` | `users.read` | allowed |
| `users.*` | `users.read.all` | allowed |
| `users.*` | `orders.read` | denied |
| `users.read` | `users.*` | **denied** |

::: warning
The wildcard is honored **only in the claim**, never in the requirement. Writing `[AuthPermission("users.*")]` requires a claim of literally `users.*`; it does not accept `users.read`.

That asymmetry is deliberate. If requirements expanded, an endpoint asking for `users.*` would be satisfied by a principal holding only `users.read`, and the endpoint would accept less access than it asked for.
:::

Grant several permissions as several claims, not one comma-separated value.

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class AuthPermissionAttribute : AuthorizeAttribute;
```
