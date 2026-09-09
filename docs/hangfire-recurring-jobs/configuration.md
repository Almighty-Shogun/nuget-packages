---
fields:
    - name: RecurringJobSettings
      description: The `RecurringJobs` section itself. Every value has a default, so the section may be absent and an application that schedules the same jobs everywhere never needs it.
      fields:
          - name: EnabledByDefault
            description: Whether a job whose attribute does not state either way is scheduled. Set it to `false` in one environment to park everything except the jobs that opt in explicitly.
            type: bool
            default: 'true'

          - name: RemoveParkedJobs
            description: Whether the schedules stored under the job ids this application parked are deleted when the host starts. Set it to `false` where the storage is shared with another instance, role, or application, so parking a job stops this application scheduling it without deleting the entry another writer owns.
            type: bool
            default: 'true'

          - name: Jobs
            description: Per-job overrides keyed by job id, matched ignoring case. A key naming a job the scan did not find stops the host, since that is nearly always a typo.
            type: 'IReadOnlyDictionary<string, RecurringJobOverride>'
            default: '{}'

    - name: RecurringJobOverride
      description: One entry under `Jobs`, replacing what a single job's attribute declares. Every value is optional and an omitted one keeps the declared one, so an entry only names what changes.
      fields:
          - name: Enabled
            description: Whether the job is scheduled, outranking both the attribute and `EnabledByDefault`. Setting it to `false` also clears whatever schedule is already stored under that job id, unless `RemoveParkedJobs` is `false`.
            type: bool?
            default: 'null'

          - name: CronExpression
            description: Cron expression to schedule with instead of the declared one. Validated exactly like a declared expression, so a bad value here stops the host.
            type: string?
            default: 'null'

          - name: TimeZone
            description: Time zone to evaluate the expression in instead of the declared one. It cannot be cleared back to UTC, because an omitted value means the attribute wins.
            type: string?
            default: 'null'

          - name: Queue
            description: Queue to enqueue on instead of the declared one, for steering a job onto a queue only some environments have a server listening on.
            type: string?
            default: 'null'
---

# Configuration

The optional `RecurringJobs` section adjusts what the attribute scan found, so a job can be scheduled differently per environment without a rebuild. It is read only when [`RegisterRecurringJobs`](./extensions/register-recurring-jobs) is passed an `IConfiguration`.

```json
{
    "RecurringJobs": {
        "EnabledByDefault": true,
        "RemoveParkedJobs": true,
        "Jobs": {
            "cleanup-expired-sessions": {
                "Enabled": false,
                "CronExpression": "0 4 * * *",
                "TimeZone": "Europe/Amsterdam",
                "Queue": "maintenance"
            }
        }
    }
}
```

::: warning
A job that resolves to disabled is unscheduled when the host starts, so a durable store is left with no entry and no next execution time for it. The deletion goes by job id against Hangfire storage, which belongs to everything pointed at that storage rather than to the instance doing the deleting: a client role registering the same jobs with `AddCustomHangfire(addServer: false)`, an older instance still running through a rolling deploy, another application, and any code that called `RecurringJob.AddOrUpdate` under a parked id all lose that entry. Removal is limited to the ids the scan found and parked, which leaves the entry of a renamed or deleted job alone, and setting `RemoveParkedJobs` to `false` leaves every parked id alone, so parking a job stops this application scheduling it without deleting what another writer scheduled.
:::

<FrontmatterDocs/>
