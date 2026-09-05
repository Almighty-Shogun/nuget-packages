# Formatter

[`AddCustomLogging`](./extensions/add-custom-logging) attaches this formatter to the console sink it adds, unless `includeConsoleSink` is `false`. The formatter is internal to the package, so application code never constructs it; write normal Serilog message templates and the console output comes back colored.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Serilog;
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
            "Imported {Count} rows for {Tenant:|bg} in {Elapsed:N1|y} ms",
            4218,
            "contoso",
            92.47
        );

        logger.LogWarning("Retrying after {Failures:|br} failures", 3);

        return Task.CompletedTask;
    }
}
```

:::

## Output format

Each line starts with a timestamp and a three-letter, upper-case level, colored by severity, followed by the rendered message template:

```text
[13:45:02 INF] Imported 4218 rows in 92.5 ms
```

An exception is written on the following line in dark gray.

### Level colors

| Color | Level |
| --- | --- |
| White | `Verbose` |
| White | `Debug` |
| Green | `Information` |
| Yellow | `Warning` |
| Red | `Error` |
| Bright red | `Fatal` |

### Default property colors

A property with no color code is colored by the type of its value:

| Color | Value |
| --- | --- |
| White | `string` |
| Cyan | Any numeric type, including `byte`, `short`, `int`, `long`, their unsigned forms, `nint`, `nuint`, `Half`, `float`, `double`, and `decimal` |
| Magenta | `bool` |
| Dark gray | `null` |
| White | Anything else |

## Template syntax

A color is applied by putting a shorthand code after a `|` in the property's format section:

| Template | Result |
| --- | --- |
| `{Value}` | Default color for the value's type. |
| `{Value:\|r}` | Red, no format applied. |
| `{Value:N2\|r}` | Formatted with `N2`, then colored red. |
| `{Value:N2}` | Formatted with `N2`, default color. |

### Colors

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

Codes are matched case-insensitively. An unrecognized code renders white rather than failing.

## When colors are suppressed

ANSI escape codes are omitted when the output is redirected, or when the `NO_COLOR` environment variable is set. This keeps escape sequences out of a log file or a piped stream, where they would otherwise appear as literal bytes.

Detection runs once per process. Pass `enableColors` to [`AddCustomLogging`](./extensions/add-custom-logging) to override it, for example on a CI system that renders ANSI but reports its output as redirected.
