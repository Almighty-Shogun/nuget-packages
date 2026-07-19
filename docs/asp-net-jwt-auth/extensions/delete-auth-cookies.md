# DeleteAuthCookies

Deletes the authentication cookies managed by ASP.NET JWT Auth from an `HttpResponse`.

Use this helper in logout endpoints, token-revocation flows, or any place where the API should explicitly clear the package's refresh-token cookie. The helper keeps cookie naming centralized and deletes the cookie with the same path, `Secure`, and `SameSite.Lax` shape used when the cookie is written.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

var httpContext = new DefaultHttpContext();

httpContext.Response.DeleteAuthCookies();
```

## Type signature

```csharp
public void DeleteAuthCookies();
```
