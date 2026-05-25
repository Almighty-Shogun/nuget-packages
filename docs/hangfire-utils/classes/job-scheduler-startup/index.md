# JobSchedulerStartup

Hosted service that schedules registered recurring jobs during application startup. The class receives `RecurringJob` metadata from dependency injection and adds each job to Hangfire through `IRecurringJobManager`.

Register this class as an `IHostedService` after `RegisterRecurringJobs` when the application should schedule recurring jobs automatically during host startup. Application code normally does not instantiate or call it manually; the .NET host calls `StartAsync` and `StopAsync`.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .RegisterRecurringJobs(typeof(Program).Assembly)
    .AddHostedService<JobSchedulerStartup>();
```

## Methods

- [StartAsync](./start-async) &mdash; schedules discovered recurring jobs.
- [StopAsync](./stop-async) &mdash; completes the hosted-service shutdown contract.
