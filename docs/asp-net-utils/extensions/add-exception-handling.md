---
returns: The same `IServiceCollection` instance with the exception handlers registered.
---

# AddExceptionHandling

Registers the three exception handlers as one chain, in the order they have to run: [`AppExceptionHandler`](../handlers/app-exception-handler) first because an [`IAppException`](../exceptions) carries its own status code, then [`FrameworkExceptionHandler`](../handlers/framework-exception-handler), then [`UnhandledExceptionHandler`](../handlers/unhandled-exception-handler) for everything else.

Handlers run in registration order and the fallback handles every exception, so anything after it would never run. It registers neither [`AddMessageLocalization`](./add-message-localization) nor [`AddHttpErrorResponseWriter`](./add-http-error-response-writer), and the chain does not run until [`UseHttpErrorResponses`](./use-http-error-responses) is called.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter(builder.Configuration)
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
