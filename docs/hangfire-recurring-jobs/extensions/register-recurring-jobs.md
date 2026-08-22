---
params:
    - name: assemblies
      description: Assemblies scanned for recurring job classes. Use the overload without it to scan the calling assembly instead; an empty array registers the scheduler with nothing to schedule.
      type: Assembly[]

    - name: configuration
      description: Read for a `RecurringJobs` section overriding what the attributes declare. Omit it when every environment schedules the same jobs the same way.
      type: IConfiguration?
      default: 'null'

returns: The `IServiceCollection` instance with recurring jobs and the startup scheduler registered.
---

# RegisterRecurringJobs

Registers the recurring job classes found in the given assemblies and the hosted service that schedules them. A class is discovered when it implements [`IRecurringJob`](../types/recurring-job) and carries [`RecurringJobAttribute`](../attributes/recurring-job-attribute). Pass an `IConfiguration` to let the [`RecurringJobs`](../configuration) section override what the attributes declare. Call it after [`AddCustomHangfire`](./add-custom-hangfire).

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Hangfire.RecurringJobs;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddCustomHangfire()
    .RegisterRecurringJobs(builder.Configuration);
```

```csharp [CleanupExpiredSessionsJob.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : IRecurringJob
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

:::

## Startup validation

The attribute scan runs when the host starts rather than during registration. An empty or malformed cron expression, a time zone the machine does not recognise, two jobs sharing a job id, or an override naming a job id nothing declares throws `InvalidOperationException` naming what is wrong, which stops the application before any schedule reaches Hangfire. Overridden values are checked exactly like declared ones. A job with `Enabled` set to `false` is checked the same way and still claims its job id, so parking a job cannot hide a broken expression or a collision until someone enables it again.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRecurringJobs(
    IConfiguration? configuration = null
);

public IServiceCollection RegisterRecurringJobs(
    Assembly[] assemblies,
    IConfiguration? configuration = null
);
```
