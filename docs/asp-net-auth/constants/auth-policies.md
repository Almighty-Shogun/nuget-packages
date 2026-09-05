# AuthPolicies

The authorization policy naming used by the package.

## PermissionPrefix

The prefix identifying a permission-backed policy. [`AuthPermission`](../attributes/auth-permission) builds its policy name by prepending
this to the permission, so `[AuthPermission("users.read")]` produces the policy `permission:users.read`. Use it when constructing a policy
name by hand, for example in an imperative authorization check.

```csharp
using System.Security.Claims;
using AlmightyShogun.AspNet.Auth;
using Microsoft.AspNetCore.Authorization;

public sealed class PermissionChecker(
    IAuthorizationService authorizationService
)
{
    public async Task<bool> HasPermissionAsync(
        ClaimsPrincipal principal,
        string permission
    )
    {
        AuthorizationResult result = await authorizationService
            .AuthorizeAsync(
                principal,
                resource: null,
                policyName: $"{AuthPolicies.PermissionPrefix}{permission}"
            );

        return result.Succeeded;
    }
}
```

### Type signature

```csharp
public const string PermissionPrefix = "permission:";
```
