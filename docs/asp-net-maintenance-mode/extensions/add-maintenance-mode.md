---
params:
    - name: configuration
      description: Application configuration that may contain the `Maintenance` section.
      type: IConfiguration

returns: The same `IServiceCollection` instance with maintenance options and services registered.
---

# AddMaintenanceMode

Binds [`MaintenanceSettings`](../configuration) from the `Maintenance` configuration section and registers the file-backed store behind [`IMaintenanceService`](../services/maintenance-service).

Call it during startup, before resolving [`IMaintenanceService`](../services/maintenance-service) or adding the middleware. The `Maintenance` section is optional.

## Usage

```csharp
using AlmightyShogun.AspNet.MaintenanceMode;

builder.Services.AddMaintenanceMode(builder.Configuration);
```

::: warning
This does not register the shared error response writer. Call [`AddHttpErrorResponseWriter`](/asp-net-core/extensions/add-http-error-response-writer) as well, or the middleware cannot write the body a blocked request receives.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddMaintenanceMode(
    IConfiguration configuration
);
```
