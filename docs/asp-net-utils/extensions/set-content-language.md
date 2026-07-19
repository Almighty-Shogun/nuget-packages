---
params:
    - name: language
      description: Language code to write to the response `Content-Language` header.
      type: string
---

# SetContentLanguage

Sets the response `Content-Language` header when the response has not started. If ASP.NET Core has already started sending the response, the method leaves the headers unchanged.

Use this helper when application code or a custom message resolver wants to tell clients which language was used for the response body. The package's internal language provider uses the same helper after resolving localized HTTP error messages.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

var httpContext = new DefaultHttpContext();

httpContext.Response.SetContentLanguage("en");
```

<FrontmatterDocs/>

## Type signature

```csharp
public void SetContentLanguage(string language);
```
