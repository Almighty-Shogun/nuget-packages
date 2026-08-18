# UnhandledExceptionHandler

Writes the standardized `500` body for anything the earlier handlers did not recognize, so an unexpected exception returns the same shape as every deliberate error rather than an empty response or a stack trace.

Registered last by [`AddExceptionHandling`](../extensions/add-exception-handling). It handles every exception, so any handler registered after it never runs.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter(builder.Configuration)
    .AddExceptionHandler<UnhandledExceptionHandler>();
```

::: warning
Register it last. Because it handles everything, a handler registered after it is unreachable.
:::

## Type signature

```csharp
public sealed class UnhandledExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter
) : IExceptionHandler;
```
