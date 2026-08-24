---
returns: The preferred language when the header contains a well-formed language tag; otherwise `null`.
---

# GetAcceptLanguage

Reads the highest-ranked language from the request `Accept-Language` header. The header is a ranked list, and this returns the top of it after the quality values are applied, so `nl;q=0.2,fr;q=0.9` yields `fr` rather than `nl`. Malformed tags and the `*` wildcard are discarded first, so it returns the best *valid* entry, and `null` only when the header is absent or holds no valid tag at all.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Localization;

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
