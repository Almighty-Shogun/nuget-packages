# Logging

Adds Serilog-based logging registration with a compact custom console formatter. The package configures log context enrichment, writes console output through an asynchronous Serilog sink, and can optionally read additional Serilog settings from application configuration.

Use this package when an application should use the same console logging format across service-collection based and host-builder based startup code. The custom formatter is internal; application code configures it through the public `AddCustomLogging` extension methods.

## Categories

- [Configuration](./configuration) &mdash; optional Serilog configuration read from application settings.
- [Formatter](./formatter) &mdash; console formatter behavior, message-template color syntax, and available colors.
- [Extensions](./extensions/add-custom-logging) &mdash; logging registration methods for `IServiceCollection` and `IHostBuilder`.

## Quick Example

```csharp
using AlmightyShogun.Logging;

builder.Services.AddCustomLogging(builder.Configuration);
```
