---
params:
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

This is the only response the package writes in its own shape. A request blocked by the middleware receives the shared error body from `IHttpErrorResponseWriter` instead.

## Usage

```csharp
using System.Net.Http.Json;
using AlmightyShogun.AspNet.MaintenanceMode;

MaintenanceResponse? details = await httpClient
    .GetFromJsonAsync<MaintenanceResponse>("/maintenance");
```

The response is serialized with the application's own JSON options. With the ASP.NET Core defaults that produces:

```json
{
    "message": "Upgrading the database. Back shortly.",
    "startsAt": null,
    "endsAt": "2026-09-01T04:00:00+00:00",
    "enabledAt": "2026-09-01T01:58:12+00:00"
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MaintenanceResponse(
    string? Message,
    DateTimeOffset? StartsAt,
    DateTimeOffset? EndsAt,
    DateTimeOffset? EnabledAt
);
```
