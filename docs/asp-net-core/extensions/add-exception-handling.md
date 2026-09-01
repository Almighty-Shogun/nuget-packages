---
params:
    - name: suppressMapClientErrors
      description: Whether MVC's client-error mapping is turned off. Left on, an `[ApiController]` rewrites a bodiless error result into `ProblemDetails`, which [`UseHttpErrorResponses`](./use-http-error-responses) then leaves alone.
      type: bool
      default: 'true'

returns: The same `IServiceCollection` instance with the exception handlers registered.
---

# AddExceptionHandling

Registers the two exception handlers this package owns, in the order they have to run: framework exceptions that map to a status code of their own first, then the fallback that turns everything else into a `500`. Both are internal, so this call is the only way to get them, and because the fallback answers every exception, your own handler has to be registered before this call or the failures you throw deliberately all become a `500` too. It registers neither [`AddMessageLocalization`](/asp-net-localization/extensions/add-message-localization) nor [`AddHttpErrorResponseWriter`](./add-http-error-response-writer).

## Usage

```csharp
using AlmightyShogun.AspNet.Core;

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

## Client error mapping

This call sets `SuppressMapClientErrors` on `ApiBehaviorOptions`. A controller marked `[ApiController]` otherwise rewrites a bodiless error result, such as a bare `NotFound()`, into a `ProblemDetails` body of its own, and [`UseHttpErrorResponses`](./use-http-error-responses) leaves that alone because it only fills in a response that has no body.

Pass `false` to keep `ProblemDetails` for the results MVC produces, so the standardized body applies only to errors raised below MVC.

```csharp
using AlmightyShogun.AspNet.Core;

builder.Services.AddExceptionHandling(suppressMapClientErrors: false);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddExceptionHandling(
    bool suppressMapClientErrors = true
);
```
