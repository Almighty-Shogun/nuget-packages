# RecurringJobSettings

The bound `RecurringJobs` section. Read it to see what the application resolved, such as on a diagnostics endpoint that reports why a job is not running in this environment.

## Usage

```csharp
using Microsoft.Extensions.Options;
using AlmightyShogun.Hangfire.RecurringJobs;

public sealed class JobPolicyService(
    IOptions<RecurringJobSettings> settings
)
{
    public bool IsOverridden(string jobId)
        => settings.Value.Jobs.ContainsKey(jobId);
}
```
