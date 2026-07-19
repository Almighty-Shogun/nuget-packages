# MaintenanceRequest

Represents the values used when enabling maintenance mode through [`IMaintenanceService.EnableAsync`](../services/maintenance-service#enableasync). Every property is optional so callers can provide only the values that should differ from [`MaintenanceSettings`](../configuration/maintenance-settings).

Use this type from protected endpoints, deployment workflows, or operational tools that enable maintenance mode. The service resolves missing values from configuration before writing the persisted [`MaintenanceState`](./maintenance-state).

## Usage

```csharp
using AlmightyShogun.AspNet.Maintenance;

MaintenanceRequest request = new()
{
    Message = "Database migration in progress.",
    EndsAt = DateTime.UtcNow.AddMinutes(30),
    AutoDisableWhenExpired = true,
    AllowedPaths = ["/health"],
    AllowedPathPrefixes = ["/admin"]
};
```

## Type signature

```csharp
public sealed class MaintenanceRequest
{
    public string? Message { get; init; }
    public DateTime? EndsAt { get; init; }
    public bool? AutoDisableWhenExpired { get; init; }
    public bool? RedirectBlockedRequests { get; init; }
    public IReadOnlyList<string>? AllowedPaths { get; init; }
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }
}
```
