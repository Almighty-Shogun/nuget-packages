# RecurringJob

Contract for application-defined recurring Hangfire jobs. Implement `IRecurringJob`, add [`RecurringJobAttribute`](../attributes/recurring-job-attribute) to the class, and put the scheduled work in `RunAsync`. A class missing either the interface or the attribute is not discovered by [`RegisterRecurringJobs`](../extensions/register-recurring-jobs).

## RunAsync

The work the schedule runs. The cancellation token is signalled when Hangfire aborts the job or the server shuts down; a job with nothing to unwind can ignore the parameter, while a long-running one that never observes it holds up shutdown until Hangfire stops waiting.

The job is resolved from a fresh scope on every run, so its constructor may take scoped services such as a database context, and no field survives between runs.

```csharp
using AlmightyShogun.Hangfire.RecurringJobs;

[RecurringJob("rebuild-search-index", "0 3 * * *")]
public sealed class RebuildSearchIndexJob(
    SearchIndexer indexer
) : IRecurringJob
{
    public Task RunAsync(CancellationToken cancellationToken)
        => indexer.RebuildAsync(cancellationToken);
}
```

::: warning
Implement `RunAsync` publicly, not explicitly. Hangfire invokes it by reflection, and an explicit interface implementation is private, so the host fails at startup naming the class.
:::

### Type signature

```csharp
public Task RunAsync(CancellationToken cancellationToken);
```
