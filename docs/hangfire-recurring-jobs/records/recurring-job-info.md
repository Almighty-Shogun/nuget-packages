---
fields:
    - name: JobId
      description: Identifier the job is registered under, taken from its `RecurringJobAttribute` and unique across the application.
      type: string
    - name: CronExpression
      description: The schedule, validated when the host starts rather than on first run.
      type: string
    - name: JobType
      description: The discovered `IRecurringJob` implementation, resolved from a fresh scope on every run.
      type: Type
    - name: TimeZone
      description: Time zone identifier the schedule is evaluated in. Null means UTC.
      type: string?
    - name: Queue
      description: Hangfire queue the job is enqueued on. Null means the default queue.
      type: string?
---

# RecurringJobInfo

One discovered recurring job, as read from its attribute when the host starts. Exposed through [`RecurringJobRegistry`](../services/recurring-job-registry) so an application can list what is scheduled.

## Usage

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;

public sealed class JobReporter(IRecurringJobRegistry registry)
{
    public IEnumerable<string> Describe() => registry.Jobs
        .Select(job => $"{job.JobId}: {job.CronExpression}");
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record RecurringJobInfo(
    string JobId,
    string CronExpression,
    Type JobType,
    string? TimeZone,
    string? Queue
);
```
