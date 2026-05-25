# ASP.NET Utils

Adds small ASP.NET Core helpers for request context capture, CORS registration, MVC action filter setup, cookie cleanup, and User-Agent parsing. The package is intended for APIs that need a few reusable request utilities without adopting a larger application framework.

Use this package when controllers or services need a consistent `SessionContext`, when startup code should register allowed origins from configuration, or when application code needs to parse raw `User-Agent` header values into a simpler browser/device record.

## Categories

- [Classes](./classes/user-agent-parser/) &mdash; public utility classes with focused ASP.NET request behavior.
- [Extensions](./extensions/add-action-filters) &mdash; startup, request, and response extension methods.
- [Records](./records/session-context) &mdash; small immutable values returned by package helpers.

## Quick Example

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddActionFilters()
    .AddAllowedOrigins("DefaultCors", builder.Configuration);
```
