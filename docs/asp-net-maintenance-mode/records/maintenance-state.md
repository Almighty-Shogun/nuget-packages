---
fields:
    - name: IsEnabled
      description: Whether maintenance mode is currently on.
      type: bool
      default: 'false'

    - name: Message
      description: The message for the current window, or the configured `DefaultMessage` when the request did not supply one.
      type: string?
      default: 'null'

    - name: StartsAt
      description: When the window becomes active. A window with a future `StartsAt` is enabled but not yet blocking traffic.
      type: DateTimeOffset?
      default: 'null'

    - name: EndsAt
      description: When the window ends. Also what the middleware writes `Retry-After` from.
      type: DateTimeOffset?
      default: 'null'

    - name: EnabledAt
      description: When maintenance mode was turned on.
      type: DateTimeOffset?
      default: 'null'
---

# MaintenanceState

A snapshot of maintenance mode, returned by [`IMaintenanceService.GetAsync`](../services/maintenance-service#getasync).

It carries only what a caller needs to report the current state; the allow lists and the redirect flag are persisted configuration and are not exposed here. Change maintenance mode through [`MaintenanceRequest`](./maintenance-request) rather than by writing the state file.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MaintenanceState
{
    public bool IsEnabled { get; init; }
    public string? Message { get; init; }
    public DateTimeOffset? StartsAt { get; init; }
    public DateTimeOffset? EndsAt { get; init; }
    public DateTimeOffset? EnabledAt { get; init; }
}
```
