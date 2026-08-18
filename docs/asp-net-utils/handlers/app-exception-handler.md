# AppExceptionHandler

Turns any exception implementing [`IAppException`](../exceptions) into the standardized error response. It reads the status code and error code from the exception, resolves `MessageKey` through [`IMessageResolver`](../services/message-resolver), and writes the body through [`IHttpErrorResponseWriter`](../services/http-error-response-writer).

Registered first by [`AddExceptionHandling`](../extensions/add-exception-handling), because an application exception carries its own status code and is the most specific match available.

## Usage

Register it through [`AddExceptionHandling`](../extensions/add-exception-handling) unless you want only part of the chain, in which case register it directly:

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter(builder.Configuration)
    .AddExceptionHandler<AppExceptionHandler>();
```

## Type signature

```csharp
public sealed class AppExceptionHandler(
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter,
    IOptions<HttpErrorSettings> errorOptions,
    ILogger<AppExceptionHandler> logger
) : IExceptionHandler;
```
