# AddCustomLogging

Registers Serilog with the package's compact console formatter. The package exposes two overloads with the same method name: one extends `IServiceCollection`, and one extends `IHostBuilder`.

Both overloads create a Serilog logger with log-context enrichment and an asynchronous console sink that uses the package formatter. When configuration is provided, the logger also reads supported Serilog settings from `IConfiguration`.

## ServiceCollection

Adds Serilog logging through `IServiceCollection`. Use this overload when application startup configures logging from `builder.Services` and the logger should be connected through `Microsoft.Extensions.Logging`.

```csharp
using AlmightyShogun.Logging;

builder.Services.AddCustomLogging(builder.Configuration);
```

### Type signature

```csharp
public IServiceCollection AddCustomLogging(
    IConfiguration? configuration = null
);
```

## HostBuilder

Adds Serilog logging through `IHostBuilder`. Use this overload when application startup configures logging at the host level through `builder.Host` and should register Serilog as the host logger.

```csharp
using AlmightyShogun.Logging;

builder.Host.AddCustomLogging(builder.Configuration);
```

### Type signature

```csharp
public IHostBuilder AddCustomLogging(
    IConfiguration? configuration = null
);
```
