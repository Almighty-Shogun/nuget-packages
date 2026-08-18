---
returns: The stored `SessionContext`, or one built from the connection address and User-Agent header when none is stored.
---

# GetSessionContext

Reads the current request's [`SessionContext`](../records/session-context) from `HttpContext.Items`.

When [`AddSessionContextFilter`](./add-session-context-filter) is registered, this returns the value the filter captured. Otherwise it builds one from the connection address and the User-Agent header, so it always returns a usable value and never throws.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

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
