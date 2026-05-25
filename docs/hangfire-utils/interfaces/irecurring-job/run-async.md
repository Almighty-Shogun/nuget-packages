---
outline: deep

returns: A task that represents the asynchronous recurring job execution.
---

# RunAsync

Executes a recurring Hangfire job. Hangfire Utils uses this method as the target method when it creates `RecurringJob` metadata for a discovered job class.

Implement this method with the work that should run on the configured recurring schedule. The method must be public, parameterless, and return `Task`; otherwise the job type is skipped during discovery.

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

<FrontmatterDocs/>

## Type signature

```csharp
public Task RunAsync();
```
