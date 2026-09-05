# Hangfire Recurring Jobs

Sets up Hangfire and discovers recurring jobs from attributes, so a job's schedule lives next to the job class instead of being repeated in startup code. Storage defaults to in-memory and any Hangfire provider can be selected instead.

Job classes implement [`IRecurringJob`](./types/recurring-job), carry [`RecurringJobAttribute`](./attributes/recurring-job-attribute), and are scheduled through Hangfire's recurring job manager.

## Categories

- [Configuration](./configuration) &mdash; per-environment overrides for what the attributes declare.
- [Extensions](./extensions/add-custom-hangfire) &mdash; startup extension methods for registering Hangfire and recurring jobs.
- [Attributes](./attributes/recurring-job-attribute) &mdash; metadata used to identify recurring job classes and cron expressions.
- [Services](./services/recurring-job-registry) &mdash; the registry listing what was discovered.
- [Types](./types/recurring-job) &mdash; the contract application-defined recurring jobs implement.
- [Records](./records/recurring-job-info) &mdash; the metadata for one discovered job.
- [Constants](./constants/cron-schedules) &mdash; cron expressions for the common schedules.

## Quick Example

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

[RecurringJob("cleanup-expired-sessions", CronSchedules.Daily)]
public sealed class CleanupExpiredSessionsJob(
    SessionStore sessions
) : IRecurringJob
{
    public Task RunAsync(CancellationToken cancellationToken)
        => sessions.DeleteExpiredAsync(cancellationToken);
}
```

```json [appsettings.Development.json]
{
    "RecurringJobs": {
        "Jobs": {
            "cleanup-expired-sessions": {
                "Enabled": false
            }
        }
    }
}
```

:::
