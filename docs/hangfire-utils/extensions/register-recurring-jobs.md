---
params:
    - name: assemblies
      description: Assemblies used when registering recurring job-related services. When omitted, the calling assembly is used.
      type: Assembly[]
      default: '[]'

returns: The `IServiceCollection` instance with recurring job registrations applied.
---

# RegisterRecurringJobs

Registers recurring job services and metadata from one or more assemblies. When no assembly is provided, the method uses the calling assembly.

Use this method after `AddHangfire` and pass the assembly that contains recurring job classes. Each discovered job class must inherit from `RecurringJobBase`, be marked with `RecurringJobAttribute`, and expose a public parameterless `RunAsync` method that returns `Task`. The method registers matching job classes as scoped services and registers the package scheduler hosted service that applies the discovered jobs during startup.

## Usage

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

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection RegisterRecurringJobs(
    params Assembly[] assemblies
);
```
