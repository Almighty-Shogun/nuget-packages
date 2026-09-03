---
fields:
    - name: Username
      description: The account name to claim. Refused with `UsernameTakenException` when another account already holds it.
      type: string

    - name: Email
      description: The address to claim, checked for a valid shape by `[Email]`. Refused with `EmailTakenException` when another account already holds it.
      type: string

    - name: Password
      description: The initial password, at least 8 characters and subject to the `[PasswordSecure]` rule. Hashed before the row is written and never stored as given.
      type: string
---

# RegisterRequest

The three values a user may supply about themselves when signing up. It carries no role or permission field on purpose, since anything a client can send there ends up as claims in its own access token.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class RegisterController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public async Task<ActionResult<AppUser>> Register(RegisterRequest request)
    {
        AppUser user = new()
        {
            Email = request.Email,
            Username = request.Username
        };

        AuthSessionResult<AppUser> result = await authUsers.RegisterAsync(user, request.Password, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result.User);
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public class RegisterRequest;
```
