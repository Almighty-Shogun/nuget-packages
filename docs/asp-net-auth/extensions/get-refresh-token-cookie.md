---
returns: The refresh-token cookie value.
---

# GetRefreshTokenCookie

Reads the refresh-token cookie named by [`CookieNames.RefreshToken`](../constants/cookie-names), the same one [`SetRefreshTokenCookie`](./set-refresh-token-cookie) writes, for a path that cannot proceed without it.

A missing or empty cookie throws [`MissingRefreshTokenException`](../exceptions) rather than returning `null`, which reaches the client as `401`. Use [`TryGetRefreshTokenCookie`](./try-get-refresh-token-cookie) where its absence is ordinary.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Auth;

string refreshToken = httpContext.Request.GetRefreshTokenCookie();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string GetRefreshTokenCookie();
```
