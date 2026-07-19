# MaintenanceState

Represents the persisted maintenance mode state read by the middleware and service. The package writes this type to `maintenance.json` when maintenance mode is enabled and reads it back on later requests or process restarts.

Application code usually receives this type from [`IMaintenanceService.GetAsync`](../services/maintenance-service#getasync). Treat it as a state snapshot: use [`MaintenanceRequest`](./maintenance-request) to change maintenance mode instead of creating or editing the state file manually.

## Usage

```csharp
using AlmightyShogun.AspNet.Maintenance;

MaintenanceState state = await maintenanceService.GetAsync();

if (state.IsEnabled)
{
    Console.WriteLine(state.Message ?? "Maintenance mode is enabled.");
}
```

## Type signature

```csharp
public sealed class MaintenanceState
{
    public bool IsEnabled { get; init; }
    public string? Message { get; init; }
    public DateTime? EndsAt { get; init; }
    public DateTime? EnabledAt { get; init; }
    public bool AutoDisableWhenExpired { get; init; }
    public bool RedirectBlockedRequests { get; init; }
    public IReadOnlyList<string> AllowedPaths { get; init; } = [];
    public IReadOnlyList<string> AllowedPathPrefixes { get; init; } = [];
}
```
