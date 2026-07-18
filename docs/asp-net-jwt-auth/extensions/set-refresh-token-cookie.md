---
params:
    - name: token
      description: Refresh token value to store in the cookie.
      type: string

    - name: days
      description: Number of days before the refresh-token cookie expires.
      type: int
---

# SetRefreshTokenCookie

Writes the package refresh-token cookie to an `HttpResponse`. The cookie is written as `HttpOnly`, uses `SameSite.Strict`, is scoped to `/`, and uses the current request scheme to decide whether the `Secure` flag should be enabled.

Use this helper after issuing or rotating a refresh token. The `days` argument should usually come from `AuthSettings.RefreshTokenDays` so cookie lifetime stays aligned with the configured refresh-token policy.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

var httpContext = new DefaultHttpContext();

httpContext.Response.SetRefreshTokenCookie("refresh-token-value", days: 30);
```

<FrontmatterDocs/>

## Type signature

```csharp
public void SetRefreshTokenCookie(string token, int days);
```
