# Formatter

The package always registers its internal console formatter when `AddCustomLogging` is used. Application code does not instantiate the formatter directly; write normal Serilog message templates and the registered logger applies the formatter to console output.

The formatter writes a timestamp, a three-letter log level, message-template text, colored property values, and exception details. Scalar values get default colors by type: strings are white, numbers are cyan, booleans are magenta, and `null` values are dark gray.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Logging;

builder.Services
    .AddCustomLogging(builder.Configuration)
    .AddHostedService<ImportWorker>();
```

```csharp [ImportWorker.cs]
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class ImportWorker(ILogger<ImportWorker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Processed {Count:c} items for {Application:bg} in {Elapsed:0.00|y} ms",
            42,
            "admin",
            18.742
        );

        logger.LogInformation("User {UserId:y} logged in", 42);
        logger.LogInformation("Completed in {Elapsed:0.00|g} ms", 18.742);

        return Task.CompletedTask;
    }
}
```

:::

Message-template property formats can include color shorthand. Use the shorthand as the property format, or combine a numeric format and color with `|`.

## Colors

| Code | Color |
| --- | --- |
| `r` | Red |
| `g` | Green |
| `b` | Blue |
| `c` | Cyan |
| `y` | Yellow |
| `m` | Magenta |
| `br` | Bright red |
| `bg` | Bright green |
| `bb` | Bright blue |
| `bc` | Bright cyan |
| `by` | Bright yellow |
| `bm` | Bright magenta |
