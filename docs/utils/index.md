# Utils

Provides the shared building blocks used by the other packages in this repository and by applications that want the same helpers without taking a dependency on a more specific package. The package covers assembly scanning for dependency-injection registration, strongly typed configuration binding, JSON deserialization helpers, and small console utilities.

The APIs are intentionally narrow and are most useful in startup code, worker services, and packages that need to discover types across assemblies.

## Categories

- [Extensions](./extensions/add-configuration) &mdash; configuration binding, service registration, and JSON deserialization extension methods.
- [Attributes](./attributes/skip-auto-registration) &mdash; opt a type out of assembly scanning.
- [Services](./services/service-registry) &mdash; the service registry module contract.
- [Utilities](./utilities/console-utils) &mdash; console helpers and assembly type discovery.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;
using Microsoft.Extensions.DependencyInjection;

ConsoleUtils.Title("Importer");

builder.Services
    .AddConfiguration<WorkerSettings>(
        builder.Configuration.GetSection("Worker")
    )
    .RegisterOnInherit<IImportStep>(ServiceLifetime.Scoped);
```

```csharp [WorkerSettings.cs]
using System.ComponentModel.DataAnnotations;

public sealed class WorkerSettings
{
    [Required]
    public required string Name { get; init; }

    [Range(1, 10000)]
    public int BatchSize { get; init; } = 100;
}
```

```json [appsettings.json]
{
    "Worker": {
        "Name": "Importer",
        "BatchSize": 500
    }
}
```

:::
