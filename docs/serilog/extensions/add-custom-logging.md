---
params:
    - name: configuration
      description: Application configuration to read the optional `Serilog` section from. Omit it and Serilog's own defaults apply.
      type: IConfiguration?
      default: 'null'
    - name: includeConsoleSink
      description: Adds the package console sink. Set it to `false` when the application declares its own sinks in configuration, otherwise a console entry there produces a second console sink and every line appears twice.
      type: bool
      default: 'true'
    - name: enableColors
      description: Writes ANSI color codes. When `null`, colors are enabled unless output is redirected or `NO_COLOR` is set. Pass `true` to force them on for a terminal that reports its output as redirected, or `false` to force them off.
      type: bool?
      default: 'null'

returns: The same service collection or host builder instance, with Serilog configured.
---

# AddCustomLogging

Configures Serilog with log-context enrichment and the package's asynchronous colored console sink, and registers it as the `Microsoft.Extensions.Logging` provider. Inject `ILogger<T>` as usual afterwards. The logger is registered for disposal, which flushes the asynchronous sink's buffer during an orderly [shutdown](../installation#flushing-on-shutdown).

## Usage

::: code-group

```csharp [IServiceCollection.cs]
using AlmightyShogun.Serilog;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddCustomLogging(builder.Configuration);
```

```csharp [IHostBuilder.cs]
using AlmightyShogun.Serilog;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .AddCustomLogging()
    .Build();
```

```csharp [SinksFromConfiguration.cs]
using AlmightyShogun.Serilog;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddCustomLogging(
    builder.Configuration,
    includeConsoleSink: false
);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCustomLogging(
    IConfiguration? configuration = null,
    bool includeConsoleSink = true,
    bool? enableColors = null
);

public IHostBuilder AddCustomLogging(
    IConfiguration? configuration = null,
    bool includeConsoleSink = true,
    bool? enableColors = null
);
```
