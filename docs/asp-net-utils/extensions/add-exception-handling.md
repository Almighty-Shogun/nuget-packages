---
returns: The same `IServiceCollection` instance with the exception handlers registered.
---

# AddExceptionHandling

Registers the two exception handlers this package owns, in the order they have to run: framework exceptions that map to a status code of their own first, then the fallback that turns everything else into a `500`. Both are internal, so this call is the only way to get them, and because the fallback answers every exception, your own handler has to be registered before this call or the failures you throw deliberately all become a `500` too. It registers neither [`AddMessageLocalization`](/asp-net-localization/extensions/add-message-localization) nor [`AddHttpErrorResponseWriter`](./add-http-error-response-writer).

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```

::: warning
Registering the handlers is not enough on its own. [`UseHttpErrorResponses`](./use-http-error-responses) adds the middleware that runs the chain, and without it every exception escapes unhandled.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddExceptionHandling();
```
