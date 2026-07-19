---
params:
    - name: configuration
      description: Application configuration that may contain the `Maintenance` section.
      type: IConfiguration

returns: The same `IServiceCollection` instance with maintenance options and services registered.
---

# AddMaintenanceMode

Registers ASP.NET Maintenance services in dependency injection. The method binds `MaintenanceSettings` from the `Maintenance` configuration section and registers the file-backed implementation behind `IMaintenanceService`.

Use this during application startup before resolving `IMaintenanceService` or adding the middleware. If the `Maintenance` section is missing, the package keeps the default values defined by `MaintenanceSettings`.

## Usage

```csharp
using AlmightyShogun.AspNet.Maintenance;

builder.Services.AddMaintenanceMode(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddMaintenanceMode(
    IConfiguration configuration
);
```
