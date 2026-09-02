# Installation

Install `AlmightyShogun.AspNet.MaintenanceMode` in the ASP.NET Core application that needs to block traffic during maintenance windows. The package targets `net10.0` and requires the ASP.NET Core shared framework.

```sh
dotnet add package AlmightyShogun.AspNet.MaintenanceMode
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; middleware, HTTP, and hosting APIs.

### Project references

- `AlmightyShogun.AspNet.Core` &mdash; supplies the shared error response writer, so a blocked request returns the same error body as every other error in the stack.
- `AlmightyShogun.Utils` &mdash; configuration binding and JSON helpers.

## Startup Registration

[`AddMaintenanceMode`](./extensions/add-maintenance-mode) binds the optional `Maintenance` section and registers [`IMaintenanceService`](./services/maintenance-service), which application code uses to turn maintenance on and off. [`UseMaintenanceMode`](./extensions/use-maintenance-mode) adds the middleware that blocks traffic while a window is active.

::: warning
`UseMaintenanceMode` must come before endpoint mapping, and after any middleware that has to run while the application is offline.
:::

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

builder.Services.AddMaintenanceMode(builder.Configuration);

WebApplication app = builder.Build();

app.UseMaintenanceMode();
```
