---
outline: deep

returns: A sequence of discovered `RecurringJob` metadata values.
---

# GetRecurringJobs

Scans loaded assemblies for concrete `IRecurringJob` implementations marked with `RecurringJobAttribute` and converts them into `RecurringJob` metadata. Each discovered job uses the attribute job id, the attribute cron expression, and a Hangfire `Job` pointing at the class `RunAsync` method.

Use this method when building job diagnostics or when manually scheduling discovered jobs. The method expects recurring job classes to be loaded in the current application domain and to expose a public parameterless `RunAsync` method that returns `Task`.

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

## Type signature

```csharp
public static IEnumerable<RecurringJob> GetRecurringJobs();
```

## Uses

- [IRecurringJob](../../interfaces/irecurring-job/)
- [RecurringJob](../../records/recurring-job)
- [RecurringJobAttribute](../../attributes/recurring-job-attribute)
