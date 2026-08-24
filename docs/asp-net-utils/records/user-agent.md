---
fields:
    - name: Browser
      description: Browser family and major version, such as `Chrome 120`.
      type: string

    - name: Os
      description: Operating-system family and major version, such as `Windows 10`.
      type: string

    - name: Device
      description: Device family, such as `iPhone`, or `Other` for a desktop browser.
      type: string

    - name: IsBot
      description: Whether the User-Agent was recognized as a crawler or spider.
      type: bool
      default: 'false'
---

# UserAgent

Simplified client information parsed from a User-Agent header, read through [`GetUserAgent`](../extensions/get-user-agent).

An absent or unparseable header yields `Unknown` for all three string values and `false` for `IsBot`, so the result is never null and never throws. Every field is pattern matching on a header the client chooses, so none of it is trustworthy enough to make an authorization or billing decision on.

## Usage

::: code-group

```csharp [AnalyticsController.cs]
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

        if (userAgent.IsBot)
        {
            return NoContent();
        }

        return Ok(new 
        {
            userAgent.Browser,
            userAgent.Os,
            userAgent.Device
        });
    }
}
```

```csharp [Parse.cs]
using AlmightyShogun.AspNet.Utils;

UserAgent userAgent = UserAgent.Parse(storedHeaderValue);
```

:::

## Parse

Parses a raw header value. The underlying parser is created once for the process, because building it compiles a large regular expression set.

### Type signature

```csharp
public static UserAgent Parse(string userAgent);
```

<FrontmatterDocs/>
