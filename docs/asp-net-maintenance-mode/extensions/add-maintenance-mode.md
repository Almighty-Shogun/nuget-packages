---
params:
    - name: configuration
      description: Application configuration that may contain the `Maintenance` section.
      type: IConfiguration

returns: The same `IServiceCollection` instance with maintenance options and services registered.
---

# AddMaintenanceMode

Binds [`MaintenanceSettings`](../configuration) from the `Maintenance` configuration section, registers the file-backed store behind [`IMaintenanceService`](../services/maintenance-service), and registers the shared error response writer used for blocked requests.

Call it during startup, before resolving [`IMaintenanceService`](../services/maintenance-service) or adding the middleware. The `Maintenance` section is optional.

## Usage

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

builder.Services.AddMaintenanceMode(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddMaintenanceMode(
    IConfiguration configuration
);
```
