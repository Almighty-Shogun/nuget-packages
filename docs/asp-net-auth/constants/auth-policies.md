# AuthPolicies

The authorization policy naming used by the package.

## Usage

```csharp
using AlmightyShogun.AspNet.Auth;
using Microsoft.AspNetCore.Authorization;

AuthorizationResult result = await authorizationService.AuthorizeAsync(
    User,
    resource: null,
    policyName: $"{AuthPolicies.PermissionPrefix}users.read"
);
```

## PermissionPrefix

The prefix identifying a permission-backed policy. [`AuthPermission`](../attributes/auth-permission) builds its policy name by prepending this to the permission, so `[AuthPermission("users.read")]` produces the policy `permission:users.read`. Use it when constructing a policy name by hand, for example in an imperative authorization check.

### Type signature

```csharp
public const string PermissionPrefix = "permission:";
```
