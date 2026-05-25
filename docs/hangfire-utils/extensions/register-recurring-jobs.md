---
outline: deep

params:
    - name: assemblies
      description: Assemblies used when registering recurring job-related services. When omitted, the calling assembly is used.
      type: Assembly[]
      default: '[]'

returns: The `IServiceCollection` instance with recurring job registrations applied.
---

# RegisterRecurringJobs

Registers recurring job services and metadata from one or more assemblies. When no assembly is provided, the method uses the calling assembly.

Use this method after `AddHangfire` and pass the assembly that contains recurring job classes. Each discovered job class must implement `IRecurringJob`, be marked with `RecurringJobAttribute`, and expose a public parameterless `RunAsync` method that returns `Task`. The method registers matching job classes as scoped services and stores their `RecurringJob` metadata for `JobSchedulerStartup`.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddHangfire()
    .RegisterRecurringJobs(typeof(Program).Assembly)
    .AddHostedService<JobSchedulerStartup>();
```

```csharp [CleanupExpiredSessionsJob.cs]
using AlmightyShogun.Hangfire.Utils;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : IRecurringJob
{
    public Task RunAsync()
    {
        return Task.CompletedTask;
    }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRecurringJobs(
    params Assembly[] assemblies
);
```

## Uses

- [IRecurringJob](../interfaces/irecurring-job/)
- [RecurringJob](../records/recurring-job)
- [RecurringJobAttribute](../attributes/recurring-job-attribute)
