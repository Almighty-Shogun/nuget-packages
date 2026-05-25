---
outline: deep

params:
    - name: section
      description: Configuration section to bind to options type `T`.
      type: IConfigurationSection

returns: The `IServiceCollection` instance with the configured options registration.
---

# AddConfiguration

Registers a strongly typed options class and binds it to an `IConfigurationSection`. The method enables options binding, data-annotation validation, and startup validation through `ValidateOnStart`.

Use this helper when a package or application has a configuration record or class that should be injected through `IOptions<T>`. The method expects the caller to pass the specific section that should be bound, such as `builder.Configuration.GetSection("Email")`.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Utils;

builder.Services.AddConfiguration<ExampleSettings>(
    builder.Configuration.GetSection("Example")
);
```

```json [appsettings.json]
{
    "Example": {
        "Name": "Importer",
        "Enabled": true
    }
}
```

```csharp [ExampleSettings.cs]
public sealed record ExampleSettings
{
    public required string Name { get; init; }

    public bool Enabled { get; init; }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddConfiguration<T>(
    IConfigurationSection section
) where T : class;
```
