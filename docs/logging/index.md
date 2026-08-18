# Logging

Configures Serilog with a console formatter that colors log output by level and by property. Property values can also carry a color code in the message template, so the parts of a line that matter can be made to stand out without string concatenation.

Registration is a single call, and configuration is optional: without an `IConfiguration` the package adds log-context enrichment and its asynchronous console sink, leaving everything else to Serilog's defaults.

## Categories

- [Configuration](./configuration) &mdash; optional `Serilog` settings read from `appsettings.json`.
- [Formatter](./formatter) &mdash; the console output format, color codes, and template syntax.
- [Extensions](./extensions/add-custom-logging) &mdash; registration for service collections and host builders.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Logging;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddCustomLogging(builder.Configuration);
```

```csharp [ImportWorker.cs]
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class ImportWorker(
    ILogger<ImportWorker> logger
) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Imported {Count} rows in {Elapsed:N1|bg} ms",
            4218,
            92.47
        );

        return Task.CompletedTask;
    }
}
```

:::
