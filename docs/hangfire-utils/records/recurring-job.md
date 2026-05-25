---
outline: deep

returns: Recurring Hangfire job metadata containing the job id, cron expression, and execution descriptor.
---

# RecurringJob

Represents recurring job metadata discovered from an `IRecurringJob` class marked with `RecurringJobAttribute`. The record contains the Hangfire recurring job id, cron expression, and `Job` object used when scheduling through `IRecurringJobManager`.

Application code usually receives these values from `HangfireUtils.GetRecurringJobs` or from dependency injection after `RegisterRecurringJobs`, rather than creating them manually.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;

IEnumerable<RecurringJob> jobs = HangfireUtils.GetRecurringJobs();

foreach (RecurringJob job in jobs)
{
    Console.WriteLine($"{job.Name}: {job.CronExpression}");
}
```

<FrontmatterDocs/>
