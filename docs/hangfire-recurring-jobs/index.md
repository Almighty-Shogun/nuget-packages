# Hangfire Recurring Jobs

Registers an in-memory Hangfire server and discovers recurring jobs from attributes, so a job's schedule lives next to the job class instead of being repeated in startup code.

Job classes inherit from [`RecurringJobBase`](./types/recurring-job-base), carry [`RecurringJobAttribute`](./attributes/recurring-job-attribute), and are scheduled through Hangfire's recurring job manager.

## Categories

- [Extensions](./extensions/add-custom-hangfire) &mdash; startup extension methods for registering Hangfire and recurring jobs.
- [Attributes](./attributes/recurring-job-attribute) &mdash; metadata used to identify recurring job classes and cron expressions.
- [Services](./services/recurring-job-registry) &mdash; the registry listing what was discovered.
- [Types](./types/recurring-job-base) &mdash; base class used by application-defined recurring jobs.
- [Records](./records/recurring-job-info) &mdash; the metadata for one discovered job.
- [Constants](./constants/cron-schedules) &mdash; cron expressions for the common schedules.

## Quick Example

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddCustomHangfire()
    .RegisterRecurringJobs();
```
