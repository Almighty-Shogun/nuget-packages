---
returns: The preferred language when the header contains a well-formed language tag; otherwise `null`.
---

# GetAcceptLanguage

Reads the preferred language from the request `Accept-Language` header. The header is a ranked list, and this takes the first entry and discards its quality value, so `nl-BE,nl;q=0.9,en;q=0.8` yields `nl-BE`. It returns `null` when the header is absent or its first entry is not a well-formed language tag, including the `*` wildcard.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("preferences")]
public sealed class PreferencesController : ControllerBase
{
    [HttpGet("language")]
    public IActionResult Get() => Ok(Request.GetAcceptLanguage() ?? "en");
}
```

::: warning
Rejecting a malformed tag is a security boundary, not a formatting nicety. The returned value is used to build a filesystem path when message files are resolved, so an unvalidated header would allow directory traversal out of the messages directory.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public string? GetAcceptLanguage();
```
