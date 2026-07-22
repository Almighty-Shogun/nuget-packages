# AuthUserService

Provides user-facing credential operations for login, user creation, and registration. Application code should depend on `IAuthUserService<TUser>`; [`AddCredentialAuth`](../extensions/add-credential-auth) registers the package implementation for the configured [`AuthDbContext<TUser>`](../types/auth-db-context).

The service hashes passwords before storing users, creates refresh-token sessions for login and registration, and generates JWT access tokens using [ASP.NET JWT Auth](/asp-net-jwt-auth/) settings. Login requests should be validated with [`LoginRequest`](../requests/login-request) before the service runs; the service then looks up the user again before creating the session.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

[ApiController]
[Route("auth")]
public sealed class AuthController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthSessionResult<AppUser>>> Login(LoginRequest request)
    {
        AuthSessionResult<AppUser> result = await authUsers.LoginAsync(request, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result);
    }
}
```

## LoginAsync

Authenticates a validated username/email login request, creates a refresh-token session, and returns a signed JWT access token. When [app scoping](/asp-net-jwt-auth/configuration/auth-settings) is enabled, the current request host determines the access-token audience and the session app value.

The method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized` and message key `auth.failed` when the identifier no longer resolves to a user by the time the session is created. The submitted password is checked by [`CurrentPassword`](../attributes/current-password-attribute) during request validation.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class LoginController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public async Task<ActionResult<AuthSessionResult<AppUser>>> Login(LoginRequest request)
    {
        AuthSessionResult<AppUser> result = await authUsers.LoginAsync(request, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result);
    }
}
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> LoginAsync(
    LoginRequest request,
    HttpContext context
);
```

## CreateUserAsync

Creates a user without creating a session. Use this method for admin-created accounts, imports, seed data, or flows where the account should exist before the user signs in.

The method hashes the supplied plain-text password with ASP.NET Core's password hasher before saving the user. It does not run request validation by itself, so validate user input before mapping it into the `TUser` entity.

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AdminUserService(IAuthUserService<AppUser> authUsers)
{
    public Task<AppUser> CreateAsync(string username, string email, string password)
        => authUsers.CreateUserAsync(new AppUser
        {
            Email = email,
            Username = username
        }, password);
}
```

### Type signature

```csharp
public Task<TUser> CreateUserAsync(
    TUser user,
    string password
);
```

## RegisterAsync

Creates a user and immediately creates an authenticated session for that user. Use this method for public registration endpoints when the user should be signed in after successful registration.

The method resolves the same app scope as [`LoginAsync`](#loginasync), stores a refresh-token session, and returns the created user with the generated access and refresh tokens.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class RegisterController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public async Task<ActionResult<AuthSessionResult<AppUser>>> Register(RegisterRequest request)
    {
        AppUser user = new()
        {
            Email = request.Email,
            Username = request.Username
        };

        AuthSessionResult<AppUser> result = await authUsers.RegisterAsync(user, request.Password, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result);
    }
}
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> RegisterAsync(
    TUser user,
    string password,
    HttpContext context
);
```
