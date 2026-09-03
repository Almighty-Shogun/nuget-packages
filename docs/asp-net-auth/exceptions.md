# Exceptions

The package throws three plain exceptions. [`AddAuth`](./extensions/add-auth) registers a mapper and a handler covering all three, built on [`IExceptionMapper`](/asp-net-core/exceptions), so each becomes a standardized error response automatically. Pass `registerExceptionHandler: false` and register your own handler to answer them differently.

Catch them by type when an endpoint needs to react differently; otherwise let them propagate. The message each one produces is resolved through [Localization](./localization).

| Exception                      | Status | error code              |
|--------------------------------|--------|-------------------------|
| `MissingUserIdClaimException`  | `401`  | `missing_user_id_claim` |
| `MissingRefreshTokenException` | `401`  | `missing_refresh_token` |
| `UnknownAppException`          | `403`  | `unknown_app`           |


## Usage

```csharp
using AlmightyShogun.AspNet.Auth;

try
{
    string app = appHostResolver.ResolveAppFromHost(host);
}
catch (UnknownAppException exception)
{
    logger.LogWarning(
        "Rejected request for unmapped host {Host}",
        exception.Host
    );

    throw;
}
```

## MissingUserIdClaimException

Thrown by [`GetCurrentUserId`](./extensions/get-current-user-id) when the principal has no usable user identifier claim, or the claim is not an integer.

In practice this means the endpoint was reached without authentication, or a token was minted without a [`UserId`](./constants/auth-claim-types) claim.

### Type signature

```csharp
public sealed class MissingUserIdClaimException : Exception;
```

## MissingRefreshTokenException

Thrown by [`GetRefreshTokenCookie`](./extensions/get-refresh-token-cookie) when the cookie is absent or blank. Use [`TryGetRefreshTokenCookie`](./extensions/try-get-refresh-token-cookie) when absence is an expected outcome rather than an error.

### Type signature

```csharp
public sealed class MissingRefreshTokenException : Exception;
```

## UnknownAppException

Thrown by [`IAppHostResolver`](./services/app-host-resolver) when host scoping is active and the request host maps to no configured application. Carries the offending `Host`, which is what you want in the log line.

A `403` from this usually means a new domain was added at the proxy but not to `Auth:Hosts`.

### Type signature

```csharp
public sealed class UnknownAppException(string? host) : Exception;
```
