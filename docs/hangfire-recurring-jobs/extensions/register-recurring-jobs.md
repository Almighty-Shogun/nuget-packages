---
params:
    - name: assemblies
      description: Assemblies scanned for recurring job classes. Use the parameterless overload to scan the calling assembly instead; an empty array registers the scheduler with nothing to schedule.
      type: Assembly[]

returns: The `IServiceCollection` instance with recurring jobs and the startup scheduler registered.
---

# RegisterRecurringJobs

Registers the recurring job classes found in the given assemblies and the hosted service that schedules them. A class is discovered when it inherits [`RecurringJobBase`](../types/recurring-job-base) and carries [`RecurringJobAttribute`](../attributes/recurring-job-attribute). Call it after [`AddCustomHangfire`](./add-custom-hangfire).

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Hangfire.RecurringJobs;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddCustomHangfire()
    .RegisterRecurringJobs();
```

```csharp [CleanupExpiredSessionsJob.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : RecurringJobBase
{
    public override Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

:::

## Startup validation

The attribute scan runs when the host starts rather than during registration. An empty or malformed cron expression, a time zone the machine does not recognise, or two jobs sharing a job id throws `InvalidOperationException` naming the offending class, which stops the application before any schedule reaches Hangfire. A job with `Enabled` set to `false` is validated the same way, so parking a job cannot hide a broken expression.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRecurringJobs();

public IServiceCollection RegisterRecurringJobs(
    Assembly[] assemblies
);
```
