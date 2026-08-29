# Installation

Install `AlmightyShogun.Hangfire.RecurringJobs` in the application that should configure Hangfire and schedule recurring jobs from attributes. The package targets `net10.0`, sets Hangfire up with in-memory storage unless another provider is selected, and discovers job classes that implement [`IRecurringJob`](./types/recurring-job).

```sh
dotnet add package AlmightyShogun.Hangfire.RecurringJobs
```

## Dependencies

### Package references

- `Hangfire` `1.8.24` &mdash; provides the background job server, recurring job manager, and job metadata types.
- `Hangfire.InMemory` `1.0.0` &mdash; backs the default storage [`AddCustomHangfire`](./extensions/add-custom-hangfire) selects. It stays referenced even when another provider is used, which the consuming project's dependency graph shows.
- `Newtonsoft.Json` `13.0.4` &mdash; Hangfire's serializer, which arrives transitively and shows up in the consuming project's dependency graph.
- `Cronos` `0.13.0` &mdash; validates cron expressions on recurring-job attributes when the host starts.

### Project references

- `AlmightyShogun.Utils` &mdash; provides assembly scanning and inherited-type registration helpers.

## Startup Registration

[`AddCustomHangfire`](./extensions/add-custom-hangfire) sets up Hangfire with a processing server, using in-memory storage unless a configuration delegate selects another provider. [`RegisterRecurringJobs`](./extensions/register-recurring-jobs) scans for job classes and adds the hosted service that puts their schedules into Hangfire when the host starts.

::: warning
`AddCustomHangfire` must come first. `RegisterRecurringJobs` schedules through the recurring job manager that call registers.
:::

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Hangfire.RecurringJobs;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddCustomHangfire()
    .RegisterRecurringJobs();
```

```csharp [CleanupExpiredSessionsJob.cs]
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : IRecurringJob
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

:::
