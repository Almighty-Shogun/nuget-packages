---
returns: The current request `SessionContext`.
---

# GetSessionContext

Returns the current request's `SessionContext` from `HttpContext.Items`. When no stored value exists, the method creates a fallback context from `HttpContext.Connection.RemoteIpAddress` and the current `User-Agent` request header.

If the reserved `SessionContext.ItemKey` exists but contains another value type, the method ignores that value and creates the same fallback context. This keeps the helper safe to call even when another component has written an unexpected value to `HttpContext.Items`.

Use this method in controllers, endpoint handlers, and request-scoped services that need the request IP address or raw User-Agent value. For the most consistent result, register `AddActionFilters` so the package filter captures forwarded IP information before actions run.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("session")]
public sealed class SessionController : ControllerBase
{
    [HttpGet]
    public ActionResult<SessionContext> Get()
        => HttpContext.GetSessionContext();
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public SessionContext GetSessionContext();
```
