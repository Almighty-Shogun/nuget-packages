---
outline: deep

params:
    - name: configuration
      description: Optional application configuration used to read Serilog settings.
      type: IConfiguration?
      default: 'null'

returns: The `IHostBuilder` instance with Serilog configured as the host logger.
---

# AddCustomLogging for IHostBuilder

Adds Serilog logging through `IHostBuilder`. The method creates a Serilog logger, enriches it from the log context, writes console output through the package's custom formatter, optionally reads Serilog settings from configuration, and registers Serilog as the host logger.

Use this overload when application startup configures logging at the host level through `builder.Host`.

## Usage

```csharp
using AlmightyShogun.Logging;

builder.Host.AddCustomLogging(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IHostBuilder AddCustomLogging(
    IConfiguration? configuration = null
);
```
