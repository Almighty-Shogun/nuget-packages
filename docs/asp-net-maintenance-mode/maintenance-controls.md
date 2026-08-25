# Maintenance Controls

Maintenance controls are application-owned entry points that call [`IMaintenanceService`](./services/maintenance-service). The package registers the service and the request and state types but ships no endpoint, because how maintenance mode is exposed and protected is an application decision.

Use the pattern for protected HTTP endpoints, deployment tooling, or any operational path that reads, enables, or disables maintenance mode.

::: danger
Apply your application's authentication and authorization to these controls; anyone who can reach them can take the application offline. Add the control route to `AllowedPathPrefixes`, or the middleware blocks the endpoint that would disable maintenance mode.
:::

## Controller

The controller below is exposed at `/ops/maintenance`, which is why `/ops` appears in the allow list alongside it.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.MaintenanceMode;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("ops/maintenance")]
public sealed class MaintenanceController(
    IMaintenanceService maintenanceService
) : ControllerBase
{
    [HttpGet]
    public Task<MaintenanceState> GetAsync()
        => maintenanceService.GetAsync();

    [HttpPost("enable")]
    public Task EnableAsync([FromBody] MaintenanceRequest request)
        => maintenanceService.EnableAsync(request);

    [HttpPost("disable")]
    public Task DisableAsync() 
        => maintenanceService.DisableAsync();
}
```

The body of `POST /ops/maintenance/enable` maps directly to [`MaintenanceRequest`](./types/maintenance-request), so a caller sends only the fields that differ from the configured defaults.

```json
{
    "Maintenance": {
        "AllowedPathPrefixes": ["/ops"]
    }
}
```
