# MaintenanceService

Reads and changes maintenance mode. [`AddMaintenanceMode`](../extensions/add-maintenance-mode) registers the implementation for `IMaintenanceService`, and application code depends on the interface.

State is held in `maintenance.json` under the application content root and cached in memory, with writes updating the cache directly so a change takes effect on the next request whether or not the file watcher fires.

::: warning
Each instance keeps its own file and its own cache. In a multi-instance deployment, either point every instance at a shared content root or call `EnableAsync` on each one, or some instances stay online while others do not. Editing `maintenance.json` by hand relies on the file watcher, which does not fire reliably on container bind mounts or network filesystems.
:::

## Usage

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

public sealed class MaintenanceControls(
    IMaintenanceService maintenanceService
)
{
    public Task EnableDeploymentWindowAsync()
        => maintenanceService.EnableAsync(new MaintenanceRequest
        {
            Message = "Deployment in progress.",
            EndsAt = DateTimeOffset.UtcNow.AddMinutes(20),
            AutoDisableWhenExpired = true,
            AllowedPaths = ["/health"]
        });
}
```

## GetAsync

Returns the current [`MaintenanceState`](../types/maintenance-state). With no state file, it returns a disabled state built from the configured defaults.

When `AutoDisableWhenExpired` is on and `EndsAt` has passed, the state file is cleared and a disabled state is returned.

An unparseable state file is treated as enabled, so a damaged file cannot silently reopen an application that was meant to be closed. A file that merely cannot be read right now, because another process holds it or storage is briefly unavailable, is retried and then falls back to the last known state instead.

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

public sealed class MaintenanceStatus(
    IMaintenanceService maintenanceService
)
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

Whether maintenance mode is on. Reads the same state as `GetAsync`, including the expiry and read-failure handling.

This reports whether maintenance mode is enabled, not whether traffic is currently blocked. A window with a future `StartsAt` is enabled while requests are still served normally.

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

public sealed class MaintenanceBanner(
    IMaintenanceService maintenanceService
)
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

Turns maintenance mode on and writes the state file. Values on [`MaintenanceRequest`](../types/maintenance-request) apply to this window; omitted values fall back to [`MaintenanceSettings`](../configuration).

Calling it while a window is already active replaces that window rather than merging with it.

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

public sealed class DeploymentMaintenance(
    IMaintenanceService maintenanceService
)
{
    public Task StartAsync()
        => maintenanceService.EnableAsync(new MaintenanceRequest
        {
            Message = "Deployment in progress.",
            EndsAt = DateTimeOffset.UtcNow.AddMinutes(15),
            AutoDisableWhenExpired = true,
            RedirectBlockedRequests = false
        });
}
```

### Type signature

```csharp
public Task EnableAsync(MaintenanceRequest request);
```

## DisableAsync

Turns maintenance mode off by removing the persisted state. Configuration defaults are untouched, so the next `EnableAsync` starts from them again.

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

public sealed class DeploymentMaintenance(
    IMaintenanceService maintenanceService
)
{
    public Task FinishAsync()
        => maintenanceService.DisableAsync();
}
```

### Type signature

```csharp
public Task DisableAsync();
```
