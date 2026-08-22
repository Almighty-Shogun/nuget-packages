---
params:
    - name: jobId
      description: Identifier the schedule is stored under, unique across the application. Changing it on a job that has already run leaves the old schedule in storage under the old id.
      type: string

    - name: cronExpression
      description: Cron expression the schedule uses. It is parsed while the host starts, so a malformed expression stops the application rather than leaving a job that never fires.
      type: string

    - name: TimeZone
      description: Time zone identifier the cron expression is evaluated in, such as `Europe/Amsterdam`. Without it the schedule is UTC, so "run at 3am" drifts by an hour under daylight saving.
      type: string?
      default: 'null'

    - name: Queue
      description: Hangfire queue the job is enqueued on. Omit it for the default queue; naming one no server listens on leaves the job enqueued and never processed.
      type: string?
      default: 'null'

    - name: Enabled
      description: Whether the job is scheduled at all. Set it to `false` to park a job without deleting the class; its cron expression and time zone are still validated.
      type: bool
      default: 'true'
---

# RecurringJobAttribute

Marks a class as a recurring Hangfire job. [`RegisterRecurringJobs`](../extensions/register-recurring-jobs) discovers [`RecurringJobBase`](../types/recurring-job-base) types carrying it and schedules each one against its [`RunAsync`](../types/recurring-job-base#runasync) method. It does nothing on a class outside that hierarchy, since the scan never looks at one.

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

<FrontmatterDocs/>

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class RecurringJobAttribute(
    string jobId,
    string cronExpression
) : Attribute;
```
