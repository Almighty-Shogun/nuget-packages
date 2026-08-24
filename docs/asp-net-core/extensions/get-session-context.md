---
returns: The seeded `SessionContext`, or one built from the connection address and User-Agent header when none is stored.
---

# GetSessionContext

Reads the current request's [`SessionContext`](../records/session-context) from `HttpContext.Items`, falling back to building one from the connection address and the User-Agent header. It always returns a usable value and never throws. A built context is not stored, so each call reads the request again.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;

[ApiController]
[Route("sessions")]
public sealed class SessionsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
    {
        SessionContext sessionContext = HttpContext.GetSessionContext();

        return Ok(new 
        {
            sessionContext.IpAddress,
            sessionContext.UserAgent
        });
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public SessionContext GetSessionContext();
```
