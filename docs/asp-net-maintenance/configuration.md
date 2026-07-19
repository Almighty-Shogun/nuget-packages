# Configuration

ASP.NET Maintenance can read a `Maintenance` section from `appsettings.json`. The section is bound to [`MaintenanceSettings`](./configuration/maintenance-settings) during [`AddMaintenanceMode`](./extensions/add-maintenance-mode), and the middleware uses the bound settings as defaults when no request-specific values are provided.

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
        ]
    }
}
```

The complete section can be omitted when the built-in defaults are correct.
