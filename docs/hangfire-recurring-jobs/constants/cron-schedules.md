# CronSchedules

Cron expressions for the common schedules, so a recurring job can avoid a raw string that is easy to get subtly wrong.

## Usage

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("nightly-cleanup", CronSchedules.Daily, "Europe/Amsterdam")]
public sealed class NightlyCleanupJob : RecurringJobBase
{
    public override Task RunAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
```

::: tip
Every expression here is UTC unless the job sets `TimeZone`, so `Daily` means midnight UTC. Set `TimeZone` when a schedule is meant to track local time across daylight saving.
:::

## Type signature

```csharp
public static class CronSchedules
{
    public const string Minutely = "* * * * *";
    public const string Hourly = "0 * * * *";
    public const string Daily = "0 0 * * *";
    public const string Weekly = "0 0 * * 1";
    public const string Monthly = "0 0 1 * *";
    public const string Yearly = "0 0 1 1 *";
}
```
