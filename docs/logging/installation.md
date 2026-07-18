# Installation

Install `AlmightyShogun.Logging` in the application that should register Serilog with the package's custom console formatter. The package targets `net10.0` and supports both `IServiceCollection` and `IHostBuilder` registration styles.

```sh
dotnet add package AlmightyShogun.Logging
```

## Dependencies

- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.10` &mdash; provides service collection logging registration APIs.
- `Microsoft.Extensions.Hosting` `10.0.10` &mdash; provides host-builder integration.
- `Serilog` `4.4.0` &mdash; provides the core logger configuration and logger types.
- `Serilog.AspNetCore` `10.0.0` &mdash; provides Serilog integration for hosted and ASP.NET Core applications.
- `Serilog.Extensions.Logging` `10.0.0` &mdash; connects Serilog to `Microsoft.Extensions.Logging`.
- `Serilog.Settings.Configuration` `10.0.1` &mdash; reads Serilog configuration from `IConfiguration`.
- `Serilog.Sinks.Async` `2.1.0` &mdash; writes log events through an asynchronous sink wrapper.
- `Serilog.Sinks.Console` `6.1.1` &mdash; writes formatted log output to the console.

## Startup Registration

Register logging through services when startup code configures logging from `IServiceCollection`, or through the `IHostBuilder` when the application configures Serilog at the host level.

::: code-group

```csharp [IServiceCollection.cs]
using AlmightyShogun.Logging;

builder.Services.AddCustomLogging(builder.Configuration);
```

```csharp [IHostBuilder.cs]
using AlmightyShogun.Logging;

builder.Host.AddCustomLogging(builder.Configuration);
```

:::
