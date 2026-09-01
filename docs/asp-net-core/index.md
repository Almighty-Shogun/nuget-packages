# ASP.NET Core

Provides the shared ASP.NET Core layer the other web packages build on: one standardized error response shape, exception mapping, request metadata capture, CORS setup, and forwarded-header configuration for a Cloudflare-proxied application.

Every error body produced by this repository is written in exactly one place, [`HttpErrorResponseWriter`](./services/http-error-response-writer), so the shape stays consistent and can be changed once.

Adopting it is a set of independent registrations followed by one middleware call, so an application can take the error response shape without the exception handlers, or the reverse. Everything beyond that is a helper reached from `HttpContext`, `HttpRequest`, or `HttpResponse`.

## Categories

- [Configuration](./configuration) &mdash; the optional `AllowedOrigins` section.
- [Exceptions](./exceptions) &mdash; the `IExceptionMapper` standard your own handler is built on.
- [HTTP Error Messages](./http-error-messages) &mdash; the `http-error.json` files every status description is resolved from.
- [Extensions](./extensions/add-exception-handling) &mdash; registration, middleware, and request helpers.
- [Services](./services/http-error-response-writer) &mdash; error response writing.
- [Utilities](./utilities/cloudflare-defaults) &mdash; the Cloudflare edge ranges and the MVC error result helper.
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
        OrderNotFoundException notFound => new ErrorMapping(
            StatusCodes.Status404NotFound,
            "order_not_found",
            "orders.not-found",
            [notFound.OrderId]
        ),
        _ => null
    };
}
```

:::
