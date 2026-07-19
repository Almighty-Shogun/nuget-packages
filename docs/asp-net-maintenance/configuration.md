# Configuration

ASP.NET Maintenance can read a `Maintenance` section from `appsettings.json`. The section is bound to `MaintenanceSettings` during `AddMaintenanceMode`, and the middleware uses the bound settings as defaults when no request-specific values are provided.

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

See [MaintenanceSettings](./configuration/maintenance-settings) for field descriptions and defaults. The complete section can be omitted when the built-in defaults are correct.
