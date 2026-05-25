---
outline: deep
---

# DeleteAuthCookies

Deletes the authentication cookies managed by ASP.NET JWT Auth from an `HttpResponse`.

Use this helper in logout endpoints, token-revocation flows, or any place where the API should explicitly clear the package's refresh-token cookie. The helper keeps cookie naming centralized so application code does not need to repeat the raw cookie name.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.JwtAuth;

var httpContext = new DefaultHttpContext();

httpContext.Response.DeleteAuthCookies();
```

<FrontmatterDocs/>

## Type signature

```csharp
public void DeleteAuthCookies();
```

## Uses

- [CookieNames](../constants/cookie-names)
