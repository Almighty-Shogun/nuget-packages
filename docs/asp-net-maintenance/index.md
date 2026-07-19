# ASP.NET Maintenance

Adds file-backed maintenance mode to ASP.NET Core applications. The package provides startup extensions, middleware, a DI service for enabling or disabling maintenance mode, and small request/state types for application code that wants to control maintenance from a protected endpoint or background workflow.

Use this package when an application needs a simple maintenance switch that survives process restarts without requiring a database table. The current state is stored in `maintenance.json` under the application content root, and the middleware blocks normal requests while still allowing only the exact paths and path prefixes provided through configuration or the active maintenance request.

## Categories

- [Configuration](./configuration/maintenance-settings) &mdash; settings for the maintenance endpoint, default message, expiration behavior, redirects, and allowed paths.
- [Maintenance Controls](./maintenance-controls) &mdash; user-owned endpoints or workflows for reading, enabling, and disabling maintenance mode.
- [Extensions](./extensions/add-maintenance-mode) &mdash; service and middleware registration methods.
- [Services](./services/maintenance-service) &mdash; the DI service used to read, enable, and disable maintenance mode.
- [Types](./types/maintenance-request) &mdash; request and state objects used by the maintenance service.

## Quick Example

```csharp
using AlmightyShogun.AspNet.Maintenance;

builder.Services.AddMaintenanceMode(builder.Configuration);

WebApplication app = builder.Build();

app.UseMaintenanceMode();
```
