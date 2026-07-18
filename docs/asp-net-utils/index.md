# ASP.NET Utils

Adds small ASP.NET Core helpers for request context capture, CORS registration, MVC action filter setup, cookie cleanup, and User-Agent parsing. The package is intended for APIs that need a few reusable request utilities without adopting a larger application framework.

Use this package when controllers or services need a consistent `SessionContext`, when startup code should register allowed origins from configuration, or when application code needs parsed User-Agent details through `HttpContext`.

## Categories

- [Configuration](./configuration) &mdash; allowed-origin configuration used by the CORS registration helper.
- [Extensions](./extensions/add-action-filters) &mdash; startup, request, and response extension methods.
- [Records](./records/session-context) &mdash; small immutable values returned by package helpers.

## Quick Example

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddActionFilters()
    .AddAllowedOrigins("DefaultCors", builder.Configuration);
```
