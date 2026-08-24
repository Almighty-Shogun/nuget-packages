---
returns: The accepted languages in client preference order, or an empty list when the header is absent.
---

# GetAcceptLanguages

Reads every language from the request `Accept-Language` header, ordered by the quality value the client assigned, highest first, counting a missing weight as `1`. An entry weighted `0` is dropped rather than ranked last, since that is the client saying it will not accept that language, and so are the `*` wildcard and any malformed tag. Repeats are removed case-insensitively, so a header naming a language twice reads its directory once. It reads the whole header rather than only its first entry, which is what lets [language negotiation](../localization#language-negotiation) try a lower-ranked language before falling back to the default. Use [`GetAcceptLanguage`](./get-accept-language) when only the top preference matters.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Localization;

[ApiController]
[Route("preferences")]
public sealed class PreferencesController : ControllerBase
{
    [HttpGet("languages")]
    public IActionResult Get()
    {
        IReadOnlyList<string> languages = Request.GetAcceptLanguages();

        return Ok(languages.Count > 0 ? languages : ["en"]);
    }
}
```

::: warning
Rejecting a malformed tag is a security boundary. Each value is used to build a filesystem path when message files are resolved, so an unvalidated header would allow directory traversal out of the messages directory.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IReadOnlyList<string> GetAcceptLanguages();
```
