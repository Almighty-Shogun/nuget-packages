# IRecurringJob

Defines the public contract for recurring Hangfire job classes discovered by Hangfire Utils. A recurring job class must implement this interface and be marked with `RecurringJobAttribute` before `RegisterRecurringJobs` will register it.

Use this interface on small job classes whose execution entry point is `RunAsync`. The package validates that the method is public, parameterless, and returns `Task`, then uses that method when building Hangfire job metadata.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : IRecurringJob
{
    public Task RunAsync()
    {
        return Task.CompletedTask;
    }
}
```

## Methods

- [RunAsync](./run-async) &mdash; executes the recurring job when Hangfire runs it.
