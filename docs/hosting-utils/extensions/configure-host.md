---
params:
    - name: shutdownTimeout
      description: Maximum amount of time the host should wait for graceful shutdown.
      type: TimeSpan

    - name: backgroundServiceExceptionBehavior
      description: Behavior used when a background service throws an unhandled exception.
      type: BackgroundServiceExceptionBehavior

returns: The `IServiceCollection` instance with host options configured.
---

# ConfigureHost

Configures .NET host options through dependency injection. The method sets `HostOptions.ShutdownTimeout` and `HostOptions.BackgroundServiceExceptionBehavior` from the provided values.

Use this method when a hosted application should have an explicit shutdown timeout or a consistent policy for background-service failures. For example, services that need time to flush queues or close external connections can use a longer shutdown timeout.

## Usage

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.Utils;

builder.Services.ConfigureHost(
    TimeSpan.FromSeconds(30),
    BackgroundServiceExceptionBehavior.StopHost
);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection ConfigureHost(
    TimeSpan shutdownTimeout,
    BackgroundServiceExceptionBehavior backgroundServiceExceptionBehavior
);
```
