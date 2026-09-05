---
params:
    - name: configuration
      description: Application configuration to read the optional [`Serilog`](../configuration) section from. Omit it to run on the console sink and log-context enrichment alone.
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

Configures Serilog with log-context enrichment and the package's asynchronous colored console sink, and registers it so an injected `ILogger<T>` writes through it. The `IHostBuilder` receiver replaces the host's logging providers; the `IServiceCollection` receiver adds Serilog next to them. Both hand the logger to Serilog for disposal, which flushes the buffered console sink during an orderly [shutdown](../installation#flushing-on-shutdown).

## Usage

::: warning
A host that already registered console logging keeps writing through it under the `IServiceCollection` receiver, so every line appears twice. Use the `IHostBuilder` receiver, or clear the host's own providers, when only the Serilog output is wanted.
:::

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
