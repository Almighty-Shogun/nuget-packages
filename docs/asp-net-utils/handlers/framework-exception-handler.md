# FrameworkExceptionHandler

Maps common framework exceptions to the status code they deserve instead of letting them all become `500`. A malformed request body becomes the `400` it already is, and a client that hangs up mid-request is answered with `499` and no body, since there is no longer a client to read one. An exception it has no mapping for is declined and falls through to [`UnhandledExceptionHandler`](./unhandled-exception-handler).

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter(builder.Configuration)
    .AddExceptionHandler<FrameworkExceptionHandler>();
```

## Type signature

```csharp
public sealed class FrameworkExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    ILogger<FrameworkExceptionHandler> logger
) : IExceptionHandler;
```
