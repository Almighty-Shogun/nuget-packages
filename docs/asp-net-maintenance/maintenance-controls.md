# Maintenance Controls

Maintenance controls are application-owned entry points that call `IMaintenanceService`. The package registers the service and provides the request/state types, but it does not ship a controller or endpoint because every application needs to decide how maintenance mode should be protected and exposed.

Use this pattern for protected HTTP endpoints, deployment tooling, background workflows, or any other operational path that needs to read, enable, or disable maintenance mode. When the controls must stay reachable during maintenance, add their route to `AllowedPaths` or `AllowedPathPrefixes`; otherwise the middleware can block the same endpoint that would disable maintenance mode.

::: warning
Protect maintenance controls with the authentication and authorization rules used by your application. Anyone who can call these controls can block or unblock normal traffic.
:::

## Controller

The example below exposes a small controller at `/ops/maintenance`. Configure that route as an allowed path prefix when the controls must stay reachable while maintenance mode is enabled.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Maintenance;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("ops/maintenance")]
public sealed class MaintenanceController(IMaintenanceService maintenanceService) : ControllerBase
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

The request body for `POST /ops/maintenance/enable` maps directly to `MaintenanceRequest`. Omitted values fall back to `MaintenanceSettings`, so callers can send only the fields that need to change for the current maintenance window.
