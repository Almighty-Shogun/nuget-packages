---
returns: The preferred request language, or `null` when the `Accept-Language` header is missing or empty.
---

# GetAcceptLanguage

Reads the preferred language from an HTTP request's `Accept-Language` header. The helper returns only the first language value and strips any quality value, so `en-US,en;q=0.9` becomes `en-US`.

Use this method when application code needs the caller's preferred language without parsing the full header format. Empty, whitespace-only, or missing headers return `null`.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

var httpContext = new DefaultHttpContext();
httpContext.Request.Headers.AcceptLanguage = "en-US,en;q=0.9";

string? language = httpContext.Request.GetAcceptLanguage();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string? GetAcceptLanguage();
```
