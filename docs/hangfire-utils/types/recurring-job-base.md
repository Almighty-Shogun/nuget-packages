# RecurringJobBase

Base class for application-defined recurring Hangfire jobs. A recurring job class should inherit from this type, add `RecurringJobAttribute` to the class, and implement `RunAsync` with the work that Hangfire should execute on the configured schedule.

Use this type for small background jobs that can be discovered from assemblies and scheduled through `RegisterRecurringJobs`. The package only discovers concrete, non-abstract classes that inherit from `RecurringJobBase`, are marked with `RecurringJobAttribute`, and expose the inherited public parameterless `RunAsync` method returning `Task`.

## Usage

```csharp
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

## RunAsync

Executes the recurring Hangfire job. `RegisterRecurringJobs` uses this method as the target method when it discovers and schedules recurring job classes.

Implement this method with the work that should run on the configured recurring schedule. The method must remain public, parameterless, and return `Task`, otherwise the job type is skipped during discovery.

### Type signature

```csharp
public abstract Task RunAsync();
```
