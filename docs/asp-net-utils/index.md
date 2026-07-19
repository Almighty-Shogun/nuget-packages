# ASP.NET Utils

Adds small ASP.NET Core helpers for request context capture, CORS registration, MVC action filter setup, cookie cleanup, language headers, User-Agent parsing, and standardized HTTP error responses. The package is intended for APIs that need reusable request utilities without adopting a larger application framework.

Use this package when controllers or services need a consistent `SessionContext`, when startup code should register allowed origins from configuration, when application code needs parsed User-Agent details through `HttpContext`, or when API errors should be returned as predictable JSON objects with localized descriptions.

## Categories

- [Configuration](./configuration) &mdash; allowed origins and default message language read from `appsettings.json`.
- [Localization](./localization) &mdash; message-file structure and complete HTTP error message examples.
- [Extensions](./extensions/add-action-filters) &mdash; startup, request, response, and error-handling extension methods.
- [Services](./services/message-resolver) &mdash; DI contracts for resolving localized HTTP messages.
- [Types](./types/http-error-exception) &mdash; exception and MVC result helpers for standardized HTTP errors.
- [Records](./records/http-error-response) &mdash; immutable values returned by package helpers and error response middleware.

## Quick Example

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddActionFilters()
    .AddAllowedOrigins("DefaultCors", builder.Configuration)
    .AddHttpErrorResponses(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```
