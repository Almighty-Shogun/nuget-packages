---
returns: The stored `ClientContext`, or one built from the connection address and User-Agent header when none is stored.
---

# GetClientContext

Reads the current request's [`ClientContext`](../records/client-context), falling back to building one from the connection address and the User-Agent header. It always returns a usable value and never throws. A built context is stored on the request, so only the first call reads the connection and every later call returns it, unless [`SetClientContext`](./set-client-context) pinned one first.

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
        ClientContext clientContext = HttpContext.GetClientContext();

        return Ok(new 
        {
            clientContext.IpAddress,
            clientContext.UserAgent
        });
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public ClientContext GetClientContext();
```
