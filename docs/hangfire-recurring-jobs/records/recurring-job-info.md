---
fields:
    - name: JobId
      description: Identifier the job is registered under, taken from its `RecurringJobAttribute` and unique across the application.
      type: string
    - name: CronExpression
      description: The schedule, validated when the host starts rather than on first run.
      type: string
    - name: JobType
      description: The discovered `IRecurringJob` implementation, resolved from a fresh scope on every run.
      type: Type
    - name: TimeZone
      description: Time zone identifier the schedule is evaluated in. Null means UTC.
      type: string?
    - name: Queue
      description: Hangfire queue the job is enqueued on. Null means the default queue.
      type: string?
---

# RecurringJobInfo

One discovered recurring job, as its attribute and any matching configuration override resolve together. Only the job id always comes from the attribute. Exposed through [`RecurringJobRegistry`](../services/recurring-job-registry) so an application can list what is scheduled.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record RecurringJobInfo
{
    public required string JobId { get; init; }
    public required string CronExpression { get; init; }
    public required Type JobType { get; init; }
    public required string? TimeZone { get; init; }
    public required string? Queue { get; init; }
}
```
