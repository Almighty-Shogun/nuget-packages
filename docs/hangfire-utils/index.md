# Hangfire Utils

Adds small Hangfire helpers for registering an in-memory Hangfire server and discovering recurring jobs from attributes and a base job type. The package is intended for applications that want recurring job metadata to live next to the job class instead of being manually repeated in startup code.

Use this package when a hosted application needs simple Hangfire setup and convention-based recurring-job discovery. Job classes inherit from [`RecurringJobBase`](./types/recurring-job-base), are marked with [`RecurringJobAttribute`](./attributes/recurring-job-attribute), and are scheduled through Hangfire's recurring job manager.

## Categories

- [Extensions](./extensions/add-hangfire) &mdash; startup extension methods for registering Hangfire and recurring jobs.
- [Attributes](./attributes/recurring-job-attribute) &mdash; metadata used to identify recurring job classes and cron expressions.
- [Types](./types/recurring-job-base) &mdash; base class used by application-defined recurring jobs.

## Quick Example

```csharp
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddHangfire()
    .RegisterRecurringJobs(typeof(Program).Assembly);
```
