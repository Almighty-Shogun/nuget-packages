---
returns: The response `Content-Language` header when set; otherwise `null`.
---

# GetContentLanguage

Reads the response `Content-Language` header, returning `null` when it is absent or blank.

The middleware from [`UseMessageLocalization`](./use-message-localization) sets this header from the language message resolution settled on for the request. Multiple languages come back joined by commas rather than as separate values.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AlmightyShogun.AspNet.Localization;

public sealed class LanguageAuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ILogger<LanguageAuditMiddleware> logger
    )
    {
        await next(context);

        logger.LogDebug(
            "Responded in {Language}",
            context.Response.GetContentLanguage() ?? "unset"
        );
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public string? GetContentLanguage();
```
