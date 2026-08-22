# RecurringJobRegistry

The list of recurring jobs discovered by [`RegisterRecurringJobs`](../extensions/register-recurring-jobs), built once when the host starts.

Application code depends on `IRecurringJobRegistry` to report what is scheduled, on a diagnostics endpoint or a startup log line.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.Hangfire.RecurringJobs;

[ApiController]
[Route("ops/jobs")]
public sealed class JobsController(
    IRecurringJobRegistry registry
) : ControllerBase
{
    [HttpGet]
    public IActionResult List() => Ok(registry.Jobs);
}
```

::: tip
This describes the schedule, not the runs. Whether a job is executing, when it last ran, and whether it succeeded live in Hangfire storage and are read through Hangfire's own monitoring API.
:::

## Jobs

The discovered jobs as [`RecurringJobInfo`](../records/recurring-job-info) values, in discovery order. Jobs whose attribute sets `Enabled` to `false` are absent. Built once at startup, so the list is fixed for the lifetime of the application and reading it costs nothing.

### Type signature

```csharp
public IReadOnlyList<RecurringJobInfo> Jobs { get; }
```
