# DeleteAuthCookies

Deletes the authentication cookies managed by ASP.NET JWT Auth from an `HttpResponse`.

Use this helper in logout endpoints, token-revocation flows, or any place where the API should explicitly clear the package's refresh-token cookie. It deletes the cookie with the same path, `Secure` flag, and [`Auth:SameSite`](../configuration) mode used when the cookie is written, because a browser ignores a deletion whose attributes do not match.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

httpContext.Response.DeleteAuthCookies();
```

## Type signature

```csharp
public void DeleteAuthCookies();
```
