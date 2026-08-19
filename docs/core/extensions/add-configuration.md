---
params:
    - name: section
      description: Configuration section the options type is bound to.
      type: IConfigurationSection
    - name: validateDataAnnotations
      description: Validates the bound options against their data annotations.
      type: bool
      default: 'true'
    - name: validateOnStart
      description: Runs validation during application startup instead of on first resolution.
      type: bool
      default: 'true'

returns: The same `IServiceCollection` instance with the options binding configured.
---

# AddConfiguration

Binds a strongly typed options class to a configuration section, validating it against its data annotations at startup so a missing or malformed setting stops the application with a message naming the property.

Resolve the result through `IOptions<T>` as usual. Set `validateDataAnnotations` to `false` for a settings type that is legitimately partial, and `validateOnStart` to `false` to defer validation to first resolution.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Core;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddConfiguration<MailSettings>(
    builder.Configuration.GetSection("Mail")
);
```

```csharp [MailSettings.cs]
using System.ComponentModel.DataAnnotations;

public sealed class MailSettings
{
    [Required]
    [EmailAddress]
    public required string FromAddress { get; init; }

    [Range(1, 300)]
    public int TimeoutSeconds { get; init; } = 30;
}
```

```json [appsettings.json]
{
    "Mail": {
        "FromAddress": "noreply@example.com",
        "TimeoutSeconds": 30
    }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddConfiguration<T>(
    IConfigurationSection section,
    bool validateDataAnnotations = true,
    bool validateOnStart = true
) where T : class;
```
