# ASP.NET Core

Provides the shared ASP.NET Core layer the other web packages build on: one standardized error response shape, exception mapping, request metadata capture, CORS setup, and forwarded-header configuration for a Cloudflare-proxied application.

Every error body shares one shape, whether [`HttpErrorResponseWriter`](./services/http-error-response-writer) writes it from middleware and exception handlers or an action returns it as an [`HttpErrorResult`](./types/http-error-result), so a client sees the same envelope wherever the failure came from.

Adopting it is a set of independent registrations followed by one middleware call, so an application can take the error response shape without the exception handlers, or the reverse. Everything beyond that is a helper reached from `HttpContext`, `HttpRequest`, or `HttpResponse`.

## Categories

- [Configuration](./configuration) &mdash; the optional `AllowedOrigins` section.
- [Exceptions](./exceptions) &mdash; the `IExceptionMapper` standard your own handler is built on.
- [HTTP Error Messages](./http-error-messages) &mdash; the `http-error.json` files every status description is resolved from.
- [Extensions](./extensions/add-exception-handling) &mdash; registration, middleware, and request helpers.
- [Services](./services/http-error-response-writer) &mdash; error response writing.
- [Utilities](./utilities/cloudflare-defaults) &mdash; the Cloudflare edge ranges and the client address header.
- [Types](./types/http-error-result) &mdash; the MVC result that returns the standardized error body.
- [Records](./records/http-error-response) &mdash; the response body, session context, and parsed User-Agent.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;

builder.Services
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```

```csharp [OrderNotFoundException.cs]
public sealed class OrderNotFoundException(int orderId)
    : Exception($"Order {orderId} was not found.")
{
    public int OrderId { get; } = orderId;
}
```

```csharp [OrderExceptionMapper.cs]
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

public sealed class OrderExceptionMapper : IExceptionMapper
{
    public ErrorMapping? Map(Exception exception) => exception switch
    {
        OrderNotFoundException notFound => new ErrorMapping
        {
            StatusCode = StatusCodes.Status404NotFound,
            Code = "order_not_found",
            MessageKey = "orders.not-found",
            MessageParameters = [notFound.OrderId]
        },
        _ => null
    };
}
```

:::
