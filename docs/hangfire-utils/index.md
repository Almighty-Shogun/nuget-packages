# Hangfire Utils

Adds small Hangfire helpers for registering an in-memory Hangfire server and discovering recurring jobs from attributes and a small job contract. The package is intended for applications that want recurring job metadata to live next to the job class instead of being manually repeated in startup code.

Use this package when a hosted application needs simple Hangfire setup and convention-based recurring-job discovery. Job classes implement `IRecurringJob`, are marked with `RecurringJobAttribute`, and are scheduled through Hangfire's recurring job manager.

## Categories

- [Attributes](./attributes/recurring-job-attribute) &mdash; metadata used to identify recurring job classes and cron expressions.
- [Classes](./classes/hangfire-utils/) &mdash; utilities and hosted-service integration for recurring job discovery and scheduling.
- [Extensions](./extensions/add-hangfire) &mdash; startup extension methods for registering Hangfire and recurring jobs.
- [Interfaces](./interfaces/irecurring-job/) &mdash; public contract implemented by recurring job classes.
- [Records](./records/recurring-job) &mdash; recurring job metadata returned by discovery helpers.

## Quick Example

```csharp
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddHangfire()
    .RegisterRecurringJobs(typeof(Program).Assembly)
    .AddHostedService<JobSchedulerStartup>();
```
