# Utils

Provides shared helpers used by the other packages in this repository and by applications that need the same small building blocks directly. The package contains console helpers, reflection-based type discovery, JSON serialization extension methods, dependency-injection registration helpers, and the `IServiceRegistry` module contract.

Use this package when an application needs lightweight infrastructure utilities without taking a dependency on one of the more specific packages. The APIs are intentionally small and are most useful in startup code, command-line applications, and packages that need to scan assemblies or bind strongly typed options.

## Categories

- [Extensions](./extensions/add-configuration) &mdash; JSON and dependency-injection extension methods.
- [Services](./services/service-registry) &mdash; service registry contracts used by the DI helpers.
- [Types](./types/application-utils) &mdash; console and assembly scanning helpers.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;

ApplicationUtils.Title("Worker");

builder.Services.AddConfiguration<WorkerSettings>(
    builder.Configuration.GetSection("Worker")
);
```

```json [appsettings.json]
{
    "Worker": {
        "Name": "Importer",
        "Enabled": true
    }
}
```

```csharp [WorkerSettings.cs]
public sealed record WorkerSettings
{
    public required string Name { get; init; }

    public bool Enabled { get; init; }
}
```

:::
