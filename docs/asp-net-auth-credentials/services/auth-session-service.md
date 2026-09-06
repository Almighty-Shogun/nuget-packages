# AuthSessionService

Creates, refreshes, and revokes the refresh-token sessions behind a signed-in user. Application code depends on `IAuthSessionService<TUser>`; only the hash of a refresh token is ever stored, so the value returned to the caller is the only copy.

## CreateSessionAsync

Issues both of a session's tokens and stores the session they belong to, recording the IP address, User-Agent, and the browser, OS, and device parsed from it. Sessions already past their expiry for that user are deleted in the same call, so the table does not accumulate dead rows. `app` scopes the session and narrows the permissions the access token carries, so passing the wrong one mints a token for the wrong audience.

Nothing is checked here: the account being active, not locked out, and past whatever second factor it owes are all the caller's to establish first. [`RegisterAsync`](./auth-user-service#registerasync), [`LoginAsync`](./auth-user-service#loginasync), and [`CompleteTwoFactorLoginAsync`](./auth-user-service#completetwofactorloginasync) already do that and call this. Call it directly only for a sign-in path this package does not own, such as an SSO callback that has established the user some other way.

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class SsoSignInService(
    IAppHostResolver appHostResolver,
    IAuthSessionService<AppUser> sessions)
{
    public Task<AuthSessionResult<AppUser>> SignInAsync(
        AppUser user,
        HttpContext httpContext
    ) => sessions.CreateSessionAsync(
                user,
                appHostResolver.Resolve(),
                httpContext.GetClientContext()
            );
}
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> CreateSessionAsync(
    TUser user,
    string? app,
    ClientContext context,
    CancellationToken cancellationToken = default
);
```

## RefreshSessionAsync

Matches the submitted token against a live session, rotates it, refreshes the recorded request metadata, and returns a new access token. The new expiry is capped by [`AbsoluteSessionLifetimeDays`](../configuration), so refreshing extends a session but cannot keep it alive forever.

Rotation records the token it replaced. Presenting that one afterwards, outside a 30-second grace for a retried request, is treated as theft: every session belonging to that user is revoked. A session that has since been signed out still counts as a source, so a token stolen shortly before a logout is caught, but the recorded hash is cleared as detection fires, so one retired token revokes once and is refused as unknown from then on. Only the immediately previous token is remembered, so replaying an older one in a chain is refused as unknown and revokes nothing.

Throws [`InvalidSessionException`](../exceptions) when the token matches no usable session, whether unknown, expired, revoked, or scoped to a different application. Two refreshes racing on one session are settled by a concurrency token, so the one that loses is refused the same way. A disabled or locked-out account is refused with [`AccountDisabledException`](../exceptions) or [`AccountLockedException`](../exceptions), so deactivating a user takes effect on their next refresh rather than at the end of their access token.

```csharp
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();
AuthSessionResult<AppUser> result = await sessions
    .RefreshSessionAsync(refreshToken, httpContext);
```

### Type signature

```csharp
public Task<AuthSessionResult<TUser>> RefreshSessionAsync(
    string refreshToken,
    HttpContext httpContext,
    CancellationToken cancellationToken = default
);
```

## RevokeSessionAsync

Revokes the one session the token belongs to, leaving the user's other sessions alone. This is what a logout endpoint calls before deleting the cookie.

An unknown or already revoked token is not an error and nothing is written, which keeps logout idempotent: clearing the browser cookie still succeeds when the stored session is already gone.

```csharp
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();

await sessions.RevokeSessionAsync(refreshToken);
```

### Type signature

```csharp
public Task RevokeSessionAsync(
    string refreshToken,
    CancellationToken cancellationToken = default
);
```
