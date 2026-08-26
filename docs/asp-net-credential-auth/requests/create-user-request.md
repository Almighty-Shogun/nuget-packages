---
fields:
    - name: Username
      description: The account name to claim. Refused with `UsernameTakenException` when another account already holds it.
      type: string

    - name: Password
      description: The initial password, at least 8 characters and subject to the `[PasswordSecure]` rule. Hashed before the row is written and never stored as given.
      type: string

    - name: Email
      description: The address to claim, checked for a valid shape by `[Email]`. Refused with `EmailTakenException` when another account already holds it.
      type: string

    - name: Role
      description: The role the new account gets, written into its access token as a role claim.
      type: string
      default: User

    - name: Permissions
      description: The permissions the new account gets, one token claim each. Prefix them per application, as in `api:users.read`, only when routes are scoped that way.
      type: 'string[]'
      default: '[]'
---

# CreateUserRequest

Everything an administrator supplies to create an account, including the role and permissions a user must never set for themselves. Public sign-up uses [`RegisterRequest`](./register-request) instead.

::: warning
Never bind this model on a route a normal user can reach. `Role` and `Permissions` become claims in the created account's own token, so exposing it publicly lets a caller grant themselves anything.
:::

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.CredentialAuth;

[Authorize(Roles = "Admin")]
public sealed class AdminUsersController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public Task<AppUser> Create(CreateUserRequest request)
        => authUsers.CreateUserAsync(new AppUser
        {
            Role = request.Role,
            Email = request.Email,
            Username = request.Username,
            Permissions = request.Permissions
        }, request.Password);
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public class CreateUserRequest;
```
