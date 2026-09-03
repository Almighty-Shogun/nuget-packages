---
fields:
    - name: Identifier
      description: A username or an email address. Both are matched, so one login form serves users who remember either.
      type: string

    - name: Password
      description: The password as typed. It is verified against the stored hash by the service, not during request validation.
      type: string
---

# LoginRequest

The credentials [`LoginAsync`](../services/auth-user-service#loginasync) takes. Validation only checks that both values are present; whether they are correct is decided by the service, so a wrong password and an unknown user come back identically.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth")]
public sealed class LoginController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginRequest request)
    {
        AuthSessionResult<AppUser> result = await authUsers.LoginAsync(request, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result.User);
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public record LoginRequest;
```
