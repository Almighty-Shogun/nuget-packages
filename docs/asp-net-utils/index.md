# AspNet Utils

Provides the shared ASP.NET Core layer the other web packages build on: one standardized error response shape, localized messages resolved from JSON files, request metadata capture, CORS setup, and forwarded-header configuration for a Cloudflare-proxied application.

Every error body produced by this repository is written in exactly one place, [`HttpErrorResponseWriter`](./services/http-error-response-writer), so the shape stays consistent and can be changed once.

Adopting it is a set of independent registrations followed by two middleware calls, so an application can take message localization without the exception handlers, or the reverse. Everything beyond that is a helper reached from `HttpContext`, `HttpRequest`, or `HttpResponse`.

## Categories

- [Configuration](./configuration) &mdash; the optional `HttpErrors`, `Localization`, and `AllowedOrigins` sections.
- [Localization](./localization) &mdash; how message files are laid out, resolved, and negotiated per request.
- [Exceptions](./exceptions) &mdash; the `IAppException` contract every exception in this repository implements.
- [Extensions](./extensions/add-exception-handling) &mdash; registration, middleware, and request helpers.
- [Handlers](./handlers/app-exception-handler) &mdash; the exception handlers that turn a thrown exception into the standard error body.
- [Services](./services/message-resolver) &mdash; message resolution, language negotiation, and error response writing.
- [Utilities](./utilities/cloudflare) &mdash; the Cloudflare edge ranges and the MVC error result helper.
- [Records](./records/http-error-response) &mdash; the response body, session context, and parsed User-Agent.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter(builder.Configuration)
    .AddExceptionHandling()
    .AddHttpErrorResponseFilter()
    .AddSessionContextFilter();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```

```csharp [SessionsController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("sessions")]
public sealed class SessionsController(
    IMessageResolver messageResolver
) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() 
        => Ok(messageResolver.Resolve("auth.failed"));
}
```

```json [messages/en/auth.json]
{
    "failed": "Authentication failed",
    "expired": "Your session expired at {0}"
}
```

:::
