---
fields:
    - name: AccessToken
      description: The signed JWT to return to the client. Its lifetime comes from `AccessTokenMinutes` in the JWT package's configuration.
      type: string

    - name: RefreshToken
      description: The refresh token in plain text, the only copy that will ever exist; only its hash is stored. Put it in the refresh-token cookie rather than the response body.
      type: string

    - name: User
      description: The authenticated user, tracked by the context. `Password` and `Sessions` are ignored during JSON serialization, so returning it directly leaks neither.
      type: TUser
---

# AuthSessionResult

What every flow that establishes a session returns: [`LoginAsync`](../services/auth-user-service#loginasync), [`RegisterAsync`](../services/auth-user-service#registerasync), and [`RefreshSessionAsync`](../services/auth-session-service#refreshsessionasync).

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class SessionResponseController : ControllerBase
{
    public ActionResult<object> CreateResponse(AuthSessionResult<AppUser> result)
    {
        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(new
        {
            result.User,
            result.AccessToken
        });
    }
}
```

::: danger
Do not return this type straight from a controller. `User` is the live database entity, so every column a consumer adds to their own type derived from [`AuthUser`](../types/auth-user) is serialized with it, including ones that were never meant to leave the server. Project the fields the client needs onto a response type of your own instead.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public sealed class AuthSessionResult<TUser> where TUser : AuthUser;
```
