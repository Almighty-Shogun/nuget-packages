# AuthUserService

Signs users in and creates new accounts. Application code depends on `IAuthUserService<TUser>`, whose session-creating paths return an [`AuthSessionResult<TUser>`](../results/auth-session-result) carrying the access token, the refresh token, and the user.

A failed login costs the same time as a successful one: when no user matches the identifier, the password is still verified against a throwaway hash before the failure is thrown, so response timing does not reveal which identifiers exist.

::: danger
`result.User` is the database entity. Returned as written here, it serializes with the password hash, the surrogate key, and any loaded sessions, so map it to a DTO that exposes only the fields the client needs before returning it.
:::

## LoginAsync

Matches `Identifier` against both username and email and verifies the password, returning an [`AuthLoginResult<TUser>`](../results/auth-login-result). A user with no enabled two-factor enrolment gets a refresh-token session for the resolved application; a user with one gets a challenge instead, and no session is opened and no token minted. The stored hash is upgraded in place when ASP.NET Core's password hasher reports an outdated format, so raising the work factor takes effect as users sign in.

Throws [`InvalidCredentialsException`](../exceptions) when the identifier matches nothing or the password is wrong, [`AccountLockedException`](../exceptions) while a lockout is in force, [`UnknownAppException`](/asp-net-auth/exceptions) when app scoping is on and this request's host maps to no configured application, and [`AccountDisabledException`](../exceptions) for a deactivated account. The disabled check runs after the password check, so it cannot be used to probe for accounts. An attempt is counted against the lockout before the password is verified rather than after, so the limit bounds attempts made at once as well as attempts made one after another; the run is cleared only once the sign-in finishes, so a user who owes a second factor keeps their count until [`CompleteTwoFactorLoginAsync`](#completetwofactorloginasync) succeeds. Issuing a challenge gives that one attempt back, so an abandoned or expired code prompt costs nothing and correct passwords never lock the account on their own.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class LoginController(
    IAuthUserService<AppUser> authUsers
) : ControllerBase
{
    public async Task<ActionResult<AppUser>> Login(LoginRequest request)
    {
        AuthLoginResult<AppUser> result = await authUsers
            .LoginAsync(request, HttpContext);

        if (result.RequiresTwoFactor)
            return Accepted(new { challenge = result.Challenge });

        Response.SetRefreshTokenCookie(result.Session.RefreshToken, 30);

        return Ok(result.User);
    }
}
```

### Type signature

```csharp
public Task<AuthLoginResult<TUser>> LoginAsync(
    LoginRequest request,
    HttpContext context,
    CancellationToken cancellationToken = default
);
```

## CompleteTwoFactorLoginAsync

Finishes a sign-in that [`LoginAsync`](#loginasync) stopped at the second factor, opening the session only once the code verifies. It takes a [`CompleteTwoFactorLoginRequest`](../requests/complete-two-factor-login-request), whose `Code` is tried as a TOTP code first and as a recovery code second, so the caller does not have to say which it is. The application is resolved from this request's host and has to match the one recorded on the challenge, so a challenge bought through one application cannot be completed through another, and the session ends up scoped to the host the password was sent to. The challenge is spent on success, so it cannot be presented twice.

Throws [`InvalidTwoFactorChallengeException`](../exceptions) when the challenge is unknown, already spent, past its expiry, or was issued through a different application, none of which are distinguished, [`UnknownAppException`](/asp-net-auth/exceptions) when app scoping is on and this request's host maps to no configured application, and [`InvalidTwoFactorCodeException`](../exceptions) when the code is refused or the enrolment was disabled after the challenge was issued. A refused code leaves the challenge unspent, so a mistyped one can be corrected without sending the password again. An account deactivated since the password verified is refused with [`AccountDisabledException`](../exceptions). Nothing here is metered: a wrong code costs no lockout attempt, so an application that wants the code prompt bounded has to bound it itself.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class TwoFactorLoginController(
    IAuthUserService<AppUser> authUsers
) : ControllerBase
{
    public async Task<ActionResult<AppUser>> CompleteLogin(
        CompleteTwoFactorLoginRequest request
    )
    {
        AuthSessionResult<AppUser> result = await authUsers
            .CompleteTwoFactorLoginAsync(request, HttpContext);

        Response.SetRefreshTokenCookie(result.RefreshToken, 30);

        return Ok(result.User);
    }
}
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> CompleteTwoFactorLoginAsync(
    CompleteTwoFactorLoginRequest request,
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

Throws [`UnknownAppException`](/asp-net-auth/exceptions) when app scoping is on and this request's host maps to no configured application, resolved before the user is inserted, so an unmapped host creates no account at all.

Build the entity from a [`RegisterRequest`](../requests/register-request) rather than binding a client payload onto it directly. `Role` and `Permissions` are ordinary properties on the entity, so anything a client can set there becomes claims in its own access token.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class RegisterController(
    IAuthUserService<AppUser> authUsers
) : ControllerBase
{
    public async Task<ActionResult<AppUser>> Register(
        RegisterRequest request
    )
    {
        AppUser user = new()
        {
            Email = request.Email,
            Username = request.Username
        };

        AuthSessionResult<AppUser> result = await authUsers
            .RegisterAsync(user, request.Password, HttpContext);

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
