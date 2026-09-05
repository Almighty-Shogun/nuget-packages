---
fields:
    - name: Message
      description: Message shown for this window. Falls back to `DefaultMessage` from configuration.
      type: string?
      default: 'null'

    - name: StartsAt
      description: When the window should begin. Until it passes, maintenance mode is enabled but traffic is served normally. Omit to start immediately.
      type: DateTimeOffset?
      default: 'null'

    - name: EndsAt
      description: When the window should end. Required for the middleware to send a `Retry-After` header, and for `AutoDisableWhenExpired` to have anything to act on. Must be after `StartsAt` when both are set.
      type: DateTimeOffset?
      default: 'null'

    - name: AutoDisableWhenExpired
      description: Whether the window turns itself off once `EndsAt` has passed. Falls back to configuration.
      type: bool?
      default: 'null'

    - name: RedirectBlockedRequests
      description: Whether blocked requests that accept `text/html` are redirected to the maintenance path. Falls back to configuration.
      type: bool?
      default: 'null'

    - name: AllowedPaths
      description: Exact paths that stay reachable for this window. Replaces the configured list rather than adding to it.
      type: 'IReadOnlyList<string>?'
      default: 'null'

    - name: AllowedPathPrefixes
      description: Path prefixes that stay reachable for this window. Replaces the configured list rather than adding to it.
      type: 'IReadOnlyList<string>?'
      default: 'null'

    - name: AllowedIpAddresses
      description: Client addresses that stay able to reach the application. Replaces the configured list rather than adding to it.
      type: 'IReadOnlyList<string>?'
      default: 'null'
---

# MaintenanceRequest

Describes a maintenance window, passed to [`IMaintenanceService.EnableAsync`](../services/maintenance-service#enableasync).

Every field is nullable and every omitted field falls back to [`MaintenanceSettings`](../configuration), so a caller sends only what differs from the configured defaults. A supplied list replaces the configured one for the duration of the window; it is not merged with it.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MaintenanceRequest
{
    public string? Message { get; init; }
    public DateTimeOffset? StartsAt { get; init; }
    public DateTimeOffset? EndsAt { get; init; }
    public bool? AutoDisableWhenExpired { get; init; }
    public bool? RedirectBlockedRequests { get; init; }
    public IReadOnlyList<string>? AllowedPaths { get; init; }
    public IReadOnlyList<string>? AllowedPathPrefixes { get; init; }
    public IReadOnlyList<string>? AllowedIpAddresses { get; init; }
}
```
