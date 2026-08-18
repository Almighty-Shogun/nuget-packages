---
returns: The same `IServiceCollection` instance with the session context filter configured.
---

# AddSessionContextFilter

Adds a global MVC filter that captures the client IP address and User-Agent into `HttpContext.Items` before each action runs, so [`GetSessionContext`](./get-session-context) returns the same values everywhere in the request.

The filter is registered through `MvcOptions`, so it composes with an existing `AddControllers` call rather than replacing it. Call order does not matter.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddControllers()
    .AddSessionContextFilter();
```

```csharp [SessionsController.cs]
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

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddSessionContextFilter();
```
