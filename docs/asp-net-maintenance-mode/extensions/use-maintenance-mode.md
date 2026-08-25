---
returns: The same `IApplicationBuilder` instance with the maintenance middleware added to the pipeline.
---

# UseMaintenanceMode

Adds the maintenance mode middleware to the request pipeline.

Its position decides what maintenance mode can take offline. Place it after middleware that must run even while the application is offline, and before endpoint routing so application routes are covered.

## Usage

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

WebApplication app = builder.Build();

app.UseForwardedHeaders();
app.UseMaintenanceMode();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
```

## Request handling

A window is active when maintenance mode is enabled and `StartsAt` has passed or was not set. While no window is active every request is served normally, and the middleware performs no file access.

While a window is active, each request is handled in this order:

1. A request for `MaintenancePath` returns [`MaintenanceResponse`](../types/maintenance-response) with `503`. Outside an active window that path returns `404`.
2. A request matching `AllowedPaths` exactly, or `AllowedPathPrefixes` on a segment boundary, or coming from an address in `AllowedIpAddresses`, is passed through to the application.
3. Every other request is blocked.

Blocked requests receive `Retry-After` when the window has an `EndsAt`. No header is sent without one, because the remaining time is not known.

A blocked request that accepts `text/html` is redirected to `MaintenancePath` when `RedirectBlockedRequests` is on. Any other blocked request, including every API client, receives `503` with the shared error body used across the stack:

```json
{
    "code": 503,
    "error": "service_unavailable",
    "errorDescription": "Upgrading the database. Back shortly."
}
```

::: warning
`AllowedIpAddresses` is matched against the connection address, never against a forwarded header, because a header-supplied address is chosen by the caller. Behind a proxy or CDN, configure forwarded headers so the connection address is the real client; otherwise the allow list sees the proxy.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IApplicationBuilder UseMaintenanceMode();
```
