# RecurringJobBase

Base class for application-defined recurring Hangfire jobs. Inherit from it, add [`RecurringJobAttribute`](../attributes/recurring-job-attribute) to the class, and implement `RunAsync` with the work Hangfire runs on the schedule. A class missing either the base type or the attribute is not discovered by [`RegisterRecurringJobs`](../extensions/register-recurring-jobs).

## Usage

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("cleanup-expired-sessions", "0 */6 * * *")]
public sealed class CleanupExpiredSessionsJob : RecurringJobBase
{
    public override Task RunAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

::: tip
The job is resolved from a fresh scope on every run, so its constructor may take scoped services such as a database context, and no field survives between runs.
:::

## RunAsync

The work the schedule runs. The cancellation token is signalled when Hangfire aborts the job or the server shuts down; a job with nothing to unwind can ignore the parameter, while a long-running one that never observes it holds up shutdown until Hangfire stops waiting.

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("rebuild-search-index", "0 3 * * *")]
public sealed class RebuildSearchIndexJob(
    SearchIndexer indexer
) : RecurringJobBase
{
    public override Task RunAsync(CancellationToken cancellationToken)
        => indexer.RebuildAsync(cancellationToken);
}
```

### Type signature

```csharp
public abstract Task RunAsync(CancellationToken cancellationToken);
```
