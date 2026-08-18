---
returns: The parsed `UserAgent` for the current request.
---

# GetUserAgent

Parses the current request's User-Agent header into a [`UserAgent`](../records/user-agent) value.

Parsing runs on each call, so store the result rather than calling it repeatedly in the same request. The result is never `null`: an absent header yields `Unknown` throughout, and an unrecognized one yields `Other` for whichever part failed to match.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("analytics")]
public sealed class AnalyticsController : ControllerBase
{
    [HttpPost("visit")]
    public IActionResult Record()
    {
        UserAgent userAgent = HttpContext.GetUserAgent();

        return userAgent.IsBot ? NoContent() : Ok(new 
        {
            userAgent.Browser,
            userAgent.Os
        });
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public UserAgent GetUserAgent();
```
