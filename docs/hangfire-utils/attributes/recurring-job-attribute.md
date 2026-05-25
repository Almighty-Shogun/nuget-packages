---
outline: deep

params:
    - name: jobId
      description: Stable Hangfire recurring job id.
      type: string

    - name: cronExpression
      description: Cron expression used by Hangfire to schedule the recurring job.
      type: string
---

# RecurringJobAttribute

Marks a class as a recurring Hangfire job. `HangfireUtils.GetRecurringJobs` reads this attribute from discovered `IRecurringJob` types and converts it into `RecurringJob` metadata that can be scheduled by `JobSchedulerStartup`.

Use this attribute on classes that implement `IRecurringJob`. The scheduler creates the Hangfire job from the marked type and its public parameterless `RunAsync` method.

## Usage

```csharp
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

<FrontmatterDocs/>

## Uses

- [IRecurringJob](../interfaces/irecurring-job/)
