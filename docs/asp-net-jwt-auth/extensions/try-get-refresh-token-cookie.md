---
returns: The refresh-token cookie value, or `null` when the cookie is not present.
---

# TryGetRefreshTokenCookie

Reads the refresh-token cookie named by [`CookieNames.RefreshToken`](../constants/cookie-names), returning `null` when the request carries none.

Use it where a signed-out caller is ordinary rather than exceptional. Where the cookie is required, [`GetRefreshTokenCookie`](./get-refresh-token-cookie) fails immediately instead.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

string? refreshToken = httpContext.Request.TryGetRefreshTokenCookie();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string? TryGetRefreshTokenCookie();
```
