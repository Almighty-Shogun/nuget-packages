---
params:
    - name: permission
      description: The permission the principal must hold, compared case-insensitively against every `permission` claim it carries, so grant several permissions as several claims rather than one comma-separated value. A granted claim ending in `.*` satisfies everything beneath it, so a principal holding `users.*` passes `users.read` and `users.read.all`.
      type: string
---

# AuthPermission

Requires the authenticated principal to hold a permission claim satisfying the named permission. Applies to a controller, an action, or a minimal API endpoint through `RequireAuthorization`.

The attribute builds a policy named `permission:{name}`, resolved and cached by the package's policy provider.

## Usage

::: code-group

```csharp [UsersController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;

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
using AlmightyShogun.AspNet.Auth;

app.MapGet("/users", [AuthPermission("users.read")] () => Results.Ok())
    .RequireAuthorization();
```

```csharp [GrantingClaims.cs]
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;

AuthToken token = tokenGenerator.Generate([
    new Claim(AuthClaimTypes.Permission, "users.*"),
    new Claim(AuthClaimTypes.Permission, "orders.read")
]);
```

:::

::: warning
The wildcard is honored only in the claim, never in the requirement. `[AuthPermission("users.*")]` demands a claim of literally `users.*`, and a principal holding only `users.read` is refused.

| Claim held | Endpoint requires | Result |
|---|---|---|
| `users.read` | `users.read` | allowed |
| `users.*` | `users.read` | allowed |
| `users.*` | `users.read.all` | allowed |
| `users.*` | `orders.read` | denied |
| `users.read` | `users.*` | denied |
:::

<FrontmatterDocs/>

## Type signature

```csharp
public sealed class AuthPermissionAttribute : AuthorizeAttribute
{
    public AuthPermissionAttribute(string permission);
}
```
