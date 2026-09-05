---
fields:
    - name: MaintenanceSettings
      description: The optional `Maintenance` section, bound by [`AddMaintenanceMode`](./extensions/add-maintenance-mode). Its values are the defaults for every maintenance window, and any field a [`MaintenanceRequest`](./records/maintenance-request) sets wins for that window.
      fields:
          - name: MaintenancePath
            description: Request path that returns maintenance details while maintenance mode is enabled. Validated at startup, so a value carrying whitespace, a query string, or a fragment fails the host.
            type: string
            default: /maintenance

          - name: DefaultMessage
            description: Fallback message used when a maintenance request does not provide its own message.
            type: string?
            default: 'null'

          - name: AutoDisableWhenExpired
            description: Disables maintenance mode automatically when the persisted `EndsAt` value has passed.
            type: bool
            default: 'false'

          - name: RedirectBlockedRequests
            description: Allows blocked requests that accept `text/html` to be redirected to `MaintenancePath`. Requests that do not accept HTML always receive the error body instead.
            type: bool
            default: 'true'

          - name: AllowedPaths
            description: Exact request paths that remain available while maintenance mode is enabled.
            type: 'IReadOnlyList<string>'
            default: '[]'

          - name: AllowedPathPrefixes
            description: Request path prefixes that remain available while maintenance mode is enabled. Matching respects segment boundaries, so `/api` does not match `/apixyz`.
            type: 'IReadOnlyList<string>'
            default: '[]'

          - name: AllowedIpAddresses
            description: Client addresses that remain able to reach the application while maintenance mode is enabled. Read from the connection, never from a request header.
            type: 'IReadOnlyList<string>'
            default: '[]'
---

# Configuration

The `Maintenance` section holds the defaults every maintenance window starts from. It is optional, and each value below has a default of its own, so an application can register the services without adding the section.

```json
{
    "Maintenance": {
        "MaintenancePath": "/maintenance",
        "DefaultMessage": "The application is temporarily unavailable.",
        "AutoDisableWhenExpired": false,
        "RedirectBlockedRequests": true,
        "AllowedPaths": [
            "/health"
        ],
        "AllowedPathPrefixes": [
            "/admin"
        ],
        "AllowedIpAddresses": [
            "203.0.113.10"
        ]
    }
}
```

::: warning
An allow list supplied on a [`MaintenanceRequest`](./records/maintenance-request) replaces the configured one for that window rather than adding to it. A window that sets `AllowedPaths` loses the configured entries unless it repeats them.

Whatever endpoint disables maintenance mode has to survive that replacement, or the open window blocks the only route that could close it and the state file has to be deleted by hand.
:::

<FrontmatterDocs/>
