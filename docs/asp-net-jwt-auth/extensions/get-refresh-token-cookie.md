---
returns: The refresh-token cookie value.
---

# GetRefreshTokenCookie

Reads the package refresh-token cookie from an `HttpRequest` and returns it as a non-nullable `string`. The helper looks up the cookie name from [`CookieNames.RefreshToken`](../constants/cookie-names), so application code can read the same cookie that [`SetRefreshTokenCookie`](./set-refresh-token-cookie) writes.

Use this in refresh-token, logout, or token-revocation code after the endpoint has already established that a refresh token must be present. If the cookie is missing or empty, the method throws [`HttpErrorException`](/asp-net-utils/types/http-error-exception) with status code `401 Unauthorized` instead of returning `null`, which keeps required refresh-token flows explicit.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

var httpContext = new DefaultHttpContext();
httpContext.Request.Headers.Cookie = "refreshToken=abc123";

string refreshToken = httpContext.Request.GetRefreshTokenCookie();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string GetRefreshTokenCookie();
```
