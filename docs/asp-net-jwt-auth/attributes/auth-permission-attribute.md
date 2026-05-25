---
outline: deep

params:
    - name: permission
      description: Permission claim value required to access the controller or action.
      type: string
---

# AuthPermissionAttribute

Applies a package permission policy to an ASP.NET Core controller or action. The constructor accepts the permission value that must be present in the authenticated user's `permission` claims.

The attribute stores the policy name as `permission:{permission}`. At runtime, the package's policy provider recognizes that prefix and creates an authorization policy that requires an authenticated user and a matching `permission` claim.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;

[ApiController]
[Route("admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    [HttpGet]
    [AuthPermission("users.read")]
    public IActionResult ListUsers() => Ok();
}
```

<FrontmatterDocs/>
