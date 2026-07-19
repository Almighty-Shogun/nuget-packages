# MaintenanceService

Dependency-injection service for reading and changing the persisted maintenance mode state. [`AddMaintenanceMode`](../extensions/add-maintenance-mode) registers the package implementation for `IMaintenanceService`, and application code should depend on the interface when it needs to enable maintenance from a protected endpoint, disable it after work completes, or check the current state.

The implementation stores state in `maintenance.json` under the application content root. Reads and writes are serialized with an internal lock, corrupt or unreadable state files are treated as enabled maintenance mode, and expired states can disable themselves when `AutoDisableWhenExpired` is active.

## Usage

```csharp
using AlmightyShogun.AspNet.Maintenance;

public sealed class MaintenanceControls(IMaintenanceService maintenanceService)
{
    public Task EnableDeploymentWindowAsync()
    {
        return maintenanceService.EnableAsync(new MaintenanceRequest
        {
            Message = "Deployment in progress.",
            EndsAt = DateTime.UtcNow.AddMinutes(20),
            AutoDisableWhenExpired = true,
            AllowedPaths = ["/health"]
        });
    }
}
```

## GetAsync

Gets the current maintenance state. If no state file exists, the method returns a disabled state populated from configured defaults. If the persisted file is corrupt or unreadable, it returns an enabled fallback state so the application does not accidentally leave maintenance mode.

When `AutoDisableWhenExpired` is enabled and `EndsAt` has passed, the service clears the state file and returns a disabled state.

```csharp
using AlmightyShogun.AspNet.Maintenance;

public sealed class MaintenanceStatus(IMaintenanceService maintenanceService)
{
    public Task<MaintenanceState> GetStatusAsync()
        => maintenanceService.GetAsync();
}
```

### Type signature

```csharp
public Task<MaintenanceState> GetAsync();
```

## IsEnabledAsync

Checks whether maintenance mode is currently enabled. This method calls `GetAsync`, so it also applies expiration handling and corrupt-file fallback behavior.

```csharp
using AlmightyShogun.AspNet.Maintenance;

public sealed class MaintenanceBanner(IMaintenanceService maintenanceService)
{
    public Task<bool> ShouldShowAsync()
        => maintenanceService.IsEnabledAsync();
}
```

### Type signature

```csharp
public Task<bool> IsEnabledAsync();
```

## EnableAsync

Enables maintenance mode and writes a new state file. Values supplied in [`MaintenanceRequest`](../types/maintenance-request) override configured defaults for the new state; omitted values fall back to [`MaintenanceSettings`](../configuration/maintenance-settings).

Use this method from protected admin routes, deployment automation, or operational tooling that can decide when maintenance mode should begin.

```csharp
using AlmightyShogun.AspNet.Maintenance;

public sealed class DeploymentMaintenance(IMaintenanceService maintenanceService)
{
    public Task StartAsync()
    {
        return maintenanceService.EnableAsync(new MaintenanceRequest
        {
            Message = "Deployment in progress.",
            EndsAt = DateTime.UtcNow.AddMinutes(15),
            AutoDisableWhenExpired = true,
            RedirectBlockedRequests = false
        });
    }
}
```

### Type signature

```csharp
public Task EnableAsync(MaintenanceRequest request);
```

## DisableAsync

Disables maintenance mode by deleting the persisted state file when it exists. The method does not change configuration defaults; it only clears the current runtime state.

```csharp
using AlmightyShogun.AspNet.Maintenance;

public sealed class DeploymentMaintenance(IMaintenanceService maintenanceService)
{
    public Task FinishAsync()
        => maintenanceService.DisableAsync();
}
```

### Type signature

```csharp
public Task DisableAsync();
```
