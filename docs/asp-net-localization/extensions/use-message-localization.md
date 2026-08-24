---
returns: The same `IApplicationBuilder` instance with the message localization middleware configured.
---

# UseMessageLocalization

Adds the middleware that writes the `Content-Language` header from the language negotiated for the request, so a client can see which language the response body was resolved in. The header names the language that actually had messages defined for it, not the one the request asked for. It is set from a response callback, so the value is resolved after the body has been produced rather than when the middleware runs, and a header the application set itself is left alone.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Localization;

WebApplication app = builder.Build();

app.UseMessageLocalization();
app.UseHttpErrorResponses();
```

::: tip
Place it before any middleware that writes a localized body, including [`UseHttpErrorResponses`](/asp-net-utils/extensions/use-http-error-responses), so the header is set on error responses too.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IApplicationBuilder UseMessageLocalization();
```
