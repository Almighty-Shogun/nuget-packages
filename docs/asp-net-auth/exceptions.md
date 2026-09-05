# Exceptions

The package throws three plain exceptions. [`AddAuth`](./extensions/add-auth) registers a mapper and a handler covering all three, built on [`IExceptionMapper`](/asp-net-core/exceptions), so each becomes a standardized error response worded through [Localization](./localization). Pass `registerExceptionHandler: false` and register your own handler to answer them differently.

| Exception                      | Status | error code              |
|--------------------------------|--------|-------------------------|
| `MissingUserIdClaimException`  | `401`  | `missing_user_id_claim` |
| `MissingRefreshTokenException` | `401`  | `missing_refresh_token` |
| `UnknownAppException`          | `403`  | `unknown_app`           |

## MissingUserIdClaimException

Thrown by [`GetCurrentUserId`](./extensions/get-current-user-id) when the principal has no usable user identifier claim, or the claim value is not a well-formed `Guid`.

In practice this means the endpoint was reached without authentication, or a token was minted with neither a [`UserId`](./constants/auth-claim-types) claim nor a `ClaimTypes.NameIdentifier` one.

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

Thrown by [`IAppHostResolver`](./services/app-host-resolver) when host scoping is active and the host maps to no configured application, or when there is no request to read a host from at all, as when a background job mints a token. Carries the offending `Host`, which is what you want in the log line.

A `403` from this usually means a new domain was added at the proxy but not to `Auth:Hosts`.

### Type signature

```csharp
public sealed class UnknownAppException(string? host) : Exception
{
    public string? Host { get; } = host;
}
```
