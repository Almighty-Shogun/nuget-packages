# LoginRequest

Represents a username/email and password login request. The `Identifier` field accepts either a username or an email address, and the `Password` field is validated against the user found by that identifier before the login service creates a session.

Use this DTO with [`IAuthUserService<TUser>.LoginAsync`](../services/auth-user-service#loginasync). The request includes `[Required]`, [`LoginIdentifierExists`](../attributes/login-identifier-exists-attribute), and [`CurrentPassword`](../attributes/current-password-attribute), so it works with [ASP.NET Validation](/asp-net-validation/) when the package is registered. The service still looks up the user again before creating a session, but the password check belongs to request validation.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth")]
public sealed class LoginController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    [HttpPost("login")]
    public Task<AuthSessionResult<AppUser>> Login(LoginRequest request)
        => authUsers.LoginAsync(request, HttpContext);
}
```

## Type signature

```csharp
public record LoginRequest
{
    public required string Identifier { get; set; }
    public required string Password { get; set; }
}
```
