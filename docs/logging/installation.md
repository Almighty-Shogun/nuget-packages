# Installation

Install `AlmightyShogun.Logging` in the application that owns logging setup. The package targets `net10.0` and brings Serilog with it, so the application does not need to reference Serilog directly.

```sh
dotnet add package AlmightyShogun.Logging
```

## Dependencies

### Package references

- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; provides the service collection APIs used by the extension methods.
- `Microsoft.Extensions.Hosting` `10.0.11` &mdash; provides the host builder contract for the `IHostBuilder` overload.
- `Serilog` `4.4.0` &mdash; the logging pipeline the package configures.
- `Serilog.AspNetCore` `10.0.0` &mdash; provides the `UseSerilog` host builder integration.
- `Serilog.Extensions.Logging` `10.0.0` &mdash; bridges Serilog to `Microsoft.Extensions.Logging`.
- `Serilog.Settings.Configuration` `10.0.1` &mdash; reads the optional `Serilog` configuration section.
- `Serilog.Sinks.Async` `2.1.0` &mdash; buffers writes off the logging thread.
- `Serilog.Sinks.Console` `6.1.1` &mdash; writes formatted output to the console.

## Startup Registration

Call the extension method once during startup. Both overloads take the same optional arguments.

::: code-group

```csharp [IServiceCollection.cs]
using AlmightyShogun.Logging;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddCustomLogging(builder.Configuration);
```

```csharp [IHostBuilder.cs]
using AlmightyShogun.Logging;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .AddCustomLogging()
    .Build();
```

:::

## Flushing on shutdown

The console sink is asynchronous, so log events are buffered and written on a background thread. The package registers the logger for disposal, which flushes that buffer, but disposal only happens during an orderly shutdown.

An application that exits by killing the process, or that suppresses shutdown signals, can still lose the last buffered events. If the host also uses `AlmightyShogun.Hosting.Utils`, [`UseCustomConsoleLifetime`](/hosting-utils/extensions/use-custom-console-lifetime) makes `SIGTERM` shut down cleanly, which is what allows the flush to run.
