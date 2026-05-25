# HangfireUtils

Provides discovery helpers for recurring Hangfire jobs. The public method scans loaded application assemblies for concrete `IRecurringJob` implementations marked with `RecurringJobAttribute` and converts them into `RecurringJob` metadata.

Use `HangfireUtils` when application code needs to inspect which recurring jobs are available, build diagnostics, or schedule jobs through custom startup logic. A discovered job must have a public parameterless `RunAsync` method that returns `Task`.

## Usage

```csharp
using AlmightyShogun.Hangfire.Utils;

IEnumerable<RecurringJob> jobs = HangfireUtils.GetRecurringJobs();
```

## Methods

- [GetRecurringJobs](./get-recurring-jobs) &mdash; discovers recurring job metadata from loaded assemblies.
