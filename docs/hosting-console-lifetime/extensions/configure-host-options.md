---
params:
    - name: shutdownTimeout
      description: Maximum time allowed for a graceful host shutdown before the host stops waiting.
      type: TimeSpan
    - name: backgroundServiceExceptionBehavior
      description: How the host responds when a background service throws an unhandled exception.
      type: BackgroundServiceExceptionBehavior

returns: The same builder or service collection instance, with the host options configured.
---

# ConfigureHostOptions

Sets the two `HostOptions` values that matter most to a long-running service: how long shutdown may take, and what happens when a background service throws.

`shutdownTimeout` bounds the whole sequence rather than each service, so set it above the slowest `StopAsync` and below the supervisor's grace period, which is 10 seconds for `docker stop` and 30 for Kubernetes.

## Usage

::: code-group

```csharp [IHostApplicationBuilder.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.ConfigureHostOptions(
    TimeSpan.FromSeconds(30),
    BackgroundServiceExceptionBehavior.StopHost
);
```

```csharp [IHostBuilder.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureHostOptions(
        TimeSpan.FromSeconds(30),
        BackgroundServiceExceptionBehavior.StopHost
    )
    .Build();
```

```csharp [IServiceCollection.cs]
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;
using Microsoft.Extensions.DependencyInjection;

builder.Services.ConfigureHostOptions(
    TimeSpan.FromSeconds(30),
    BackgroundServiceExceptionBehavior.StopHost
);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection ConfigureHostOptions(
    TimeSpan shutdownTimeout,
    BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
);

public IHostApplicationBuilder ConfigureHostOptions(
    TimeSpan shutdownTimeout,
    BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
);

public IHostBuilder ConfigureHostOptions(
    TimeSpan shutdownTimeout,
    BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
);
```
