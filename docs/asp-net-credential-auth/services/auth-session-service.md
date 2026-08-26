# AuthSessionService

Creates, refreshes, and revokes the refresh-token sessions behind a signed-in user. Application code depends on `IAuthSessionService<TUser>`; only the hash of a refresh token is ever stored, so the value returned to the caller is the only copy.

Refreshing rotates the token and records the one it replaced. Presenting a token that was already rotated away, outside a 30-second grace for a retried request, is treated as theft: every session belonging to that user is revoked.

## CreateSessionAsync

Issues a refresh token for a user and stores the session it belongs to, recording the IP address, User-Agent, and the browser, OS, and device parsed from it. Sessions already past their expiry for that user are deleted in the same call, so the table does not accumulate dead rows.

[`LoginAsync`](./auth-user-service#loginasync) and [`RegisterAsync`](./auth-user-service#registerasync) already call this. Call it directly only for a sign-in path this package does not own, such as an SSO callback that has established the user some other way.

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class SsoSignInService(
    IAppHostResolver appHostResolver,
    IAuthSessionService<AppUser> sessions)
{
    public Task<string> SignInAsync(AppUser user, HttpContext httpContext)
        => sessions.CreateSessionAsync(
                user,
                appHostResolver.Resolve(),
                httpContext.GetSessionContext()
            );
}
```

### Type signature

```csharp
public Task<string> CreateSessionAsync(
    TUser user,
    string? app,
    SessionContext context
);
```

## RefreshSessionAsync

Matches the submitted token against a live session, rotates it, refreshes the recorded request metadata, and returns a new access token. The new expiry is capped by [`AbsoluteSessionLifetimeDays`](../configuration), so refreshing extends a session but cannot keep it alive forever.

Throws [`InvalidSessionException`](../exceptions) when the token matches no usable session, whether unknown, expired, revoked, or scoped to a different application. A disabled or locked-out account is refused with [`AccountDisabledException`](../exceptions) or [`AccountLockedException`](../exceptions), so deactivating a user takes effect on their next refresh rather than at the end of their access token.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();
AuthSessionResult<AppUser> result = await sessions
    .RefreshSessionAsync(refreshToken, httpContext);
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> RefreshSessionAsync(
    string refreshToken,
    HttpContext httpContext
);
```

## RevokeSessionAsync

Revokes the one session the token belongs to, leaving the user's other sessions alone. This is what a logout endpoint calls before deleting the cookie.

An unknown, expired, or already revoked token is not an error and nothing is written, which keeps logout idempotent: clearing the browser cookie still succeeds when the stored session is already gone.

```csharp
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();

await sessions.RevokeSessionAsync(refreshToken);
```

### Type signature

```csharp
public Task RevokeSessionAsync(string refreshToken);
```
