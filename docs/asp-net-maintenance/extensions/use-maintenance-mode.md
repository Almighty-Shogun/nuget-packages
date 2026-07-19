---
returns: The same `IApplicationBuilder` instance with the maintenance middleware added to the pipeline.
---

# UseMaintenanceMode

Adds the maintenance mode middleware to the ASP.NET Core request pipeline. The middleware reads the current [`MaintenanceState`](../types/maintenance-state) through [`IMaintenanceService`](../services/maintenance-service), allows only the exact paths and path prefixes stored in that state, and blocks or redirects normal requests while maintenance mode is enabled.

Use this after building the application pipeline. Place it before endpoint execution so regular application routes can be blocked, while still leaving earlier infrastructure middleware in place when needed.

## Usage

```csharp
using AlmightyShogun.AspNet.Maintenance;

WebApplication app = builder.Build();

app.UseMaintenanceMode();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IApplicationBuilder UseMaintenanceMode();
```
