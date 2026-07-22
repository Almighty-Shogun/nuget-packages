# AuthTokenService

Generates JWT access tokens for credential users. Application code should depend on `IAuthTokenService<TUser>` when it needs to create a token from a custom flow that does not use [`IAuthUserService<TUser>`](./auth-user-service).

The service uses [`AuthSettings.Issuer`](/asp-net-jwt-auth/configuration/auth-settings), [`AuthSettings.Secret`](/asp-net-jwt-auth/configuration/auth-settings), and [`AuthSettings.Hours`](/asp-net-jwt-auth/configuration/auth-settings) from [ASP.NET JWT Auth](/asp-net-jwt-auth/). It writes `userId`, `ClaimTypes.NameIdentifier`, `username`, role, and permission claims into the token. When no app value is provided, permission claims can be plain values such as `users.read`. When an app value is provided, permission claims are limited to permissions prefixed with that app name, such as `api:users.read`.

## Usage

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ImpersonationService(
    IAppHostResolver appHostResolver,
    IAuthTokenService<AppUser> tokens)
{
    public string CreateToken(AppUser user)
    {
        string? app = appHostResolver.Resolve();

        return tokens.GenerateToken(user, app);
    }
}
```

## GenerateToken

Creates a signed JWT access token for the supplied user. Pass an app value when the token should be scoped to a specific audience. Pass `null` only when app scoping is disabled or when the calling flow intentionally wants all user permissions included.

The method does not create or update a refresh-token session. Use [`LoginAsync`](./auth-user-service#loginasync), [`RegisterAsync`](./auth-user-service#registerasync), or [`RefreshSessionAsync`](./auth-session-service#refreshsessionasync) when the flow should also manage refresh sessions.

Permission claims keep their stored value. Protect routes with the same value you store on the user: `[AuthPermission("users.read")]` for single-host APIs, or `[AuthPermission("api:users.read")]` when host/app scoping uses app-prefixed permissions.

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

string accessToken = tokens.GenerateToken(user, "api");
```

### Type signature

```csharp
public string GenerateToken(
    TUser user,
    string? app = null
);
```
