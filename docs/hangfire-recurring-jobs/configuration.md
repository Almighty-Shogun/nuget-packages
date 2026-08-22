---
fields:
    - name: RecurringJobs
      description: The `RecurringJobs` section itself. Every value has a default, so the section may be absent and an application that schedules the same jobs everywhere never needs it.
      fields:
          - name: EnabledByDefault
            description: Whether a job whose attribute does not state either way is scheduled. Set it to `false` in one environment to park everything except the jobs that opt in explicitly.
            type: bool
            default: 'true'

          - name: Jobs
            description: Per-job overrides keyed by job id, matched ignoring case. A key naming a job the scan did not find stops the host, since that is nearly always a typo.
            type: 'Dictionary<string, RecurringJobOverride>'
            default: '{}'

    - name: Jobs
      description: One entry under `Jobs`. Every value is optional and an omitted one keeps what the attribute declared, so an entry only names what changes.
      fields:
          - name: Enabled
            description: Whether the job is scheduled, outranking both the attribute and `EnabledByDefault`.
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

<FrontmatterDocs/>
