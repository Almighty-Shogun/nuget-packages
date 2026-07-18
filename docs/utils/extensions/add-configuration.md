---
params:
    - name: section
      description: Configuration section that is bound to the options type `T`.
      type: IConfigurationSection

returns: The same `IServiceCollection` instance with options binding, validation, and startup validation configured.
---

# AddConfiguration

Registers a strongly typed options class and binds it to an `IConfigurationSection`. The method enables options binding, data-annotation validation, and startup validation through `ValidateOnStart`.

Use this helper when a package or application has a configuration record or class that should be injected through `IOptions<T>`. Pass the exact section that should be bound, such as `builder.Configuration.GetSection("Email")`; the method does not choose the section name for you.

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
