---
returns: The refresh-token cookie value, or `null` when the cookie is not present.
---

# TryGetRefreshTokenCookie

Attempts to read the package refresh-token cookie from an `HttpRequest`. The helper uses [`CookieNames.RefreshToken`](../constants/cookie-names), so it checks the same cookie name that [`SetRefreshTokenCookie`](./set-refresh-token-cookie) writes.

Use this when a request may or may not contain a refresh token and the caller needs to decide what should happen next. For flows where the cookie is required, use [`GetRefreshTokenCookie`](./get-refresh-token-cookie) instead so missing cookies fail immediately.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

var httpContext = new DefaultHttpContext();
httpContext.Request.Headers.Cookie = "refreshToken=abc123";

string? refreshToken = httpContext.Request.TryGetRefreshTokenCookie();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string? TryGetRefreshTokenCookie();
```
