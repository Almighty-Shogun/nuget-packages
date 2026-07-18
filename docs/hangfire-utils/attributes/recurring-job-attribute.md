---
params:
    - name: jobId
      description: Stable Hangfire recurring job id.
      type: string

    - name: cronExpression
      description: Cron expression used by Hangfire to schedule the recurring job.
      type: string
---

# RecurringJobAttribute

Marks a class as a recurring Hangfire job. `RegisterRecurringJobs` discovers `RecurringJobBase` types with this attribute and registers them for scheduling during application startup.

Use this attribute on classes that inherit from `RecurringJobBase`. The scheduler creates the Hangfire job from the marked type and its public parameterless `RunAsync` method.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : RecurringJobBase
{
    public override Task RunAsync()
    {
        return Task.CompletedTask;
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public RecurringJobAttribute(
    string jobId,
    string cronExpression
);
```
