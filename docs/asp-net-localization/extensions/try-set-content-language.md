---
returns: '`true` when the header was set; `false` when the response had already started, or the value was blank or carried a control character, and nothing was written.'
---

# TrySetContentLanguage

Sets the response `Content-Language` header, but only while the response has not started. Once the first byte is written the headers are already on the wire, so a late call reports failure rather than throwing, as it does for a blank value or one carrying a control character. Call it when a response is deliberately in a language other than the negotiated one: [`UseMessageLocalization`](./use-message-localization) fills the header in only when nothing has set it.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Localization;

[ApiController]
[Route("legal")]
public sealed class LegalController : ControllerBase
{
    [HttpGet("terms")]
    public IActionResult GetTerms()
    {
        Response.TrySetContentLanguage("en");

        return Ok(LoadCanonicalEnglishTerms());
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public bool TrySetContentLanguage(string language);
```
