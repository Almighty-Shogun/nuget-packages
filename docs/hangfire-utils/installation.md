# Installation

Install `AlmightyShogun.Hangfire.Utils` in the application that should configure Hangfire and schedule recurring jobs from attributes. The package targets `net10.0`, registers Hangfire with in-memory storage, and discovers job classes that inherit from `RecurringJobBase`.

```sh
dotnet add package AlmightyShogun.Hangfire.Utils
```

## Dependencies

- `Hangfire` `1.8.24` &mdash; provides the background job server, recurring job manager, and job metadata types.
- `Hangfire.InMemory` `1.0.0` &mdash; provides the in-memory Hangfire storage used by `AddHangfire`.
- `Newtonsoft.Json` `13.0.4` &mdash; dependency used by Hangfire serialization.
- `AlmightyShogun.Utils` project reference &mdash; provides assembly scanning and inherited-type registration helpers.

## Startup Registration

Register Hangfire first, then register recurring job classes from the assemblies that contain them. `RegisterRecurringJobs` also registers the hosted scheduler that applies the discovered recurring jobs during startup.

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Hangfire.Utils;
using Microsoft.Extensions.DependencyInjection;

builder.Services
    .AddHangfire()
    .RegisterRecurringJobs(typeof(Program).Assembly);
```

```csharp [CleanupExpiredSessionsJob.cs]
using AlmightyShogun.Hangfire.Utils;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : RecurringJobBase
{
    public override Task RunAsync()
    {
        return Task.CompletedTask;
    }
}
```

:::
