---
outline: deep

params:
    - name: cancellationToken
      description: Host cancellation token provided during startup.
      type: CancellationToken

returns: A completed task after discovered recurring jobs have been scheduled.
---

# StartAsync

Schedules all registered recurring job metadata values. For each `RecurringJob`, the method calls Hangfire's recurring job manager with the job name, execution method, and cron expression.

Application code normally should not call this method directly. Register recurring jobs first, then register `JobSchedulerStartup` as a hosted service and let the .NET host call it during application startup.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .RegisterRecurringJobs(typeof(Program).Assembly)
    .AddHostedService<JobSchedulerStartup>();
```

<FrontmatterDocs/>

## Type signature

```csharp
public Task StartAsync(CancellationToken cancellationToken);
```

## Uses

- [RecurringJob](../../records/recurring-job)
