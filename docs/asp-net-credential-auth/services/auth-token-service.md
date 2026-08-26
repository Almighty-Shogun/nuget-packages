# AuthTokenService

Builds the claim set for a user and returns a signed access token carrying it. Application code depends on `IAuthTokenService<TUser>` only for a flow this package does not own; [`LoginAsync`](./auth-user-service#loginasync), [`RegisterAsync`](./auth-user-service#registerasync), and [`RefreshSessionAsync`](./auth-session-service#refreshsessionasync) already issue one.

Signing, issuer, audience, and expiry come from [ASP.NET JWT Auth](/asp-net-jwt-auth/configuration). This service decides only what the token says.

## GenerateToken

Writes the user's public identifier as both `userId` and `ClaimTypes.NameIdentifier`, the username, the role, and one permission claim per entry in `Permissions`. It creates no session and no refresh token, so a token minted here cannot be refreshed and simply expires.

`app` decides which permissions travel. With a value, only permissions prefixed `app:` are included and the prefix is stripped, so a user holding `api:users.read` receives `users.read` in a token for `api`. With `null`, every stored permission is included verbatim.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ImpersonationService(
    IAppHostResolver appHostResolver,
    IAuthTokenService<AppUser> tokens
)
{
    public string CreateToken(AppUser user)
        => tokens.GenerateToken(user, appHostResolver.Resolve());
}
```

### Type signature

```csharp
public string GenerateToken(
    TUser user,
    string? app = null
);
```
