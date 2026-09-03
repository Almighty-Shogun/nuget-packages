# AuthUserService

Signs users in and creates new accounts. Application code depends on `IAuthUserService<TUser>`, which returns an [`AuthSessionResult<TUser>`](../results/auth-session-result) carrying the access token, the refresh token, and the user.

A failed login costs the same time as a successful one: when no user matches the identifier, the password is still verified against a throwaway hash before the failure is thrown, so response timing does not reveal which identifiers exist.

## LoginAsync

Matches `Identifier` against both username and email, verifies the password, and creates a refresh-token session for the resolved application. The stored hash is upgraded in place when ASP.NET Core's password hasher reports an outdated format, so raising the work factor takes effect as users sign in.

Throws [`InvalidCredentialsException`](../exceptions) when the identifier matches nothing or the password is wrong, [`AccountLockedException`](../exceptions) while a lockout is in force, and [`AccountDisabledException`](../exceptions) for a deactivated account. The disabled check runs after the password check, so it cannot be used to probe for accounts. An attempt is counted against the lockout before the password is verified rather than after, so the limit bounds attempts made at once as well as attempts made one after another; a successful sign-in then clears the run, including the attempt it counted for itself.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class LoginController(IAuthUserService<AppUser> authUsers) : ControllerBase
{
    public async Task<ActionResult<AppUser>> Login(LoginRequest request)
    {
        AuthSessionResult<AppUser> result = await authUsers.LoginAsync(request, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result.User);
    }
}
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> LoginAsync(
    LoginRequest request,
    HttpContext context,
    CancellationToken cancellationToken = default
);
```

## CreateUserAsync

Creates a user without signing anyone in. Use it for administrator-created accounts, imports, and seed data, where the account should exist before its owner ever sends a request.

The plain-text password is hashed before the row is written and is never stored as given. Throws [`UsernameTakenException`](../exceptions) or [`EmailTakenException`](../exceptions) when another account already holds either value. Both comparisons run under the database's own collation, so give the username and email columns a case-insensitive one unless two accounts differing only in casing are acceptable.

```csharp
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class AdminUserService(IAuthUserService<AppUser> authUsers)
{
    public Task<AppUser> CreateAsync(CreateUserRequest request)
        => authUsers.CreateUserAsync(new AppUser
        {
            Role = request.Role,
            Email = request.Email,
            Username = request.Username,
            Permissions = request.Permissions
        }, request.Password);
}
```

### Type signature

```csharp
public Task<TUser> CreateUserAsync(
    TUser user,
    string password,
    CancellationToken cancellationToken = default
);
```

## RegisterAsync

Creates a user and signs them in immediately, which is what a public sign-up endpoint wants. It performs the same uniqueness checks and the same hashing as [`CreateUserAsync`](#createuserasync), then creates a session scoped to the resolved application.

Build the entity from a [`RegisterRequest`](../requests/register-request) rather than binding a client payload onto it directly. `Role` and `Permissions` are ordinary properties on the entity, so anything a client can set there becomes claims in its own access token.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

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

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> RegisterAsync(
    TUser user,
    string password,
    HttpContext context,
    CancellationToken cancellationToken = default
);
```
