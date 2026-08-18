# SetContentLanguage

Sets the response `Content-Language` header, but only while the response has not started. Once the first byte is written the headers are already on the wire, so a late call is ignored rather than throwing. It replaces any language already set, and [`UseMessageLocalization`](./use-message-localization) already does this for every response.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("legal")]
public sealed class LegalController : ControllerBase
{
    [HttpGet("terms")]
    public IActionResult GetTerms()
    {
        Response.SetContentLanguage("en");

        return Ok(LoadCanonicalEnglishTerms());
    }
}
```

## Type signature

```csharp
public void SetContentLanguage(
    string language
);
```
