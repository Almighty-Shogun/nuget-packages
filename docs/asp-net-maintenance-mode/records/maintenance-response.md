---
fields:
    - name: Message
      description: The message for the active window.
      type: string?

    - name: StartsAt
      description: When the window became, or becomes, active.
      type: DateTimeOffset?

    - name: EndsAt
      description: When the window ends, when an end time was given.
      type: DateTimeOffset?

    - name: EnabledAt
      description: When maintenance mode was turned on.
      type: DateTimeOffset?
---

# MaintenanceResponse

The body returned by the maintenance details endpoint at `MaintenancePath` while a window is active, sent with `503 Service Unavailable`.

This is the only response the package writes in its own shape. A request blocked by the middleware receives the shared error body from [`IHttpErrorResponseWriter`](/asp-net-core/services/http-error-response-writer) instead.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MaintenanceResponse
{
    public required string? Message { get; init; }
    public required DateTimeOffset? StartsAt { get; init; }
    public required DateTimeOffset? EndsAt { get; init; }
    public required DateTimeOffset? EnabledAt { get; init; }
}
```
