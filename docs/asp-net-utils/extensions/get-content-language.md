---
returns: The response content language, or `null` when the `Content-Language` header is missing or empty.
---

# GetContentLanguage

Reads the current `Content-Language` response header. The helper returns `null` when the header has not been set or contains only whitespace.

Use this method when response-building code needs to inspect which language was selected by the message resolver or by application-specific localization logic.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

var httpContext = new DefaultHttpContext();
httpContext.Response.SetContentLanguage("en");

string? language = httpContext.Response.GetContentLanguage();
```

<FrontmatterDocs/>

## Type signature

```csharp
public string? GetContentLanguage();
```
