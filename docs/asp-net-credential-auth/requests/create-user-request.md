# CreateUserRequest

Represents the reusable credential fields needed to create a user from an administrative or trusted flow: username, email, plain-text password, role, and permission names. The package does not include profile-specific fields such as display name; add those in an application request and map them onto your own [`AuthUser`](../types/auth-user) subclass.

Use this DTO for admin user creation forms, imports, or other flows where the caller is allowed to assign roles and permissions. Public registration endpoints should use [`RegisterRequest`](./register-request), because clients should not submit their own authorization values. The username and email fields include uniqueness validation, and the password field uses the secure password rule from [ASP.NET Validation](/asp-net-validation/validation-rules/passwords).

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AdminUsersController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public Task<AppUser> Create(CreateUserRequest request)
    {
        AppUser user = new()
        {
            Role = request.Role,
            Email = request.Email,
            Username = request.Username,
            Permissions = request.Permissions
        };

        return authUsers.CreateUserAsync(user, request.Password);
    }
}
```

## Type signature

```csharp
public class CreateUserRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public string Role { get; set; } = "User";
    public string[] Permissions { get; set; } = [];
}
```
