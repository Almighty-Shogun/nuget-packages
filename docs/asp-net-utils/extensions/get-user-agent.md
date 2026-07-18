---
returns: The parsed `UserAgent` for the current request.
---

# GetUserAgent

Parses the current request's `User-Agent` header into the package's simplified `UserAgent` record. The method reads `HttpContext.Request.Headers.UserAgent`, converts the header value to a string, and delegates parsing to `UserAgent.Parse`.

Use this helper in controllers, endpoint handlers, and request-scoped services that need browser and device information without working directly with `UAParser`. Empty or missing header values produce a `UserAgent` with `Unknown` values.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("session")]
public sealed class SessionController : ControllerBase
{
    [HttpGet("user-agent")]
    public ActionResult<UserAgent> GetUserAgentInfo()
        => HttpContext.GetUserAgent();
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public UserAgent GetUserAgent();
```
