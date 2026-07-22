# RegisterRequest

Represents a public registration request for a credential user. The request accepts only the values a new user should be allowed to provide themselves: username, email address, and plain-text password.

Use this DTO with [`IAuthUserService<TUser>.RegisterAsync`](../services/auth-user-service#registerasync). The username and email fields include uniqueness validation, and the password field uses the secure password rule from [ASP.NET Validation](/asp-net-validation/validation-rules/passwords). Role and permission assignment should happen in application code or through an admin-only flow that uses [`CreateUserRequest`](./create-user-request).

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class RegisterController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public Task<AuthSessionResult<AppUser>> Register(RegisterRequest request)
    {
        AppUser user = new()
        {
            Email = request.Email,
            Username = request.Username
        };

        return authUsers.RegisterAsync(user, request.Password, HttpContext);
    }
}
```

## Type signature

```csharp
public class RegisterRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
```
