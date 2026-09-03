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

Writes the refresh-token cookie after issuing or rotating one. It is `HttpOnly` and scoped to `/`, the `SameSite` mode comes from [`Auth:SameSite`](../configuration), and the `Secure` flag follows the current request scheme.

Pass [`RefreshTokenDays`](../configuration) as the lifetime so the cookie expires with the token it carries rather than outliving it.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Auth;

httpContext.Response.SetRefreshTokenCookie("refresh-token-value", days: 30);
```

<FrontmatterDocs/>

## Type signature

```csharp
public void SetRefreshTokenCookie(string token, int days);
```
