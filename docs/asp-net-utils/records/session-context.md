---
outline: deep

returns: A request metadata record containing the client IP address, raw User-Agent value, and `HttpContext.Items` key.
---

# SessionContext

Represents request metadata captured for the current HTTP request. `AddActionFilters` registers the package action filter that stores a `SessionContext` in `HttpContext.Items` before controller actions run.

Use this record when application code needs a lightweight view of the caller's IP address and raw User-Agent header. `GetSessionContext` returns the stored value when it exists and creates a fallback value from the current `HttpContext` when the filter has not populated one yet.

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
