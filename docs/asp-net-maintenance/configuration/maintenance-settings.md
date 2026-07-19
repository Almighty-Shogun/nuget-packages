---
fields:
    - name: MaintenancePath
      description: Request path that returns maintenance details while maintenance mode is enabled.
      type: string
      default: /maintenance

    - name: DefaultMessage
      description: Fallback message used when a maintenance request does not provide its own message.
      type: string?
      default: 'null'

    - name: AutoDisableWhenExpired
      description: Disables maintenance mode automatically when the persisted `EndsAt` value has passed.
      type: bool
      default: 'false'

    - name: RedirectBlockedRequests
      description: Redirects blocked requests to `MaintenancePath` when `true`; writes the maintenance response directly when `false`.
      type: bool
      default: 'true'

    - name: AllowedPaths
      description: Exact request paths that remain available while maintenance mode is enabled.
      type: 'List<string>'
      default: '[]'

    - name: AllowedPathPrefixes
      description: Request path prefixes that remain available while maintenance mode is enabled.
      type: 'List<string>'
      default: '[]'
---

# MaintenanceSettings

Represents the `Maintenance` configuration section consumed by ASP.NET Maintenance. The package binds this class during [`AddMaintenanceMode`](../extensions/add-maintenance-mode) and uses the values as defaults for the middleware and for maintenance states created by [`IMaintenanceService`](../services/maintenance-service).

Application code normally does not create `MaintenanceSettings` manually. Configure it through `appsettings.json`, then inject `IOptions<MaintenanceSettings>` only when application-specific code needs to inspect the configured maintenance path, redirect behavior, or allowed routes.

## Usage

::: tip
The JSON shape is documented on the [configuration page](../configuration). The example below shows how application services can consume the already-bound options.
:::

```csharp
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.Maintenance;

public sealed class MaintenanceLinkBuilder(IOptions<MaintenanceSettings> options)
{
    public string GetDetailsPath()
        => options.Value.MaintenancePath;
}
```

<FrontmatterDocs/>
