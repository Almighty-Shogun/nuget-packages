# CookieNames

The cookie names this package reads and writes. Use these rather than string literals, so application code and the package helpers can never disagree about which cookie carries what.

## Usage

```csharp
using AlmightyShogun.AspNet.Auth;

bool hasRefreshToken = httpContext.Request.Cookies
    .ContainsKey(CookieNames.RefreshToken);
```

## RefreshToken

The cookie carrying the refresh token, written by [`SetRefreshTokenCookie`](../extensions/set-refresh-token-cookie) and read by [`GetRefreshTokenCookie`](../extensions/get-refresh-token-cookie). Written `HttpOnly`, so script on the page cannot read it even though the browser sends it with every request to the origin.

### Type signature

```csharp
public const string RefreshToken = "refreshToken";
```
