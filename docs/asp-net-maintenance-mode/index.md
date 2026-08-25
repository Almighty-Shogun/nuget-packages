# ASP.NET Maintenance Mode

File-backed maintenance mode for ASP.NET Core. Middleware blocks traffic while a maintenance window is active, a service turns the window on and off, and the state survives process restarts without a database table or an external coordinator.

It ships no endpoint for controlling maintenance mode, because how that is exposed and who may call it is an application decision.

## Categories

- [Configuration](./configuration) &mdash; maintenance path, default message, expiry behavior, redirects, and the allow lists.
- [Maintenance Controls](./maintenance-controls) &mdash; application-owned endpoints or workflows for reading, enabling, and disabling maintenance mode.
- [Extensions](./extensions/add-maintenance-mode) &mdash; service and middleware registration.
- [Services](./services/maintenance-service) &mdash; the DI contract used to read, enable, and disable maintenance mode.
- [Types](./types/maintenance-request) &mdash; the request, state, and response records.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.MaintenanceMode;

builder.Services.AddMaintenanceMode(builder.Configuration);

WebApplication app = builder.Build();

app.UseMaintenanceMode();
```

```csharp [DeploymentMaintenance.cs]
using AlmightyShogun.AspNet.MaintenanceMode;

public sealed class DeploymentMaintenance(
    IMaintenanceService maintenanceService
)
{
    public async Task StartAsync()
    {
        var request = new MaintenanceRequest 
        {
            Message = "Upgrading the database. Back shortly.",
            EndsAt = DateTimeOffset.UtcNow.AddMinutes(30),
            AutoDisableWhenExpired = true,
            AllowedPaths = ["/health"],
            AllowedPathPrefixes = ["/ops"]
        };
        
        await maintenanceService.EnableAsync(request);
    }

    public aysnc Task FinishAsync() 
        => await maintenanceService.DisableAsync();
}
```

```json [appsettings.json]
{
    "Maintenance": {
        "MaintenancePath": "/maintenance",
        "DefaultMessage": "The application is temporarily unavailable.",
        "AllowedPaths": ["/health"]
    }
}
```

:::
