---
outline: deep

params:
    - name: configuration
      description: Optional application configuration used to read Serilog settings.
      type: IConfiguration?
      default: 'null'

returns: The `IServiceCollection` instance with Serilog logging configured.
---

# AddCustomLogging for IServiceCollection

Adds Serilog logging through `IServiceCollection`. The method creates a Serilog logger, enriches it from the log context, writes console output through the package's custom formatter, optionally reads Serilog settings from configuration, and connects the logger to `Microsoft.Extensions.Logging`.

Use this overload when application startup configures services directly and should register logging through `builder.Services`.

## Usage

```csharp
using AlmightyShogun.Logging;

builder.Services.AddCustomLogging(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCustomLogging(
    IConfiguration? configuration = null
);
```
