---
outline: deep

params:
    - name: cancellationToken
      description: Host cancellation token provided during shutdown.
      type: CancellationToken

returns: A completed task.
---

# StopAsync

Completes the hosted-service shutdown contract. The current implementation does not perform custom cleanup because recurring job scheduling happens during startup and Hangfire server shutdown is handled by Hangfire's own hosted services.

Application code normally should not call this method directly. The .NET host calls it when the application is stopping.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services.AddHostedService<JobSchedulerStartup>();
```

<FrontmatterDocs/>

## Type signature

```csharp
public Task StopAsync(CancellationToken cancellationToken);
```
