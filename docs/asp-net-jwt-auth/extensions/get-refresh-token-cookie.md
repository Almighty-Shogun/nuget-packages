---
outline: deep

returns: The refresh-token cookie value, or `null` when the cookie is not present.
---

# GetRefreshTokenCookie

Reads the package refresh-token cookie from an `HttpRequest`. The helper looks up the cookie name from `CookieNames.RefreshToken`, so application code can read the same cookie that `SetRefreshTokenCookie` writes.

Use this in refresh-token endpoints when the access token has expired and the API needs to retrieve the refresh token sent by the browser. The method returns `null` when the cookie does not exist, allowing the endpoint to return an unauthorized response or start a new sign-in flow.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

var httpContext = new DefaultHttpContext();
httpContext.Request.Headers.Cookie = "refreshToken=abc123";

string? refreshToken = httpContext.Request.GetRefreshTokenCookie();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string? GetRefreshTokenCookie();
```

## Uses

- [CookieNames](../constants/cookie-names)
