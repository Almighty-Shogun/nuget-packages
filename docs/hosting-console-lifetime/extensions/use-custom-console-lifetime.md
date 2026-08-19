---
returns: The same builder or service collection instance, with the custom `IHostLifetime` registered.
---

# UseCustomConsoleLifetime

Replaces the host's console lifetime so that `Ctrl+C` does not stop the application, while `SIGTERM` still triggers an orderly shutdown. Use it for a service that must not be killed by a stray key press in a shared terminal, but that still has to stop cleanly when an orchestrator asks it to. The replacement is unconditional, so ordering relative to `Host.CreateApplicationBuilder` does not matter.

## Usage

::: code-group

```csharp [IHostApplicationBuilder.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.UseCustomConsoleLifetime();
```

```csharp [IHostBuilder.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;

IHost host = Host.CreateDefaultBuilder(args)
    .UseCustomConsoleLifetime()
    .Build();
```

```csharp [IServiceCollection.cs]
using AlmightyShogun.Hosting.ConsoleLifetime;
using Microsoft.Extensions.DependencyInjection;

builder.Services.UseCustomConsoleLifetime();
```

:::

::: warning
There is no way to stop the application from the keyboard once this is registered outside an IDE. Stop it with `docker stop`, `systemctl stop`, or `kill`, all of which send `SIGTERM`, rather than `Ctrl+C`.
:::

## Shutdown behavior

| Signal | Source | Behavior |
|---|---|---|
| Ctrl+C | interactive terminal | Suppressed. The application keeps running. |
| Ctrl+C | IDE run, with `DOTNET_RUNNING_IN_IDE` set | Allowed, so the IDE stop button works. |
| SIGTERM | `docker stop`, Kubernetes, systemd | Orderly shutdown through `IHostApplicationLifetime.StopApplication`. `StopAsync` runs on every hosted service, within `ShutdownTimeout` from [`ConfigureHostOptions`](./configure-host-options), and registered services are disposed. |

`SIGTERM` is honored on Linux and macOS. Windows has no `SIGTERM`, so only the `Ctrl+C` behavior applies there.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection UseCustomConsoleLifetime();

public IHostApplicationBuilder UseCustomConsoleLifetime();

public IHostBuilder UseCustomConsoleLifetime();
```
