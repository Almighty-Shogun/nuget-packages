# AuthSessionResult

Represents the result returned by credential flows that create or refresh an authenticated session. [`LoginAsync`](../services/auth-user-service#loginasync), [`RegisterAsync`](../services/auth-user-service#registerasync), and [`RefreshSessionAsync`](../services/auth-session-service#refreshsessionasync) all return this model.

The result contains the authenticated user, a JWT access token, and a refresh token. API endpoints usually return the user and access token in the response body and store the refresh token through [`SetRefreshTokenCookie`](/asp-net-jwt-auth/extensions/set-refresh-token-cookie).

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class SessionResultController : ControllerBase
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

## Type signature

```csharp
public sealed class AuthSessionResult<TUser> where TUser : AuthUser
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public TUser User { get; init; }
}
```
